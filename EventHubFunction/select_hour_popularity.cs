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
    public static class select_hour_popularity
    {

        
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        
        public class popularHour{
            public long Count { get; set; }
            public string Date { get; set; }

        }
        [FunctionName("select_hour_popularity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            MySqlConnection conn;
            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            
            List<popularHour> popularHours = new List<popularHour>();
            if (query == "select_popular_hours")
            {
                conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();
                string start_date = req.Query["start_date"];
                string end_date = req.Query["end_date"];
                string cmd_text = $"SELECT hour(usage_gym.start) as h, count(*) " +
               $"from usage_gym " +
               $"where date(usage_gym.start) >= '{start_date}' " +
               $"and date(usage_gym.start) <= '{end_date}' " +
               $"group by h ";
                
                MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        long count = -1;
                        string date = "";
                        if (rdr[0] != DBNull.Value)
                        {
                            date = rdr.GetInt32(0).ToString();
                        }
                        else { continue; }
                        if (rdr[1] != DBNull.Value)
                        {
                            count = rdr.GetInt32(1);
                        }
                        else { continue; }
                        popularHours.Add(new popularHour { Date = date, Count = count });

                    }
                }
                rdr.Close();
                conn.Close();
            }

            return new OkObjectResult(JsonConvert.SerializeObject(popularHours));

        }
    }
}
