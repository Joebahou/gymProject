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
    public static class select_all_schedule
    {

        public class Future_schedule
        {
            public string Name_member { get; set; }
            public int Id_machine { get; set; }
            public DateTime Start_time { get; set; }
            public int Id_member { get; set; }
           

        }

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        [FunctionName("select_all_schedule")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;

            List<Future_schedule> schedules = new List<Future_schedule>();
            if (query == "select_schedule_for_date")
            {
                DateTime start_time = Convert.ToDateTime(req.Query["start_time"]);
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT * FROM future_schedule_machines WHERE start_time>=@start_time AND start_time<=@tomorrow;";
                        command.Parameters.AddWithValue("@start_time", start_time);
                        command.Parameters.AddWithValue("@tomorrow", start_time.AddDays(1.0));

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(1);
                                int id_member = reader.GetInt32(2);
                                string name_member = reader.GetString(3);
                                DateTime time = reader.GetDateTime(4);
                                Future_schedule temp = new Future_schedule {Id_machine=id_machine,Id_member=id_member, Name_member=name_member,Start_time=time};
                                schedules.Add(temp);
                                


                            }
                        }





                    }
                }
            }

            return new OkObjectResult(JsonConvert.SerializeObject(schedules));

        }
    }
}
