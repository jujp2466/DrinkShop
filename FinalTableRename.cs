using Microsoft.Data.Sqlite;
using System;

class FinalTableRename
{
    private static readonly string connectionString = "Data Source=../DrinkShop.Api/data/drinkshop.db";
    
    static void Main(string[] args)
    {
        Console.WriteLine("最終表名統一為 PascalCase...");
        
        try
        {
            RenameAllTables();
            Console.WriteLine("✅ 所有表名已統一為 PascalCase！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 重新命名失敗: {ex.Message}");
        }
    }
    
    static void RenameAllTables()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // 將所有小寫表名改為 PascalCase
        var renames = new Dictionary<string, string>
        {
            {"users", "Users"},
            {"products", "Products"},
            {"orders", "Orders"}
        };
        
        foreach (var rename in renames)
        {
            try
            {
                var cmd = new SqliteCommand($"ALTER TABLE {rename.Key} RENAME TO {rename.Value};", connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine($"✓ {rename.Key} → {rename.Value}");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"- {rename.Key} 跳過: {ex.Message}");
            }
        }
        
        // 顯示最終結果
        Console.WriteLine("\n=== 最終表名清單 ===");
        var tablesCmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE '__%';", connection);
        using (var reader = tablesCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"表名: {reader.GetString(0)}");
            }
        }
    }
}
