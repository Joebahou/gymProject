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
    public static class update_schedule_of_trainer
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("update_schedule_of_trainer")]
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
            Boolean ready_to_add = true;
            string responseMessage="";

            if (query == "update_new_schedule")
            {
                
                int id_machine = Int32.Parse(req.Query["id_machine"]);
                int new_id_member = Int32.Parse(req.Query["new_id_member"]);
                int old_id_member = Int32.Parse(req.Query["old_id_member"]);
                DateTime start_time = Convert.ToDateTime(req.Query["start_time"]);
                string new_name_member = req.Query["new_name_member"];
                bool machine_exists = false;
                //chcking if a trainee can be selected for another future schedule
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM gym_schema.machines WHERE idmachine=@idmachine;";
                        command.Parameters.AddWithValue("@idmachine", id_machine);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                machine_exists = true;
                            }
                        }
                    }
                    if (machine_exists)
                    {
                        using (MySqlCommand command = conn.CreateCommand())
                        {

                            command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_member=@id_member and start_time=@time_to_schedule;";
                            command.Parameters.AddWithValue("@id_member", new_id_member);
                            command.Parameters.AddWithValue("@time_to_schedule", start_time);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    ready_to_add = false;
                                }
                            }


                        }
                        if (ready_to_add)
                        {
                            //updating DB
                            using (MySqlCommand command = conn.CreateCommand())
                            {

                                command.CommandText = @"UPDATE future_schedule_machines SET id_member=@new_id_member,name_member=@new_name_member WHERE id_member=@old_id_member and id_machine=@id_machine and start_time=@start_time;";
                                command.Parameters.AddWithValue("@id_machine", id_machine);
                                command.Parameters.AddWithValue("@new_id_member", new_id_member);
                                command.Parameters.AddWithValue("@old_id_member", old_id_member);
                                command.Parameters.AddWithValue("@start_time", start_time);
                                command.Parameters.AddWithValue("@new_name_member", new_name_member);
                                rowCount = command.ExecuteNonQuery();
                                responseMessage = rowCount.ToString();


                            }
                        }
                        else
                        {
                            responseMessage = "trainee is not free";
                        }
                    }
                    else
                    {
                        responseMessage = "machine_not_exists";
                    }
                   
                }

            }


            return new OkObjectResult(responseMessage);
        }
    }
}
