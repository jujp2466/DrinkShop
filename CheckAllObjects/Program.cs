using Microsoft.Data.Sqlite;
using System;

class CheckAllObjects
{
	static void Main()
	{
		string connectionString = "Data Source=D:\\Lab\\DrinkShop\\DrinkShop.Api\\data\\drinkshop.db";
        
		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();

			Console.WriteLine("=== 檢查所有物件 ===");
			var command = new SqliteCommand(
				"SELECT type, name FROM sqlite_master ORDER BY type, name;",
				connection);
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					Console.WriteLine($"{reader["type"]}: {reader["name"]}");
				}
			}
		}
	}
}
