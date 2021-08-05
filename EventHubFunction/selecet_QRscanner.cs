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
    public static class selecet_QRscanner
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("selecet_QRscanner")]


        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;


            if (query == "select_name_member")
            {

                int id_member = Int32.Parse(req.Query["id_member"]);
                string result = "";

                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT name FROM members  WHERE idmember=@id_member;";
                        command.Parameters.AddWithValue("@id_member", id_member);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                result = reader.GetString(0);

                            }
                        }

                    }


                }
                return new OkObjectResult(result);

            }
        
            else
            {
                if (query == "ready_to_choose_trainee")
                {
                    bool ready_to_choose_trainee = true;
                    int id_machine = Int32.Parse(req.Query["id_machine"]);
                    string strdate = req.Query["time_to_schedule"];
                    DateTime time_to_schedule = Convert.ToDateTime(strdate);

                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {

                        conn.Open();
                        using (MySqlCommand command = conn.CreateCommand())
                        {

                            command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_machine=@id_machine and start_time=@time_to_schedule;";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@time_to_schedule", time_to_schedule);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    ready_to_choose_trainee = false;
                                }
                            }


                        }
                    }
                    return new OkObjectResult(ready_to_choose_trainee);
                }
                return new OkObjectResult(0);
            }






        }
    }
}
