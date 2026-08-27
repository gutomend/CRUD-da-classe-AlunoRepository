# ADOLab — CRUD de Alunos com ADO.NET

Projeto acadêmico desenvolvido para estudo de acesso a dados em C# utilizando ADO.NET e SQL Server. A solução contém uma biblioteca de domínio, uma aplicação Console e uma aplicação Web ASP.NET Core MVC, ambas conectadas ao mesmo banco de dados.

O projeto-base foi disponibilizado pelo professor. Nesta atividade, foram implementadas e testadas as operações CRUD da classe `AlunoRepository`.

## Objetivo

Implementar os métodos de acesso a dados necessários para:

- inserir alunos;
- listar todos os alunos;
- atualizar os dados de um aluno;
- excluir um aluno;
- buscar alunos por propriedade e valor.

## Estrutura do projeto

```text
ADOLab/
├── ADOLab/              # Domínio, interfaces e repositório
├── ADOLab.Console/      # Aplicação de linha de comando
├── ADOLab.Web/          # Aplicação Web ASP.NET Core MVC
├── ADOLab.sln           # Solução do Visual Studio
├── global.json          # Versão do SDK utilizada
└── README.md
```

## Tecnologias utilizadas

- C#;
- .NET 8 na biblioteca e na aplicação Console;
- ASP.NET Core MVC com .NET 10 na aplicação Web;
- ADO.NET;
- `Microsoft.Data.SqlClient`;
- SQL Server Express LocalDB;
- Visual Studio;
- Git e GitHub.

## Funcionalidades implementadas

### Inserir

Realiza um `INSERT` parametrizado e retorna o identificador gerado pelo SQL Server por meio de `SCOPE_IDENTITY()`.

### Listar

Consulta todos os registros da tabela `dbo.Alunos`, ordenados pelo identificador, e converte cada linha retornada pelo `SqlDataReader` em um objeto `Aluno`.

### Atualizar

Atualiza nome, idade, e-mail e data de nascimento do aluno selecionado pelo ID. O método retorna a quantidade de registros afetados.

### Excluir

Exclui o aluno correspondente ao ID informado e retorna a quantidade de registros afetados.

### Buscar

Permite pesquisar pelas propriedades:

- `Id`;
- `Nome`;
- `Idade`;
- `Email`;
- `DataNascimento`.

A propriedade recebida é validada por uma lista de colunas permitidas. Os valores são enviados por parâmetros, reduzindo o risco de SQL Injection e mantendo os tipos compatíveis com o banco.

## Banco de dados

A aplicação utiliza uma instância do SQL Server Express LocalDB e o banco `AlunosDB`.

Antes da primeira execução, crie o banco com o nome:

```text
AlunosDB
```

No Visual Studio, isso pode ser feito em **Exibir → Pesquisador de Objetos do SQL Server**, conectando à instância:

```text
(localdb)\MSSQLLocalDB
```

O método `GarantirEsquema()` cria automaticamente a tabela `dbo.Alunos` caso ela ainda não exista.

### Connection string

Configure o arquivo `appsettings.json` da aplicação que será executada:

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AlunosDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

A mesma connection string deve ser utilizada em:

```text
ADOLab.Console/appsettings.json
ADOLab.Web/appsettings.json
```

## Pré-requisitos

- Visual Studio com suporte ao desenvolvimento .NET e ASP.NET;
- SDK do .NET 8;
- SDK do .NET 10;
- SQL Server Express LocalDB;
- SQL Server Data Tools, recomendado para administrar o banco pelo Visual Studio.

Para verificar os SDKs instalados:

```powershell
dotnet --list-sdks
```

Para verificar a instância LocalDB:

```powershell
sqllocaldb info
```

## Como executar

Na pasta raiz da solução, restaure as dependências e compile:

```powershell
dotnet restore
dotnet build ADOLab.sln
```

### Aplicação Console

```powershell
dotnet run --project ADOLab.Console/ADOLab.Console.csproj
```

O menu disponibiliza as cinco operações implementadas:

```text
=== CRUD ADO.NET – Alunos ===
1) Inserir
2) Listar
3) Editar
4) Deletar
5) Buscar
0) Sair
```

As operações realizadas no Console são registradas no arquivo `log.txt`.

### Aplicação Web

```powershell
dotnet run --project ADOLab.Web/ADOLab.Web.csproj
```

Após a inicialização, acesse o endereço exibido no terminal, normalmente:

```text
http://localhost:5000
```

A aplicação Web utiliza o mesmo banco da aplicação Console. Dessa forma, registros inseridos em uma interface também podem ser visualizados e alterados pela outra.

## Validações e segurança

- comandos SQL executados com parâmetros;
- validação das propriedades permitidas no método de busca;
- conversão adequada dos valores numéricos e de data;
- uso de `using` para liberar conexões, comandos e leitores;
- tratamento de `SqlException`, `ArgumentException` e exceções gerais;
- mensagens para registros não encontrados;
- registro das operações da aplicação Console em arquivo de log.

## Testes realizados

Foram testados com sucesso:

- criação automática da tabela `dbo.Alunos`;
- inserção de alunos pelo Console e pela Web;
- listagem dos registros nas duas aplicações;
- edição dos dados de um aluno;
- exclusão por ID;
- busca por nome, ID, idade, e-mail e data de nascimento;
- compartilhamento dos dados entre Console e Web;
- compilação completa da solução.

## Observações

- As pastas `bin`, `obj` e `.vs` não devem ser enviadas ao repositório.
- O arquivo `log.txt` gerado em execução também pode ser ignorado pelo Git.
- Antes de recompilar a aplicação Web, encerre o processo em execução com `Ctrl + C` ou `Shift + F5` para evitar bloqueio do arquivo `ADOLab.Web.exe`.

## Integrantes

- **Augusto Mendonça** — RM558371
- **Gabriel Vasquez** — RM557056
- **Gustavo Oliveira** — RM559163

Engenharia de Software — FIAP

## Créditos

Projeto-base disponibilizado pelo professor para a atividade acadêmica. Implementação do CRUD, configuração do ambiente e testes realizados pela equipe.
