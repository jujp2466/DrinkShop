using Microsoft.Data.Sqlite;
using System;
using System.IO;

/// <summary>
/// SQLite Schema 轉換工具 - 統一所有欄位為 PascalCase
/// </summary>
class PascalCaseConverter
{
    private static readonly string connectionString = "Data Source=../DrinkShop.Api/data/drinkshop.db";
    
    static void Main(string[] args)
    {
        Console.WriteLine("檢查資料庫表名與欄位狀態...");
        
        try
        {
            CheckDatabaseStatus();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 檢查失敗: {ex.Message}");
        }
    }
    
    static void CheckDatabaseStatus()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // 顯示所有表名
        Console.WriteLine("=== 資料庫中的所有表 ===");
        var tablesCmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection);
        using (var tablesReader = tablesCmd.ExecuteReader())
        {
            while (tablesReader.Read())
            {
                Console.WriteLine($"表名: {tablesReader.GetString(0)}");
            }
        }
        Console.WriteLine();
        
        // 檢查 users/Users 表結構
        CheckTableStructure(connection, "users");
        CheckTableStructure(connection, "Users");
    }
    
    static void CheckTableStructure(SqliteConnection connection, string tableName)
    {
        try
        {
            Console.WriteLine($"=== {tableName} 表結構 ===");
            var cmd = new SqliteCommand($"PRAGMA table_info({tableName});", connection);
            using var reader = cmd.ExecuteReader();
            
            Console.WriteLine("cid | name           | type    | notnull | dflt_value | pk");
            Console.WriteLine("----+----------------+---------+---------+------------+---");
            
            while (reader.Read())
            {
                var cid = reader.GetInt32(0);
                var name = reader.GetString(1);
                var type = reader.GetString(2);
                var notnull = reader.GetInt32(3);
                var dfltValue = reader.IsDBNull(4) ? "" : reader.GetString(4);
                var pk = reader.GetInt32(5);
                
                Console.WriteLine($"{cid,-4}| {name,-15}| {type,-8}| {notnull,-8}| {dfltValue,-11}| {pk}");
            }
            Console.WriteLine();
        }
        catch (SqliteException)
        {
            Console.WriteLine($"{tableName} 表不存在");
            Console.WriteLine();
        }
    }
    
    static void ConvertToPascalCase()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // 1. 轉換 users 表欄位
        Console.WriteLine("轉換 users 表欄位...");
        ConvertUsersTable(connection);
        
        // 2. 轉換 products 表欄位
        Console.WriteLine("轉換 products 表欄位...");
        ConvertProductsTable(connection);
        
        // 3. 轉換 orders 表欄位
        Console.WriteLine("轉換 orders 表欄位...");
        ConvertOrdersTable(connection);
        
        // 4. 轉換 order_items 表欄位
        Console.WriteLine("轉換 order_items 表欄位...");
        ConvertOrderItemsTable(connection);
        
        // 5. 重新命名表名為 PascalCase
        Console.WriteLine("重新命名表名為 PascalCase...");
        RenameTablesToPascalCase(connection);
    }
    
    static void ConvertUsersTable(SqliteConnection connection)
    {
        // 先檢查欄位是否存在，避免重複轉換
        var checkColumns = new SqliteCommand("PRAGMA table_info(users);", connection);
        var columnNames = new List<string>();
        using (var reader = checkColumns.ExecuteReader())
        {
            while (reader.Read())
            {
                columnNames.Add(reader.GetString(1)); // name 在第 1 個位置 (0-indexed)
            }
        }
        
        // 只轉換需要的欄位
        var columnMappings = new Dictionary<string, string>
        {
            {"username", "UserName"},
            {"email", "Email"},
            {"password", "Password"},
            {"role", "Role"},
            {"phone", "Phone"},
            {"address", "Address"},
            {"status", "Status"},
            {"last_login_at", "LastLoginAt"},
            {"created_at", "CreatedAt"}
        };
        
        foreach (var mapping in columnMappings)
        {
            if (columnNames.Contains(mapping.Key))
            {
                var renameCmd = new SqliteCommand($"ALTER TABLE users RENAME COLUMN {mapping.Key} TO {mapping.Value};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"  ✓ {mapping.Key} → {mapping.Value}");
            }
        }
    }
    
    static void ConvertProductsTable(SqliteConnection connection)
    {
        var columnMappings = new Dictionary<string, string>
        {
            {"name", "Name"},
            {"description", "Description"},
            {"price", "Price"},
            {"category", "Category"},
            {"image_url", "ImageUrl"},
            {"stock", "Stock"},
            {"is_active", "IsActive"},
            {"created_at", "CreatedAt"}
        };
        
        foreach (var mapping in columnMappings)
        {
            try
            {
                var renameCmd = new SqliteCommand($"ALTER TABLE products RENAME COLUMN {mapping.Key} TO {mapping.Value};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"  ✓ {mapping.Key} → {mapping.Value}");
            }
            catch (SqliteException)
            {
                // 欄位可能已經是 PascalCase 或不存在
                Console.WriteLine($"  - {mapping.Key} 跳過");
            }
        }
    }
    
    static void ConvertOrdersTable(SqliteConnection connection)
    {
        var columnMappings = new Dictionary<string, string>
        {
            {"user_id", "UserId"},
            {"total_amount", "TotalAmount"},
            {"shipping_fee", "ShippingFee"},
            {"status", "Status"},
            {"shipping_address", "ShippingAddress"},
            {"payment_method", "PaymentMethod"},
            {"notes", "Notes"},
            {"created_at", "CreatedAt"}
        };
        
        foreach (var mapping in columnMappings)
        {
            try
            {
                var renameCmd = new SqliteCommand($"ALTER TABLE orders RENAME COLUMN {mapping.Key} TO {mapping.Value};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"  ✓ {mapping.Key} → {mapping.Value}");
            }
            catch (SqliteException)
            {
                Console.WriteLine($"  - {mapping.Key} 跳過");
            }
        }
    }
    
    static void ConvertOrderItemsTable(SqliteConnection connection)
    {
        var columnMappings = new Dictionary<string, string>
        {
            {"order_id", "OrderId"},
            {"product_id", "ProductId"},
            {"quantity", "Quantity"},
            {"price", "Price"}
        };
        
        foreach (var mapping in columnMappings)
        {
            try
            {
                var renameCmd = new SqliteCommand($"ALTER TABLE order_items RENAME COLUMN {mapping.Key} TO {mapping.Value};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"  ✓ {mapping.Key} → {mapping.Value}");
            }
            catch (SqliteException)
            {
                Console.WriteLine($"  - {mapping.Key} 跳過");
            }
        }
    }
    
    static void RenameTablesToPascalCase(SqliteConnection connection)
    {
        var tableMappings = new Dictionary<string, string>
        {
            {"users", "Users"},
            {"products", "Products"},
            {"orders", "Orders"},
            {"order_items", "OrderItems"}
        };
        
        foreach (var mapping in tableMappings)
        {
            try
            {
                var renameCmd = new SqliteCommand($"ALTER TABLE {mapping.Key} RENAME TO {mapping.Value};", connection);
                renameCmd.ExecuteNonQuery();
                Console.WriteLine($"  ✓ 表名: {mapping.Key} → {mapping.Value}");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"  - 表名 {mapping.Key} 轉換跳過: {ex.Message}");
            }
        }
    }
}
