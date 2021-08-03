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
    public static class machine_used_by_trainee
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class machineProgress
        {
            public double Score { get; set; }
            public DateTime Start { get; set; }

        }

        [FunctionName("machine_used_by_trainee")]
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

            if (query == "machine_used_by_trainee")
            {
                List<string> MachinesName = new List<string>();
                conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();
                int id_member = Int32.Parse(req.Query["id_member"]);
                string cmd_text = $"select distinct machines.name " +
               $"from members, machines, usage_gym " +
               $"where members.idmember = usage_gym.idmember " +
               $"and machines.idmachine = usage_gym.idmachine " +
               $"and members.idmember = {id_member} ";
                
                MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        if (rdr[0] != DBNull.Value)
                        {
                            MachinesName.Add(rdr[0].ToString());
                        }
                    }
                }
                rdr.Close();
                conn.Close();

                return new OkObjectResult(JsonConvert.SerializeObject(MachinesName));
            }
            else
            {
                if (query == "machine_progress")
                {
                    List<machineProgress> progressedMachines = new List<machineProgress>();
                    conn = new MySqlConnection(builder.ConnectionString);
                    conn.Open();
                    int id_member = Int32.Parse(req.Query["id_member"]);
                    string machineName = req.Query["machineName"];
                    string cmd_text = $"select score, usage_gym.start " +
                       $"from members, machines, usage_gym " +
                       $"where members.idmember = usage_gym.idmember " +
                       $"and machines.idmachine = usage_gym.idmachine " +
                       $"and members.idmember = {id_member} " +
                       $"and machines.name = '{machineName}' " +
                       $"limit 20";
                    MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            double score = -1;
                            DateTime date = DateTime.Now;
                            if (rdr[0] != DBNull.Value)
                            {
                                score = rdr.GetDouble(0);
                            }
                            if (rdr[1] != DBNull.Value)
                            {
                                date = (DateTime)rdr[1];
                            }


                            progressedMachines.Add(new machineProgress { Score = score, Start = date });

                        }
                    }
                    rdr.Close();
                    conn.Close();

                    return new OkObjectResult(JsonConvert.SerializeObject(progressedMachines));
                }
                return new OkObjectResult(0);
            }
          

        }
    }
}
