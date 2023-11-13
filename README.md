# Lista de Exercícios X: HashSet

## UC - Estrutura de dados e análise de algoritmos -Pratica

**Pedro Antônio Esteves Silva - RA: 622122907**

Considerações Iniciais:
Esta lista de exercício deve:

- Ser realizada em equipes de até 06 alunos.
- Ser entregue no prazo proposto.
- Todos os integrantes devem enviar a lista na plataforma.
- Ter os algoritmos pedidos escritos em linguagem C#(csharp) ou Java.
- Ter todos os algoritmos devidamente identados.

1- Utilizando a Estrutura de Dados HashSet, crie um programa para criar um sistema de gerenciamento de contas bancárias.

O programa permitirá ao usuário criar contas, depositar e sacar dinheiro, verificar saldos e listar todas as contas existentes neste banco.

Você deverá criar uma classe chamada ContaBancaria e a mesma deverá ter três atributos: int Numero; String Titular; Double Saldo;

O Número da Conta é formado por 4 algarismos aleatórios no intervalo de 1000 a 1999.

Não é preciso fazer nenhuma validação ao gerar um novo número para uma nova conta. 

Seguem imagens do programa:

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/8c95bb29-a724-48e5-bfc2-06ebdff4b74e/97ee445a-c52f-4095-9297-02b7c1322fb3/Untitled.png)

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/8c95bb29-a724-48e5-bfc2-06ebdff4b74e/297fda41-baa1-4578-8e58-7752af47dce7/Untitled.png)

2. Persista as informações da conta e todas as operações de CRUD em um banco de dados SQlite.

A biblioteca utilizada para te apoiar no projeto: 

`dotnet add package Microsoft.Data.Sqlite`

Para apoiar sua tarefa, segue banco de dados já criado:

https://github.com/danhpaiva/sistema-gerenciamento-conta-bancaria-net-7-sqlite

```csharp
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

class ContaBancaria
{
    public int Numero { get; }
    public string Titular { get; set; }
    public double Saldo { get; set; }

    public ContaBancaria(int numero, string titular, double saldo)
    {
        Numero = numero;
        Titular = titular;
        Saldo = saldo;
    }
}

class Program
{
    static string connectionString = "Data Source=contas.db";

    static void Main()
    {
        CriarTabelaContas();

        HashSet<ContaBancaria> contas = CarregarContasDoBanco();

        int opcao;
        do
        {
            Console.WriteLine("======= Sistema Bancário =======");
            Console.WriteLine("1 - Criar Nova Conta");
            Console.WriteLine("2 - Listar Contas");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");
            opcao = Convert.ToInt32(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    CriarNovaConta(contas);
                    break;

                case 2:
                    ListarContas(contas);
                    break;

                case 0:
                    Console.WriteLine("Sistema encerrado.");
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.\n");
                    break;
            }

        } while (opcao != 0);
    }

    static void CriarTabelaContas()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Contas (
                    Numero INTEGER PRIMARY KEY,
                    Titular TEXT NOT NULL,
                    Saldo REAL NOT NULL
                );
            ";
            createTableCommand.ExecuteNonQuery();
        }
    }

    static HashSet<ContaBancaria> CarregarContasDoBanco()
    {
        HashSet<ContaBancaria> contas = new HashSet<ContaBancaria>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM Contas";

            using (var reader = selectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int numero = reader.GetInt32(0);
                    string titular = reader.GetString(1);
                    double saldo = reader.GetDouble(2);

                    contas.Add(new ContaBancaria(numero, titular, saldo));
                }
            }
        }

        return contas;
    }

    static void InserirContaNoBanco(ContaBancaria conta)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Contas (Numero, Titular, Saldo) VALUES (@Numero, @Titular, @Saldo)";
            insertCommand.Parameters.AddWithValue("@Numero", conta.Numero);
            insertCommand.Parameters.AddWithValue("@Titular", conta.Titular);
            insertCommand.Parameters.AddWithValue("@Saldo", conta.Saldo);

            insertCommand.ExecuteNonQuery();
        }
    }

    static void CriarNovaConta(HashSet<ContaBancaria> contas)
    {
        int numeroConta = GerarNumeroConta();
        Console.Write("Digite o nome do titular da conta: ");
        string titular = Console.ReadLine();
        Console.Write("Digite o saldo inicial da conta: ");
        double saldoInicial = Convert.ToDouble(Console.ReadLine());

        ContaBancaria novaConta = new ContaBancaria(numeroConta, titular, saldoInicial);
        contas.Add(novaConta);

        InserirContaNoBanco(novaConta);

        Console.WriteLine($"Conta criada com sucesso! Número da Conta: {numeroConta}\n");
    }

    static void ListarContas(HashSet<ContaBancaria> contas)
    {
        Console.WriteLine("\nLista de Contas Bancárias:");
        foreach (var conta in contas)
        {
            Console.WriteLine($"Número: {conta.Numero}, Titular: {conta.Titular}, Saldo: {conta.Saldo:C}");
        }
        Console.WriteLine();
    }

    static int GerarNumeroConta()
    {
        Random random = new Random();
        return random.Next(1000, 2000);
    }
}
```

3. Versione o programa no GitHub com o nome “lista-10-csharp-sqlite” e coloque