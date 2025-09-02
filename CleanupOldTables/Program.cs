using Microsoft.Data.Sqlite;
using System;

class CleanupOldTables
{
    static void Main()
    {
        string connectionString = "Data Source=D:\\Lab\\DrinkShop\\DrinkShop.Api\\data\\drinkshop.db";
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            Console.WriteLine("正在刪除 Orders 相關物件...");
            var dropSqls = new[]
            {
                "DROP INDEX IF EXISTS IX_orders_user_id;",
                "DROP INDEX IF EXISTS IX_order_items_order_id;",
                "DROP INDEX IF EXISTS IX_order_items_product_id;",
                "DROP TABLE IF EXISTS orders;",
                "DROP TABLE IF EXISTS Orders;"
            };
            foreach (var sql in dropSqls)
            {
                try
                {
                    var command = new SqliteCommand(sql, connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"執行成功: {sql}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"執行失敗: {sql} - {ex.Message}");
                }
            }
            Console.WriteLine("\n=== 檢查剩餘的資料庫物件 ===");
            var command2 = new SqliteCommand(
                "SELECT type, name FROM sqlite_master ORDER BY type, name;",
                connection);
            using (var reader = command2.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["type"]}: {reader["name"]}");
                }
            }
        }
    }
}
