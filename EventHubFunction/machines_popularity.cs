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
    public static class machines_popularity
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        public class popularMachine
        {
            public long Machinenumuses { get; set; }
            public string Machinename { get; set; }

        }
        [FunctionName("machines_popularity")]
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

            List<popularMachine> popularMachines = new List<popularMachine>();
            if (query == "select_popular_machines")
            {
                conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();
                string start_date = req.Query["start_date"];
                string end_date = req.Query["end_date"];
                string cmd_text = $"select machines.name, count(*) " +
                 $"from usage_gym, machines " +
                 $"where machines.idmachine = usage_gym.idmachine " +
                 $"and date(usage_gym.start) >= '{start_date}' " +
                 $"and date(usage_gym.start) <= '{end_date}' " +
                 $"group by usage_gym.idmachine order by count(*) desc ";
                
                MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        string machineName = (string)rdr[0];
                        Console.WriteLine(machineName);
                        long machineNumUses = (long)rdr[1];
                        popularMachines.Add(new popularMachine { Machinename = machineName, Machinenumuses = machineNumUses });
                    }
                }
                rdr.Close();
                conn.Close();
            }

            return new OkObjectResult(JsonConvert.SerializeObject(popularMachines));

        }
    }
}
