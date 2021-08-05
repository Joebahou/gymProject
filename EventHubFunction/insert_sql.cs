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
                bool machine_exists = false;
                string responseMessage = "";
                string name_trainee = req.Query["name_trainee"];
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM gym_schema.machines WHERE idmachine=@idmachine;";
                        command.Parameters.AddWithValue("@idmachine", id_machine);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                machine_exists = true;
                            }
                        }
                    }
                    if (machine_exists)
                    {
                        using (MySqlCommand command = conn.CreateCommand())
                        {


                            command.CommandText = @"INSERT INTO future_schedule_machines(id_machine,id_member,start_time,name_member) VALUES(@id_machine,@id_member,@start_time,@name_member);";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@id_member", id_Trainee);
                            command.Parameters.AddWithValue("@start_time", time_to_schedule);
                            command.Parameters.AddWithValue("@name_member", name_trainee);
                            rowCount = command.ExecuteNonQuery();
                            responseMessage = rowCount.ToString();



                        }
                    }
                    else
                    {
                        responseMessage = "machine_not_exists";

                    }
                 
                }
               


                return new OkObjectResult(responseMessage);
            }
            if(query == "insert_new_user")
            {
                int id = Int32.Parse(req.Query["id"]);
                string name = req.Query["name"];
                int age= Int32.Parse(req.Query["age"]);
                string username = req.Query["username"];
                string password = req.Query["password"];
                string gender = req.Query["gender"];
                int type = Int32.Parse(req.Query["type"]);
                int trainer = Int32.Parse(req.Query["trainer"]);
                bool isduplicate=false;
                log.LogInformation($"C# Event Hub trigger function processed a message: {id} {name} {age} {username} {password} {gender} {type} {trainer}");

                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM members WHERE idmember=@id;";
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())

                            {
                                if (!reader.IsDBNull(0))

                                    isduplicate = true;

                            }
                        }
                    }
                    if (!isduplicate)
                    {
                        using (MySqlCommand command = conn.CreateCommand())
                        {


                            command.CommandText = @"INSERT INTO members(idmember,name,age,username,password,gender,type,trainer) VALUES(@idmember,@name,@age,@username,@password,@gender,@type,@trainer);";
                            command.Parameters.AddWithValue("@idmember", id);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@age", age);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", password);
                            command.Parameters.AddWithValue("@gender", gender);
                            command.Parameters.AddWithValue("@type", type);
                            command.Parameters.AddWithValue("@trainer", trainer);
                            rowCount = command.ExecuteNonQuery();



                        }

                        return new OkObjectResult(id);
                    }
                    else return new OkObjectResult("isDuplicate");
                }

            }
            return new OkObjectResult(-1);


        }
    }
}
