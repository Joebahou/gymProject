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
    public static class initLIstTrainers
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class Trainer
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        [FunctionName("initLIstTrainers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;

            List<Trainer> trainers = new List<Trainer>();
            if (query == "select_trainers")
            {
                //selecting all the trainers from DB
               
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT idmember,name FROM members where type=1;";
                       
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_member = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                
                                Trainer temp = new Trainer { Name = name, Id = id_member };
                                trainers.Add(temp);
                                //dict_machines[id_machine] = temp;


                            }
                        }





                    }
                }
                return new OkObjectResult(JsonConvert.SerializeObject(trainers));
            }

            else
            {
                
                return new OkObjectResult(0);
            }

        }
    }
}
