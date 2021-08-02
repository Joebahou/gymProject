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
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace EventHubFunction
{
    public static class delete_sql
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("delete_sql")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            int rowCount = 0;
            if (query == "delete_machine")
            {
                int id_machine = Int32.Parse(req.Query["id_machine"]);
                string machine_name = req.Query["machine_name"];
                Object[] result = new object[2];
                result[0] = id_machine;
                result[1] = machine_name;
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM future_schedule_machines WHERE id_machine=@id_machine;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.ExecuteNonQuery();
                    }
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM usage_gym WHERE idmachine=@id_machine;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.ExecuteNonQuery();
                    }
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM machines WHERE idmachine=@id_machine;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        rowCount =command.ExecuteNonQuery();
                    }
                }
                
                await signalRMessages.AddAsync(
                            new SignalRMessage
                            {
                                Target = "deleteMachine",
                                Arguments = new[] { result }
                            });


            }
            else
            {
                if (query == "delete_schedule")
                {
                    int id_machine = Int32.Parse(req.Query["id_machine"]);
                    int id_member = Int32.Parse(req.Query["id_member"]);
                    string strdate = req.Query["start_time"];
                    DateTime start_time = Convert.ToDateTime(strdate);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {
                        conn.Open();
                        using (MySqlCommand command = conn.CreateCommand())
                        {

                            command.CommandText = @"DELETE FROM future_schedule_machines WHERE id_machine=@id_machine and id_member=@id_member and start_time=@start_time;";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@id_member", id_member);
                            command.Parameters.AddWithValue("@start_time", start_time);
                            rowCount= command.ExecuteNonQuery();


                        }
                    }
                }

            }
            
            string responseMessage = rowCount.ToString();
            return new OkObjectResult(responseMessage);
        }
    }
}
