using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homePage : ContentPage
    {
        private string name_log;
        public string Name_log
        {
            get { return name_log; }
            set
            {
                name_log = value;
               

            }
        }
        public homePage()
        {
            InitializeComponent();
            Name_log ="Hello "+ Models.User.Name;
            BindingContext = this;

        }
        private void statisticsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatsPage());
        }
        private void scheduleButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AllSchedule());
        }

        private async void machinesButton_Clicked(object sender, EventArgs e)
        {
            UsedMachines.machines_list = new List<Models.Machine>();

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymserver.mysql.database.azure.com",
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
                            if (taken == 0)
                            {
                                temp = new Models.Machine(name, Color.Green, id_machine);

                            }
                            else
                            {
                                temp = new Models.Machine(name, Color.Red, id_machine);
                            }
                            UsedMachines.machines_list.Add(temp);


                        }
                    }


                }
            }

           await  Navigation.PushAsync(new UsedMachines());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new Pages.LoginPage(), this);
            await Navigation.PopAsync();
        }

        
    }
}