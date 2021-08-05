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
    public static class selecet_QRscanner
    {

        static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };

        [FunctionName("selecet_QRscanner")]


        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string query = req.Query["query"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            DateTime nearest_schedule = new DateTime(2021, 7, 19, 0, 0, 0);
            int id_member_of_the_nearest_schedule = -1;
            int id_machine_of_member_fromDB = -1;
            int id_member;
            int id_machine;
            DateTime nearest_future_schedule = new DateTime(2024, 7, 19, 0, 0, 0);
            string strdate ;
            DateTime scanning_time;
            switch (query)
            {
                case "select_name_member":
                    id_member = Int32.Parse(req.Query["id_member"]);
                    string result = "";

                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {
                        conn.Open();
                        using (MySqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = @"SELECT name FROM members  WHERE idmember=@id_member;";
                            command.Parameters.AddWithValue("@id_member", id_member);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {

                                    result = reader.GetString(0);

                                }
                            }

                        }


                    }
                    return new OkObjectResult(result);
                
                case "id_machine_of_member_fromDB":
                    id_member = Int32.Parse(req.Query["id_member"]);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {

                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"SELECT idmachine FROM gym_schema.machines WHERE idmember = @id_member;";
                            command.Parameters.AddWithValue("@id_member", id_member);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())

                                {
                                    if (reader != null)
                                        id_machine_of_member_fromDB = reader.GetInt32(0);

                                }
                            }
                        }
                      
                    }
                    return new OkObjectResult(id_machine_of_member_fromDB);
                case "nearest_schedule":
                    id_machine = Int32.Parse(req.Query["id_machine"]);
                    strdate = req.Query["scanning_time"];
                    scanning_time = Convert.ToDateTime(strdate);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {

                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"select MAX(start_time) as nearestSchedule from gym_schema.future_schedule_machines where start_time< @scanning_time and id_machine=@id_machine;";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@scanning_time", scanning_time);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())

                                {
                                    if (!reader.IsDBNull(0))

                                        nearest_schedule = reader.GetDateTime(0);

                                }
                            }

                        }

                    }
                    return new OkObjectResult(nearest_schedule.ToString());
                case "id_member_of_the_nearest_schedule":
                    id_machine = Int32.Parse(req.Query["id_machine"]);
                    strdate = req.Query["nearest_schedule"];
                    scanning_time = Convert.ToDateTime(strdate);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {

                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"select id_member from gym_schema.future_schedule_machines where start_time=@nearest_schedule and id_machine=@id_machine;";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@nearest_schedule", scanning_time);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())

                                {
                                    if (reader != null)
                                        id_member_of_the_nearest_schedule = reader.GetInt32(0);

                                }
                            }

                        }

                    }
                    return new OkObjectResult(id_member_of_the_nearest_schedule);
                case "select_nearest_future_schdule":
                    id_machine = Int32.Parse(req.Query["id_machine"]);
                    id_member = Int32.Parse(req.Query["id_member"]);
                    strdate = req.Query["submiting_time"];
                    DateTime submiting_time = Convert.ToDateTime(strdate);
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {

                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"select min(start_time) as nearestFutureSchedule from gym_schema.future_schedule_machines where start_time> @submit_time and id_machine=@id_machine and id_member!=@id_member;";
                            command.Parameters.AddWithValue("@id_machine", id_machine);
                            command.Parameters.AddWithValue("@id_member", id_member);
                            command.Parameters.AddWithValue("@submit_time", submiting_time);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())

                                {
                                    if (!reader.IsDBNull(0))

                                        nearest_future_schedule = reader.GetDateTime(0);

                                }
                            }

                        }

                    }
                    return new OkObjectResult(nearest_future_schedule.ToString());


                default:
                    return new OkObjectResult(0);
            }
       
        }
    }
}
