using Microsoft.Data.Sqlite;

string dbPath = @"D:\Lab\DrinkShop\DrinkShop.Api\data\drinkshop.db";
string connectionString = $"Data Source={dbPath}";

try
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    
    Console.WriteLine("開始手動移除 Account 欄位...");
    
    // 停用外鍵約束檢查
    using (var cmd = new SqliteCommand("PRAGMA foreign_keys = OFF;", connection))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 開始事務
    using var transaction = connection.BeginTransaction();
    
    // 1. 建立新的 users 表格（不包含 Account 欄位）
    string createNewTableSql = @"
        CREATE TABLE users_new (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            IsActive INTEGER NOT NULL,
            PasswordHash TEXT NOT NULL,
            address TEXT,
            created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
            email TEXT NOT NULL,
            last_login_at TEXT,
            password TEXT NOT NULL,
            phone TEXT NOT NULL,
            role TEXT NOT NULL DEFAULT 'user',
            status TEXT NOT NULL DEFAULT 'active',
            username TEXT NOT NULL
        );";
    
    using (var cmd = new SqliteCommand(createNewTableSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 2. 複製資料（除了 Account 欄位）
    string copyDataSql = @"
        INSERT INTO users_new (Id, IsActive, PasswordHash, address, created_at, email, last_login_at, password, phone, role, status, username)
        SELECT Id, IsActive, PasswordHash, address, created_at, email, last_login_at, password, phone, role, status, username
        FROM users;";
    
    using (var cmd = new SqliteCommand(copyDataSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 3. 刪除舊表格
    string dropOldTableSql = "DROP TABLE users;";
    using (var cmd = new SqliteCommand(dropOldTableSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 4. 重新命名新表格
    string renameTableSql = "ALTER TABLE users_new RENAME TO users;";
    using (var cmd = new SqliteCommand(renameTableSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 5. 重新建立索引
    string createUsernameIndexSql = "CREATE UNIQUE INDEX IX_users_username ON users (username);";
    using (var cmd = new SqliteCommand(createUsernameIndexSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    string createEmailIndexSql = "CREATE UNIQUE INDEX IX_users_email ON users (email);";
    using (var cmd = new SqliteCommand(createEmailIndexSql, connection, transaction))
    {
        cmd.ExecuteNonQuery();
    }
    
    // 提交事務
    transaction.Commit();
    
    // 重新啟用外鍵約束檢查
    using (var cmd = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
    {
        cmd.ExecuteNonQuery();
    }
    
    Console.WriteLine("✅ Account 欄位已成功移除！");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ 錯誤: {ex.Message}");
}
