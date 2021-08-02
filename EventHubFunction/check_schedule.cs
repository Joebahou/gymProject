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
using System.Net.Http;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace EventHubFunction
{
    public static class check_schedule
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class Result
        {
            public bool isTrue { get; set; }
        }

        [FunctionName("check_schedule")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            Result firstcheck =new Result();
            firstcheck.isTrue = true;
            Result secondcheck = new Result();
            secondcheck.isTrue = false;
            Result[] results = { firstcheck, secondcheck };

            if (query == "check_schedule_for_trainee")
            {
                int id_machine = Int32.Parse(req.Query["id_machine"]);
                int id_member = Int32.Parse(req.Query["id_member"]);
                string strdate = req.Query["time_to_schedule"];
                DateTime time_to_schedule = Convert.ToDateTime(strdate);
                
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        
                        command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_member=@id_member and start_time=@time_to_schedule;";
                        command.Parameters.AddWithValue("@id_member", id_member);
                        command.Parameters.AddWithValue("@time_to_schedule", time_to_schedule);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                firstcheck.isTrue = false;
                            }
                        }


                    }
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_machine=@id_machine and start_time=@time_to_schedule;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.Parameters.AddWithValue("@time_to_schedule", time_to_schedule);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                secondcheck.isTrue = true;
                            }
                        }


                    }

                }
                return new OkObjectResult(JsonConvert.SerializeObject
                (new
                {
                    results = results

                }
                 )
            ); ;
            }
            else
            {
                if(query== "ready_to_choose_trainee")
                {
                    bool ready_to_choose_trainee=true;
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
