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
   
    public static class initListMachines
    {
        
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class Machine
        {
            public string Name { get; set; }
            public int Available { get; set; }
            public int Id_machine { get; set; }
            public int Taken { get; set; }
            public int Id_member {get; set;}
            public int Alert_broken { get; set; }
           
        }

        [FunctionName("initListMachines")]

        //selecting all the machines from DB
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            
            List<Machine> machines = new List<Machine>();
            if (query == "select_machines")
            {
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT * FROM machines;";
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                int taken = reader.GetInt32(2);
                                int id_member = reader.GetInt32(3);
                                int working = reader.GetInt32(4);
                                int alert_broken = reader.GetInt32(5);
                                Machine temp = new Machine {Name=name,Id_machine=id_machine,Available=working,Id_member=id_member,Taken=taken,Alert_broken=alert_broken };
                                machines.Add(temp);
                                //dict_machines[id_machine] = temp;


                            }
                        }





                    }
                }
            }

            return new OkObjectResult(JsonConvert.SerializeObject(machines));
                
        }
    }
}
