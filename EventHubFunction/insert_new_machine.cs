using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MySqlConnector;

namespace EventHubFunction
{
    public static class insert_new_machine
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("insert_new_machine")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            int rowCount = 0;
            int new_id=-1;

            if (query == "insert_new_machine")
            {
                string machine_name = req.Query["machine_name"];
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO machines(name) VALUES (@name);";
                        command.Parameters.AddWithValue("@name", machine_name);
                        rowCount = command.ExecuteNonQuery();
                    }
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT idmachine FROM gym_schema.machines WHERE name=@name;";
                        command.Parameters.AddWithValue("@name", machine_name);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())

                            {
                                new_id = reader.GetInt32(0);
                            }
                        }
                    }
                }

            }



            string responseMessage = new_id.ToString();


            return new OkObjectResult(responseMessage);
        }

    }
}
