using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace EventHubFunction
{
    public static class EventHubFunction
    {
        public static async Task<string> updateDB(int id_member,int id_machine,int usage, int weight_or_speed, int reps, int sets, MySqlConnection conn)
        {
            if (usage == 0)
            {
                using (var command = conn.CreateCommand())
                {

                    command.CommandText = @"INSERT INTO usage_gym (idmember,start,idmachine,weight_or_speed,reps,sets) VALUES (@id_member,@start, @id_machine,@weight_or_speed,@reps,@sets);";
                    command.Parameters.AddWithValue("@id_member", id_member);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@weight_or_speed", weight_or_speed);
                    command.Parameters.AddWithValue("@reps", reps);
                    command.Parameters.AddWithValue("@sets", sets);
                    DateTime theDate = DateTime.Now.AddHours(3.0);
                    command.Parameters.AddWithValue("@start", theDate);
                    int rowCount = await command.ExecuteNonQueryAsync();

                   
                    


                }
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE machines SET taken = @taken,idmember=@id_member WHERE idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_member", id_member);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@taken", 1-usage);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    String temp = String.Format("Number of rows updated={0}", rowCount);
                    return temp;
                }

            }
            else
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE usage_gym SET end = @end WHERE idmember = @id_member AND idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_member", id_member);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@end", DateTime.Now.AddHours(3.0));
                    int rowCount = await command.ExecuteNonQueryAsync();
                    
                }
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE machines SET taken = @taken,idmember=@id_member WHERE idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_member", -1);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@taken", 1 - usage);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    String temp = String.Format("Number of rows updated={0}", rowCount);
                    return temp;
                }

            }
            
        }
        [FunctionName("EventHubFunction")]
        public static async Task Run([EventHubTrigger("gymeventhub", Connection = "EventHubGym")] EventData[] events, [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {
            var exceptions = new List<Exception>();
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymservernew.mysql.database.azure.com",
                Database = "gym_schema",
                UserID = "gymAdmin",
                Password = "gym1Admin",
                SslMode = MySqlSslMode.Required,
            };
            foreach (EventData eventData in events)
            {
                try
                {
                   
                    char[] spl = { '[', ',', ']' };
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    string[] strID = messageBody.Split(spl);
                    int len = strID.Length;
                    log.LogInformation(len.ToString());
                    if (strID.Length < 4)
                    {
                        object[] help_msg = new object[2];
                        int id_machine = Int32.Parse(strID[1]);
                        help_msg[0] = id_machine;
                        using (var conn = new MySqlConnection(builder.ConnectionString))
                        {

                            await conn.OpenAsync();

                            using (var command = conn.CreateCommand())
                            {
                                command.CommandText = "SELECT name FROM machines WHERE idmachine=@id_machine";
                                command.Parameters.AddWithValue("@id_machine", id_machine);

                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        help_msg[1] = reader.GetString(0);


                                    }
                                }
                            }
                        }

                        await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "helpMessage",
                            Arguments = new[] { help_msg }
                        });

                    }
                    else
                    {
                        if (strID.Length <= 6)
                        {
                            object[] broken_msg = new object[2];
                            int id_machine = Int32.Parse(strID[1]);
                            broken_msg[0] = id_machine;
                            
                                //extracting from table the machine name
                            using (var conn = new MySqlConnection(builder.ConnectionString))
                            {

                                await conn.OpenAsync();

                                using (var command = conn.CreateCommand())
                                {
                                    command.CommandText = "SELECT name FROM machines WHERE idmachine=@id_machine";
                                    command.Parameters.AddWithValue("@id_machine", id_machine);

                                    using (var reader = await command.ExecuteReaderAsync())
                                    {
                                        while (await reader.ReadAsync())
                                        {
                                            broken_msg[1] = reader.GetString(0);


                                        }
                                    }
                                }
                            }

                            //checking if its an notifcation or set as not working by owner
                            int filter = Int32.Parse(strID[2]);
                            if (filter == 0)
                            {

                                await signalRMessages.AddAsync(
                                new SignalRMessage
                                {
                                    Target = "BrokenMachine_alert",
                                    Arguments = new[] { broken_msg }
                                });
                            }
                            else
                            {
                                if(filter==1)
                                {
                                    await signalRMessages.AddAsync(
                                    new SignalRMessage
                                    {
                                        Target = "BrokenMachine_real",
                                        Arguments = new[] { broken_msg }
                                    });
                                }
                                else
                                {
                                        await signalRMessages.AddAsync(
                                       new SignalRMessage
                                       {
                                           Target = "BrokenMachine_fixed",
                                           Arguments = new[] { broken_msg }
                                       });

                                }
                            }

                        }
                        else
                        {
                            log.LogInformation(strID[1] + " its an usage data not help " + strID[2]);
                            int id_member = Int32.Parse(strID[1]);
                            int id_machine = Int32.Parse(strID[2]);
                            int weight_or_speed = Int32.Parse(strID[3]);
                            int reps = Int32.Parse(strID[4]);
                            int sets = Int32.Parse(strID[5]);
                            int num_usage = 0;
                            using (var conn = new MySqlConnection(builder.ConnectionString))
                            {

                                await conn.OpenAsync();

                                using (var command = conn.CreateCommand())
                                {
                                    command.CommandText = "SELECT * FROM machines WHERE idmachine=@id_machine";
                                    command.Parameters.AddWithValue("@id_machine", id_machine);

                                    using (var reader = await command.ExecuteReaderAsync())
                                    {
                                        while (await reader.ReadAsync())
                                        {
                                            num_usage = reader.GetInt32(2);


                                        }
                                    }
                                }
                                log.LogInformation(await updateDB(id_member, id_machine, num_usage, weight_or_speed, reps, sets, conn));

                            }

                            int[] msgupdate = new int[3];
                            msgupdate[0] = id_machine;
                            msgupdate[1] = 1 - num_usage;
                            msgupdate[2] = id_member;

                            await signalRMessages.AddAsync(
                            new SignalRMessage
                            {
                                Target = "newMessage",
                                Arguments = new[] { msgupdate }
                            });
                        }
                    }



                    // Replace these two lines with your processing logic.

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
