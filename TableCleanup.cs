using Microsoft.Data.Sqlite;
using System;

/// <summary>
/// 清理重複表名工具 - 移除小寫表，只保留 PascalCase 表
/// </summary>
class TableCleanup
{
    private static readonly string connectionString = "Data Source=../DrinkShop.Api/data/drinkshop.db";
    
    static void Main(string[] args)
    {
        Console.WriteLine("開始清理重複表名，只保留 PascalCase 表...");
        
        try
        {
            CleanupTables();
            Console.WriteLine("✅ 表名清理完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 清理失敗: {ex.Message}");
        }
    }
    
    static void CleanupTables()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // 檢查並刪除小寫表（如果存在相應的 PascalCase 表）
        var tablesToDrop = new[] { "users", "products", "orders" };
        
        foreach (var table in tablesToDrop)
        {
            string pascalTable = char.ToUpper(table[0]) + table.Substring(1);
            
            // 檢查兩個表是否都存在
            bool lowerExists = TableExists(connection, table);
            bool pascalExists = TableExists(connection, pascalTable);
            
            if (lowerExists && pascalExists)
            {
                Console.WriteLine($"發現重複表: {table} 和 {pascalTable}");
                
                // 刪除小寫表
                var dropCmd = new SqliteCommand($"DROP TABLE {table};", connection);
                dropCmd.ExecuteNonQuery();
                Console.WriteLine($"✓ 已刪除小寫表: {table}");
            }
            else if (lowerExists && !pascalExists)
            {
                // 將小寫表重新命名為 PascalCase
                var renameCmd = new SqliteCommand($"ALTER TABLE {table} RENAME TO {pascalTable};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"✓ 已重新命名: {table} → {pascalTable}");
            }
        }
        
        // 特殊處理 order_items → OrderItems
        if (TableExists(connection, "order_items") && !TableExists(connection, "OrderItems"))
        {
            var renameCmd = new SqliteCommand("ALTER TABLE order_items RENAME TO OrderItems;", connection);
            renameCmd.ExecuteNonQuery();
            Console.WriteLine("✓ 已重新命名: order_items → OrderItems");
        }
        else if (TableExists(connection, "order_items") && TableExists(connection, "OrderItems"))
        {
            var dropCmd = new SqliteCommand("DROP TABLE order_items;", connection);
            dropCmd.ExecuteNonQuery();
            Console.WriteLine("✓ 已刪除小寫表: order_items");
        }
    }
    
    static bool TableExists(SqliteConnection connection, string tableName)
    {
        var cmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;", connection);
        cmd.Parameters.AddWithValue("@tableName", tableName);
        
        using var reader = cmd.ExecuteReader();
        return reader.Read();
    }
}
