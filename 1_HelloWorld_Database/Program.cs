using System.Data;
using System.Globalization;
using _1_HelloWorld_Database.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace _1_HelloWorld_Database;

class Program
{
    static void Main(string[] args)
    {

        string connectionString = "Server=localhost;Database=DotNetCourseDatabase;Trusted_Connection=false;TrustServerCertificate=True;User Id=;Password=;";

        IDbConnection dbConnection = new SqlConnection(connectionString);

        string sqlCommand = "SELECT GETDATE()";

        DateTime rightNow = dbConnection.QuerySingle<DateTime>(sqlCommand);

        Console.WriteLine(rightNow);

        Computer myComputer = new()
        {
            Motherboard = "Z609",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 943.87m,
            VideoCard = "RTX 2060"
        };

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

        IEnumerable<Computer> computers = dbConnection.Query<Computer>(sqlSelect);

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
    }
}
