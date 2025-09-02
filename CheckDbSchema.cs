using System;
using System.Data;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        string dbPath = @"D:\Lab\DrinkShop\DrinkShop.Api\data\drinkshop.db";
        string connectionString = $"Data Source={dbPath}";
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // 查詢 users 表的結構
        string query = "PRAGMA table_info(users);";
        using var command = new SqliteCommand(query, connection);
        using var reader = command.ExecuteReader();
        
        Console.WriteLine("Users table structure:");
        Console.WriteLine("cid | name | type | notnull | dflt_value | pk");
        Console.WriteLine("----+------+------+---------+------------+---");
        
        while (reader.Read())
        {
            Console.WriteLine($"{reader["cid"]} | {reader["name"]} | {reader["type"]} | {reader["notnull"]} | {reader["dflt_value"]} | {reader["pk"]}");
        }
    }
}
