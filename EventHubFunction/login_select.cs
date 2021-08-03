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
    public static class login_select
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public class User
        {
            public int Id_member { get; set; }
            public int Age { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public int Type { get; set; }


        }
        public class Trainee
        {
            public int Id_member { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public int Age { get; set; }



        }

        [FunctionName("login_select")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;

            if (query == "check_login")
            {

                User user=new User { Id_member=-1};
                string email = req.Query["email"];
                string password = req.Query["password"];
                

                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT email,password,idmember,name,type,age,gender FROM gym_schema.members;";
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string email_DB = reader.GetString(0);
                                string password_DB = reader.GetString(1);
                                if (email == email_DB && password == password_DB)
                                {

                                    int id_member_login = reader.GetInt32(2);
                                    string name_login = reader.GetString(3);
                                    int Ttype = reader.GetInt32(4);
                                    //need to save info on app
                                    user = new User
                                    {
                                        Name = name_login,
                                        Id_member = id_member_login,
                                        Type = Ttype,
                                        Age = reader.GetInt32(5),
                                        Gender = reader.GetString(6),
                                    };
                                }


                            }
                        }



                    }

                }
                return new OkObjectResult(JsonConvert.SerializeObject(user));


            }
            else
            {
                if (query == "select_trainees_for_trainer")
                {
                    List<Trainee> Trainees = new List<Trainee>();
                    
                    int id_member = Int32.Parse(req.Query["id_member"]);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {
                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"SELECT idmember,name,gender,age FROM members WHERE trainer=@id_member;";
                            command.Parameters.AddWithValue("@id_member", id_member);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    int id_trainee = reader.GetInt32(0);
                                    string name = reader.GetString(1);
                                    string gender = reader.GetString(2);
                                    int age = reader.GetInt32(3);
                                    Trainee temp = new Trainee { Id_member = id_trainee, Name = name,Gender=gender,Age=age };
                                    Trainees.Add(temp);


                                }
                            }



                        }
                    }
                    return new OkObjectResult(JsonConvert.SerializeObject(Trainees));
                }
                return new OkObjectResult(0);
            }

        }
    }
}
