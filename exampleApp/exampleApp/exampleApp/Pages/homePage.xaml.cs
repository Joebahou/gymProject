using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homePage : ContentPage
    {
        public static HubConnection connection;
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
            /*if (Models.User.Type == 1)
            {
                Set_signalR();
            }*/

        }
        public async void Set_signalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                
            };

            
            connection.On<int[]>("helpMessage", (help_msg) =>
            {
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {

                    String resultusage = "";
                    int helpM = help_msg[0];
                    resultusage = "someone is asking for help in machine id " + helpM;
                    await App.Current.MainPage.DisplayAlert("HElP ME", resultusage, "OK");

                    });
                
            });
            await connection.StartAsync();
        }
        private void statisticsButton_Clicked(object sender, EventArgs e)
        {
            if(Models.User.Type == 0) { Navigation.PushAsync(new StatisticsNavigationTrainee()); }
            if (Models.User.Type == 1) { Navigation.PushAsync(new TrainerStatistics.StatisticsNavigationTrainer()); }
            if (Models.User.Type == 2) { Navigation.PushAsync(new OwnerStatistics.StatisticsNavigationOwner()); }

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
            await connection.StopAsync();
            Navigation.InsertPageBefore(new Pages.LoginPage(), this);
            await Navigation.PopAsync();
            
        }

        
    }
}