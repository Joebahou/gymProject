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
    public static class select_used_machines
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class MachineData
        {
            public int Sets { get; set; }
            public int Reps { get; set; }
            public int WeightOrSpeed { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public string Date { get; set; }
        }

        [FunctionName("select_used_machines")]
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

            List<MachineData> usedMachines = new List<MachineData>();
            
            //select all usage rows where idmember and machine name is given
            if (query == "select_used_machines")
            {
                conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();
                int id_member = Int32.Parse(req.Query["id_member"]);
                string machineName = req.Query["machineName"];
                string cmd_text = $"select sets, reps, weight_or_speed, usage_gym.start, usage_gym.end " +
               $"from members, machines, usage_gym " +
               $"where members.idmember = usage_gym.idmember " +
               $"and machines.idmachine = usage_gym.idmachine " +
               $"and members.idmember = {id_member} " +
               $"and machines.name = '{machineName}'" +
               $"order by usage_gym.start";


                MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        int sets = 0;
                        int reps = 0;
                        int speedOrWeight = 0;
                        DateTime dateStart = DateTime.Now;
                        DateTime dateEnd = DateTime.Now;

                        if (rdr[0] != DBNull.Value)
                        {
                            sets = rdr.GetInt32(0);
                        }
                        if (rdr[1] != DBNull.Value)
                        {
                            reps = rdr.GetInt32(1);
                        }
                        if (rdr[2] != DBNull.Value)
                        {
                            speedOrWeight = rdr.GetInt32(2);
                        }
                        if (rdr[3] != DBNull.Value)
                        {
                            dateStart = (DateTime)rdr[3];
                        }
                        if (rdr[4] != DBNull.Value)
                        {
                            dateEnd = (DateTime)rdr[4];
                        }

                        MachineData temp = new MachineData
                        {
                            Sets = sets,
                            Reps = reps,
                            WeightOrSpeed = speedOrWeight,
                            Start = dateStart.ToString("HH:mm:ss"),
                            End = dateEnd.ToString("HH:mm:ss"),
                            Date = dateStart.ToString("MM/dd/yy")
                        };
                        usedMachines.Add(temp);


                    }
                    rdr.Close();
                    conn.Close();
                }
            }
            return new OkObjectResult(JsonConvert.SerializeObject(usedMachines));


        }
            
        }
}
