using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace testingMySQL
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymserver.mysql.database.azure.com",
                Database = "gym_schema",
                UserID = "gymAdmin@gymserver",
                Password = "gym1Admin",
                SslMode = MySqlSslMode.Required,
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM members;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(string.Format(
                                "Reading from table=({0}, {1}, {2})",
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)));
                        }
                    }
                }

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE members SET age = @age WHERE name = @name;";
                    command.Parameters.AddWithValue("@age", 50);
                    command.Parameters.AddWithValue("@name", "fani");

                    int rowCount = await command.ExecuteNonQueryAsync();
                    Console.WriteLine(String.Format("Number of rows updated={0}", rowCount));
                }


                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM members;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(string.Format(
                                "Reading from table=({0}, {1}, {2})",
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }


            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();
        }
    }
}

