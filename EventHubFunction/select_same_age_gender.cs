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
    
    public static class select_same_age_gender
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class UserScore
        {
            public double Score { get; set; }
            public int Id_member { get; set; }
        }

        [FunctionName("select_same_age_gender")]
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

            List<UserScore> UsersScore = new List<UserScore>();
            if (query == "select_users_scores")
            {
                conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();
                string machineName = req.Query["machineName"];
                string gender = req.Query["gender"];
                int age = Int32.Parse(req.Query["age"]);
                string cmd_text = $"select usage_gym.score, members.idmember " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.age = '{age}' " +
                 $"and members.gender = '{gender}' " +
                $"and machines.name = '{machineName}' " +
                $"limit 50";
                MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        double score = rdr.GetDouble(0);
                        int id_member = rdr.GetInt32(1);
                        UsersScore.Add(new UserScore { Score = score, Id_member = id_member });

                    }
                }
                rdr.Close();
                conn.Close();
            }

            return new OkObjectResult(JsonConvert.SerializeObject(UsersScore));

        }
    }
}
