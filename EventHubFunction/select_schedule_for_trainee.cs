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
using System.Collections.Generic;

namespace EventHubFunction
{
    public static class select_schedule_for_trainee
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class Future_schedule_trainee
        {
            public string Name_machine { get; set; }
            public int Id_machine { get; set; }
            public DateTime Start_time { get; set; }
           
            public int Available { get; set; }

        }

        [FunctionName("select_schedule_for_trainee")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            DateTime today = DateTime.Now;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);

            List<Future_schedule_trainee> schedule_trainee = new List<Future_schedule_trainee>();
            if (query == "select_schedule_for_trainee")
            {
                int id_member = Int32.Parse(req.Query["id_member"]);
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        //selecting all the bookings for the given member and day
                        command.CommandText = @"SELECT future_schedule_machines.id_machine,future_schedule_machines.start_time,machines.working,machines.name " +
                        "FROM future_schedule_machines,machines " +
                        "WHERE future_schedule_machines.id_machine=machines.idmachine and future_schedule_machines.id_member=@id_member and future_schedule_machines.start_time>=@today " +
                        "order by start_time;";
                        command.Parameters.AddWithValue("@id_member", id_member);
                        command.Parameters.AddWithValue("@today", today);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(0);

                                DateTime start_time = reader.GetDateTime(1);
                                int working = reader.GetInt32(2);
                                string name_machine = reader.GetString(3);
                                Future_schedule_trainee temp = new Future_schedule_trainee { Id_machine = id_machine, Available=working, Name_machine = name_machine, Start_time = start_time };
                                schedule_trainee.Add(temp);

                            }
                        }





                    }
                }
            }

            return new OkObjectResult(JsonConvert.SerializeObject(schedule_trainee));

        }
    }
}
