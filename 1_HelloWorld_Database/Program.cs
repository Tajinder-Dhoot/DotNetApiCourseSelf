using System.Data;
using System.Globalization;
using _1_HelloWorld_Database.Data;
using _1_HelloWorld_Database.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace _1_HelloWorld_Database;

class Program
{
    static void Main(string[] args)
    {
        DataContextDapper dapper = new();
        DataContextEF entityFramework = new();

        DateTime rightNow = dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");

        Computer myComputer = new()
        {
            Motherboard = "Z609",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 943.87m,
            VideoCard = "RTX 2060"
        };

        // Insert data into table using Dapper
        string sql = @"INSERT INTO TutorialAppSchema.Computer (
                Motherboard,
                HasWifi,
                HasLTE,
                ReleaseDate,
                Price,
                VideoCard
            ) VALUES ('" + myComputer.Motherboard 
                    + "','" + myComputer.HasWifi
                    + "','" + myComputer.HasLTE
                    + "','" + myComputer.ReleaseDate.ToString("yyyy-MM-dd")
                    + "','" + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    + "','" + myComputer.VideoCard
            + "')";

        Console.WriteLine("Insert record using Dapper");
        int result = dapper.ExecuteSqlWithRowCount(sql);
        Console.WriteLine("number of rows affected:" +result);

        // Insert data into table using Entity Framework
        Console.WriteLine("Insert record using Entity Framework");
        entityFramework.Add(myComputer);
        entityFramework.SaveChanges();

        string sqlSelect = @"
            SELECT 
                Computer.ComputerId,
                Computer.Motherboard,
                Computer.HasWifi,
                Computer.HasLTE,
                Computer.ReleaseDate,
                Computer.Price,
                Computer.VideoCard
            FROM TutorialAppSchema.Computer";

        // Get data from table using Dapper
        Console.WriteLine("Get data using Dapper");
        IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);

        Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'" 
            + ",'Price','VideoCard'");

        foreach(Computer singleComputer in computers)
        {
            Console.WriteLine("'" + singleComputer.ComputerId 
                + "','" + singleComputer.Motherboard
                + "','" + singleComputer.HasWifi
                + "','" + singleComputer.HasLTE
                + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
                + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','" + singleComputer.VideoCard + "'");
        }

        // Get data from table using Dapper
        Console.WriteLine("Get data using Entity Framework");
        IEnumerable<Computer>? computersEf = entityFramework.Computer?.ToList<Computer>();

        if(computersEf != null)
        {
            foreach(Computer singleComputer in computersEf)
            {
                Console.WriteLine("'" + singleComputer.ComputerId 
                    + "','" + singleComputer.Motherboard
                    + "','" + singleComputer.HasWifi
                    + "','" + singleComputer.HasLTE
                    + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
                    + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    + "','" + singleComputer.VideoCard + "'");
            }
        }
    }
}
