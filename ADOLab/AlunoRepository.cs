using System.Data;
using Microsoft.Data.SqlClient;

/// <summary>
/// Classe de reposit�rio para gerenciar entidades Aluno no banco de dados.
/// </summary>
public class AlunoRepository : IRepository<Aluno>
{
    /// <summary>
    /// Obt�m ou define a string de conex�o com o banco de dados.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Inicializa uma nova inst�ncia da classe <see cref="AlunoRepository"/>.
    /// </summary>
    /// <param name="connectionString">A string de conex�o com o banco de dados.</param>
    public AlunoRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// Garante que o esquema do banco de dados para a tabela Aluno exista.
    /// </summary>
    public void GarantirEsquema()
    {
        const string ddl = @"
        IF OBJECT_ID('dbo.Alunos', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Alunos (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Nome NVARCHAR(100) NOT NULL,
                Idade INT NOT NULL,
                Email NVARCHAR(100) NOT NULL,
                DataNascimento DATE NOT NULL
            );
        END";
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(ddl, conn) { CommandType = CommandType.Text, CommandTimeout = 30 };
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Insere um novo registro de Aluno no banco de dados.
    /// </summary>
    /// <param name="nome">O nome do Aluno.</param>
    /// <param name="idade">A idade do Aluno.</param>
    /// <param name="email">O email do Aluno.</param>
    /// <param name="dataNascimento">A data de nascimento do Aluno.</param>
    /// <returns>O ID do Aluno rec�m-inserido.</returns>
    public int Inserir(string nome, int idade, string email, DateTime dataNascimento)
    {
        const string sql = @"
        INSERT INTO dbo.Alunos (Nome, Idade, Email, DataNascimento)
        VALUES (@Nome, @Idade, @Email, @DataNascimento);

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 100).Value = nome;
        cmd.Parameters.Add("@Idade", SqlDbType.Int).Value = idade;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
        cmd.Parameters.Add("@DataNascimento", SqlDbType.Date).Value = dataNascimento.Date;

        conn.Open();

        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Recupera uma lista de todos os registros de Aluno do banco de dados.
    /// </summary>
    /// <returns>Uma lista de entidades Aluno.</returns>
    public List<Aluno> Listar()
    {
        const string sql = @"
        SELECT Id, Nome, Idade, Email, DataNascimento
        FROM dbo.Alunos
        ORDER BY Id;";

        var alunos = new List<Aluno>();

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        conn.Open();

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var aluno = new Aluno(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Nome")),
                reader.GetInt32(reader.GetOrdinal("Idade")),
                reader.GetString(reader.GetOrdinal("Email")),
                reader.GetDateTime(reader.GetOrdinal("DataNascimento"))
            );

            alunos.Add(aluno);
        }

        return alunos;
    }
    public int Atualizar(
    int id,
    string nome,
    int idade,
    string email,
    DateTime dataNascimento)
    {
        const string sql = @"
        UPDATE dbo.Alunos
        SET Nome = @Nome,
            Idade = @Idade,
            Email = @Email,
            DataNascimento = @DataNascimento
        WHERE Id = @Id;";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 100).Value = nome;
        cmd.Parameters.Add("@Idade", SqlDbType.Int).Value = idade;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
        cmd.Parameters.Add("@DataNascimento", SqlDbType.Date).Value =
            dataNascimento.Date;

        conn.Open();

        return cmd.ExecuteNonQuery();
    }
    public int Excluir(int id)
    {
        const string sql = @"
        DELETE FROM dbo.Alunos
        WHERE Id = @Id;";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        conn.Open();

        return cmd.ExecuteNonQuery();
    }
    public List<Aluno> Buscar(string propriedade, object valor)
    {
        string valorTexto = valor?.ToString()
            ?? throw new ArgumentException(
                "O valor da busca não pode ser nulo.",
                nameof(valor));

        var colunasPermitidas = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
    {
        { "Id", "Id" },
        { "Nome", "Nome" },
        { "Idade", "Idade" },
        { "Email", "Email" },
        { "DataNascimento", "DataNascimento" }
    };

        if (!colunasPermitidas.TryGetValue(propriedade, out string? coluna))
        {
            throw new ArgumentException(
                "Propriedade inválida. Utilize: Id, Nome, Idade, Email ou DataNascimento.",
                nameof(propriedade));
        }

        string sql = $@"
        SELECT Id, Nome, Idade, Email, DataNascimento
        FROM dbo.Alunos
        WHERE {coluna} = @Valor
        ORDER BY Id;";

        var alunos = new List<Aluno>();

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        switch (coluna)
        {
            case "Id":
            case "Idade":
                if (!int.TryParse(valorTexto, out int numero))
                {
                    throw new ArgumentException(
                        $"O valor de {coluna} deve ser um número inteiro.",
                        nameof(valor));
                }

                cmd.Parameters.Add("@Valor", SqlDbType.Int).Value = numero;
                break;

            case "DataNascimento":
                if (!DateTime.TryParse(valorTexto, out DateTime data))
                {
                    throw new ArgumentException(
                        "A data deve estar no formato yyyy-MM-dd.",
                        nameof(valor));
                }

                cmd.Parameters.Add("@Valor", SqlDbType.Date).Value = data.Date;
                break;

            default:
                cmd.Parameters
                    .Add("@Valor", SqlDbType.NVarChar, 100)
                    .Value = valorTexto;
                break;
        }

        conn.Open();

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var aluno = new Aluno(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Nome")),
                reader.GetInt32(reader.GetOrdinal("Idade")),
                reader.GetString(reader.GetOrdinal("Email")),
                reader.GetDateTime(reader.GetOrdinal("DataNascimento"))
            );

            alunos.Add(aluno);
        }

        return alunos;
    }
}