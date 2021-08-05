using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QRscanner.Models
{
  
    class Connection
    {  
        
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
        

    }
}
