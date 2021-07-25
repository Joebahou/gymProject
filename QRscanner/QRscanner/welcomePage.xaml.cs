using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class welcomePage : ContentPage
    {
       
        
        
        public welcomePage()
        {

            
            InitializeComponent();
           
            //initwelcome();
        }
       
        /* private  void initwelcome()
        {
            checkboxmachinePage.machines_list = new List<Models.Machine>();
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymservernew.mysql.database.azure.com",
                Database = "gym_schema",
                UserID = "gymAdmin",
                Password = "gym1Admin",
                SslMode = MySqlSslMode.Required,
            };

            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,working,name FROM gym_schema.machines;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id_machine = reader.GetInt32(0);
                            int available = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            Models.Machine temp;
                            temp = new Models.Machine(name, id_machine,available);
                            checkboxmachinePage.machines_list.Add(temp);
                            

                        }
                    }

                }
            }
        }*/
        private async void availablemachinesButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            activityIndicatorLabel.IsVisible = true;
            await Task.Delay(20);
            checkboxmachinePage.machines_list = new List<Models.Machine>();
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymservernew.mysql.database.azure.com",
                Database = "gym_schema",
                UserID = "gymAdmin",
                Password = "gym1Admin",
                SslMode = MySqlSslMode.Required,
            };

            using (MySqlConnection conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,working,name FROM gym_schema.machines;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id_machine = reader.GetInt32(0);
                            int available = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            Models.Machine temp;
                            temp = new Models.Machine(name, id_machine, available);
                            checkboxmachinePage.machines_list.Add(temp);


                        }
                    }

                }
            }
          
            await Navigation.PushAsync(new checkboxmachinePage());
            activityIndicator.IsVisible = false;
            activityIndicatorLabel.IsVisible = false;
        }
    }
}