using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace exampleApp.Models
{
  
    class Connection
    {  
        public static MySqlConnection conn;
        /*
        public static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };*/

        //make an http request and returns the response as string
        public static string get_result_from_http(string req,bool json)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(req);
            if (json)
            {
                request.ContentType = "application/json; charset=utf-8";
            }
            System.Net.WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
            response.Close();
            reader.Close();
            return result;
        }
        /*
        public static void ConnectDataBase()
        {
            try
            {

                Console.WriteLine("Trying to connect");
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "gymserver.mysql.database.azure.com",
                    Database = "gym_schema",
                    UserID = "gymAdmin",
                    Password = "gym1Admin",
                    SslMode = MySqlSslMode.Required,
                };

                conn = new MySqlConnection(builder.ConnectionString);

                conn.Open();
                Console.WriteLine(conn.State.ToString() + Environment.NewLine);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }*/

    }
}
