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
        }

        private async void availablemachinesButton_Clicked(object sender, EventArgs e)
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

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,taken,name FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            int taken = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            Models.Machine temp;
                            temp = new Models.Machine(name, id_machine);
                            checkboxmachinePage.machines_list.Add(temp);


                        }
                    }

                }
            }
            await Navigation.PushAsync(new checkboxmachinePage());
        }
    }
}