using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace EventHubFunction
{
    public static class EventHubFunction
    {
        public static async Task<string> updateDB(int id_member,int id_machine,int usage, MySqlConnection conn)
        {
            if (usage == 0)
            {
                using (var command = conn.CreateCommand())
                {

                    command.CommandText = @"INSERT INTO usage_gym (idmember,start,idmachine) VALUES (@id_member,@start, @id_machine);";
                    command.Parameters.AddWithValue("@id_member", id_member);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    DateTime theDate = DateTime.Now;
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
                    command.Parameters.AddWithValue("@end", DateTime.Now);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    
                }
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE machines SET taken = @taken,idmember=@id_member WHERE idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_member", id_member);
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@taken", 1 - usage);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    String temp = String.Format("Number of rows updated={0}", rowCount);
                    return temp;
                }

            }
            
        }
        [FunctionName("EventHubFunction")]
        public static async Task Run([EventHubTrigger("gymeventhub", Connection = "EventHubGym")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymserver.mysql.database.azure.com",
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
                    log.LogInformation(strID[1] +" "+ strID[2]);
                    int id_member = Int32.Parse(strID[1]);
                    int id_machine = Int32.Parse(strID[2]);
                    log.LogInformation($"id_member {id_member}");
                    log.LogInformation($"id_machine { id_machine}");
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
                         log.LogInformation(await updateDB(id_member, id_machine, num_usage,conn));

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
