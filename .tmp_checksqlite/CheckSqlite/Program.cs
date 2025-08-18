using System;
using Microsoft.Data.Sqlite;
class Program{
  static void Main(){
    string[] paths = new[] { @"D:\Lab\DrinkShop\DrinkShop.Api\drinkshop.db", @"D:\Lab\DrinkShop\DrinkShop.Api\data\drinkshop.db" };
    foreach(var p in paths){
        Console.WriteLine($"DB: {p}");
        if(!System.IO.File.Exists(p)){
            Console.WriteLine("  (not found)");
            continue;
        }
        using(var conn = new SqliteConnection($"Data Source={p}")){
            conn.Open();
            using(var cmd = conn.CreateCommand()){
                cmd.CommandText = "PRAGMA table_info(Drinks);";
                using(var rdr = cmd.ExecuteReader()){
                    while(rdr.Read()){
                        var cid = rdr.GetInt32(0);
                        var name = rdr.GetString(1);
                        var type = rdr.GetString(2);
                        var notnull = rdr.GetInt32(3);
                        var dflt = rdr.IsDBNull(4)?"NULL":rdr.GetValue(4).ToString();
                        var pk = rdr.GetInt32(5);
                        Console.WriteLine($"  cid={cid}, name={name}, type={type}, notnull={notnull}, dflt_value={dflt}, pk={pk}");
                    }
                }
            }
        }
    }
  }
}
