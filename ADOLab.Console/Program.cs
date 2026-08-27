using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ADOLab;

class Program
{
    static async Task Main(string[] args)
    {
        #region Configuração

        var config = new ConfigurationBuilder()
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        string connString = config.GetConnectionString(
            "SqlServerConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:SqlServerConnection não encontrada.");

        #endregion

        var logger = new FileLogger("log.txt");

        try
        {
            var alunoRepo = new AlunoRepository(connString);

            await logger.LogAsync(
                "Iniciando aplicação e garantindo o esquema.");

            alunoRepo.GarantirEsquema();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== CRUD ADO.NET – Alunos ===");
                Console.WriteLine("1) Inserir");
                Console.WriteLine("2) Listar");
                Console.WriteLine("3) Editar");
                Console.WriteLine("4) Deletar");
                Console.WriteLine("5) Buscar");
                Console.WriteLine("0) Sair");
                Console.Write("Escolha: ");

                string? opcao = Console.ReadLine();

                if (opcao == "0")
                {
                    await logger.LogAsync("Aplicação encerrada pelo usuário.");
                    Console.WriteLine("Aplicação encerrada.");
                    break;
                }

                switch (opcao)
                {
                    case "1":
                        Console.Write("Nome: ");
                        string nome = Console.ReadLine() ?? "";

                        Console.Write("Idade: ");
                        string? idadeTexto = Console.ReadLine();

                        Console.Write("Email: ");
                        string email = Console.ReadLine() ?? "";

                        Console.Write("Data de nascimento (yyyy-MM-dd): ");
                        string? dataTexto = Console.ReadLine();

                        bool idadeValida = int.TryParse(
                            idadeTexto,
                            out int idade);

                        bool dataValida = DateTime.TryParse(
                            dataTexto,
                            out DateTime dataNascimento);

                        if (idadeValida && dataValida)
                        {
                            int id = alunoRepo.Inserir(
                                nome,
                                idade,
                                email,
                                dataNascimento);

                            Console.WriteLine($"✅ Inserido com Id={id}.");

                            await logger.LogAsync(
                                $"Inserido aluno com Id={id}, " +
                                $"Nome={nome}, " +
                                $"Idade={idade}, " +
                                $"Email={email}, " +
                                $"DataNascimento={dataNascimento:yyyy-MM-dd}.");
                        }
                        else
                        {
                            Console.WriteLine("Dados inválidos.");

                            await logger.LogWarningAsync(
                                "Falha ao inserir aluno devido a dados inválidos.");
                        }

                        break;

                    case "2":
                        var alunos = alunoRepo.Listar();

                        Console.WriteLine();
                        Console.WriteLine("== Lista de Alunos ==");

                        foreach (var aluno in alunos)
                        {
                            Console.WriteLine(
                                $"#{aluno.Id} " +
                                $"{aluno.Nome} " +
                                $"({aluno.Idade}) - " +
                                $"{aluno.Email} - " +
                                $"{aluno.DataNascimento:yyyy-MM-dd}");
                        }

                        if (alunos.Count == 0)
                        {
                            Console.WriteLine("(vazio)");
                        }

                        await logger.LogAsync("Listou todos os alunos.");
                        break;

                    case "3":
                        Console.Write("Id: ");
                        string? idEdicaoTexto = Console.ReadLine();

                        Console.Write("Novo nome: ");
                        string novoNome = Console.ReadLine() ?? "";

                        Console.Write("Nova idade: ");
                        string? novaIdadeTexto = Console.ReadLine();

                        Console.Write("Novo email: ");
                        string novoEmail = Console.ReadLine() ?? "";

                        Console.Write(
                            "Nova data de nascimento (yyyy-MM-dd): ");

                        string? novaDataTexto = Console.ReadLine();

                        bool idEdicaoValido = int.TryParse(
                            idEdicaoTexto,
                            out int idEdicao);

                        bool novaIdadeValida = int.TryParse(
                            novaIdadeTexto,
                            out int novaIdade);

                        bool novaDataValida = DateTime.TryParse(
                            novaDataTexto,
                            out DateTime novaDataNascimento);

                        if (idEdicaoValido &&
                            novaIdadeValida &&
                            novaDataValida)
                        {
                            int registrosAfetados = alunoRepo.Atualizar(
                                idEdicao,
                                novoNome,
                                novaIdade,
                                novoEmail,
                                novaDataNascimento);

                            if (registrosAfetados > 0)
                            {
                                Console.WriteLine("✅ Atualizado.");

                                await logger.LogAsync(
                                    $"Atualizado aluno Id={idEdicao} com " +
                                    $"Nome={novoNome}, " +
                                    $"Idade={novaIdade}, " +
                                    $"Email={novoEmail}, " +
                                    $"DataNascimento=" +
                                    $"{novaDataNascimento:yyyy-MM-dd}.");
                            }
                            else
                            {
                                Console.WriteLine(
                                    "⚠️ Nenhum registro afetado.");

                                await logger.LogWarningAsync(
                                    $"Nenhum aluno encontrado com " +
                                    $"Id={idEdicao} para atualização.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Dados inválidos.");

                            await logger.LogWarningAsync(
                                "Falha ao atualizar aluno devido " +
                                "a dados inválidos.");
                        }

                        break;

                    case "4":
                        Console.Write("Id: ");
                        string? idExclusaoTexto = Console.ReadLine();

                        if (int.TryParse(
                            idExclusaoTexto,
                            out int idExclusao))
                        {
                            int registrosAfetados =
                                alunoRepo.Excluir(idExclusao);

                            if (registrosAfetados > 0)
                            {
                                Console.WriteLine("✅ Deletado.");

                                await logger.LogAsync(
                                    $"Deletado aluno com Id={idExclusao}.");
                            }
                            else
                            {
                                Console.WriteLine(
                                    "⚠️ Nenhum registro afetado.");

                                await logger.LogWarningAsync(
                                    $"Nenhum aluno encontrado com " +
                                    $"Id={idExclusao} para exclusão.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Id inválido.");

                            await logger.LogWarningAsync(
                                "Falha ao deletar aluno devido " +
                                "a Id inválido.");
                        }

                        break;

                    case "5":
                        Console.Write("Propriedade (coluna): ");
                        string propriedade = Console.ReadLine() ?? "";

                        Console.Write("Valor: ");
                        string valor = Console.ReadLine() ?? "";

                        var resultados = alunoRepo.Buscar(
                            propriedade,
                            valor);

                        Console.WriteLine();
                        Console.WriteLine("== Resultados da Busca ==");

                        foreach (var aluno in resultados)
                        {
                            Console.WriteLine(
                                $"#{aluno.Id} " +
                                $"{aluno.Nome} " +
                                $"({aluno.Idade}) - " +
                                $"{aluno.Email} - " +
                                $"{aluno.DataNascimento:yyyy-MM-dd}");
                        }

                        if (resultados.Count == 0)
                        {
                            Console.WriteLine("(vazio)");
                        }

                        await logger.LogAsync(
                            $"Busca realizada pela propriedade " +
                            $"'{propriedade}' com valor '{valor}'.");

                        break;

                    default:
                        Console.WriteLine("Opção inválida.");

                        await logger.LogWarningAsync(
                            "Opção de menu inválida selecionada.");

                        break;
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                $"[ERRO SQL] {ex.Number} - {ex.Message}");

            await logger.LogErrorAsync(
                $"Erro SQL {ex.Number}: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[DADOS INVÁLIDOS] {ex.Message}");

            await logger.LogWarningAsync(
                $"Argumento inválido: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {ex.Message}");

            await logger.LogErrorAsync(
                $"Exceção não tratada: {ex.Message}");
        }
    }
}