using Microsoft.Data.Sqlite;
using System;

class DatabaseVerifier
{
    static void Main()
    {
        string connectionString = "Data Source=D:\\Lab\\DrinkShop\\DrinkShop.Api\\data\\drinkshop.db";
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            Console.WriteLine("=== 檢查所有資料表 ===");
            var command = new SqliteCommand(
                "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;",
                connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"資料表: {reader["name"]}");
                }
            }

            Console.WriteLine("\n=== Users 資料表結構 ===");
            command = new SqliteCommand("PRAGMA table_info(Users);", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"欄位: {reader["name"]} ({reader["type"]})");
                }
            }

            Console.WriteLine("\n=== Products 資料表結構 ===");
            command = new SqliteCommand("PRAGMA table_info(Products);", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"欄位: {reader["name"]} ({reader["type"]})");
                }
            }

            Console.WriteLine("\n=== Orders 資料表結構 ===");
            command = new SqliteCommand("PRAGMA table_info(Orders);", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"欄位: {reader["name"]} ({reader["type"]})");
                }
            }

            Console.WriteLine("\n=== OrderItems 資料表結構 ===");
            command = new SqliteCommand("PRAGMA table_info(OrderItems);", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"欄位: {reader["name"]} ({reader["type"]})");
                }
            }
        }
    }
}
