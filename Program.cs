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