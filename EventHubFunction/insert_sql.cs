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

namespace EventHubFunction
{
    public static class insert_sql
    {
        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("insert_sql")]



        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            int rowCount = 0;
            if (query == "insert_new_schedule")
            {
                int id_machine = Int32.Parse(req.Query["id_machine"]);
                int id_Trainee = Int32.Parse(req.Query["id_Trainee"]);
                string strdate = req.Query["time_to_schedule"];
                DateTime time_to_schedule = Convert.ToDateTime(strdate);
                string name_trainee = req.Query["name_trainee"];
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {


                        command.CommandText = @"INSERT INTO future_schedule_machines(id_machine,id_member,start_time,name_member) VALUES(@id_machine,@id_member,@start_time,@name_member);";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.Parameters.AddWithValue("@id_member", id_Trainee);
                        command.Parameters.AddWithValue("@start_time", time_to_schedule);
                        command.Parameters.AddWithValue("@name_member", name_trainee);
                        rowCount = command.ExecuteNonQuery();



                    }
                }
            }
            
            string responseMessage = rowCount.ToString();


            return new OkObjectResult(responseMessage);
        }
    }
}
