﻿using MySqlConnector;
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
        public   HubConnection connection;
        private string name_log;
        public string Name_log
        {
            get { return name_log; }
            set
            {
                name_log = value;
               

            }
        }
        public Boolean IsOwner { get; set; }
        public homePage()
        {
            InitializeComponent();
            Name_log ="Hello "+ Models.User.Name;
            
            if (Models.User.Type == 1)
            {
                Set_signalR_to_trainer();
            }
            if (Models.User.Type == 2)
            {
                Set_signalR_to_owner();
                IsOwner = true;
                
            }
            BindingContext = this;
        }
        public async void Set_signalR_to_trainer()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                
            };

            
            this.connection.On<int[]>("helpMessage", (help_msg) =>
            {
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {

                        if (Models.User.Type == 1)
                        {
                            String resultusage = "";
                            int helpM = help_msg[0];
                            resultusage = "someone is asking for help in machine id " + helpM;
                            await App.Current.MainPage.DisplayAlert("HElP ME", resultusage, "OK");
                        }
                        

                    });
                
            });
            await this.connection.StartAsync();
        }
        public async void Set_signalR_to_owner()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);

            };


            this.connection.On<int[]>("BrokenMachine", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    if (Models.User.Type == 2)
                    {
                        String resultusage = "";
                        
                        resultusage = "id machine "+ broken_msg[0]+" isn't working";
                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                    }


                });

            });
            await this.connection.StartAsync();
        }
        private void statisticsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatsPage());
        }
        private void scheduleButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AllSchedule());
        }
        private void schedule_for_trainerButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Schedule_for_trainer());
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
                    command.CommandText = @"SELECT idmachine,taken,name,working FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            int taken = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            int available= reader.GetInt32(3);
                            Models.Machine temp;
                            if (available == 1)
                            {
                                if (taken == 0)
                                {
                                    temp = new Models.Machine(name, Color.Green, id_machine);

                                }
                                else
                                {
                                    temp = new Models.Machine(name, Color.Red, id_machine);
                                }
                                
                            }
                            else
                            {
                                temp = new Models.Machine(name, Color.Yellow, id_machine);
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
            if (Models.User.Type > 0 && connection.State==HubConnectionState.Connected)
            {
                await this.connection.StopAsync();
            }
            else
            {
                this.connection = null;
            }

            App.Current.MainPage = new NavigationPage(new Pages.LoginPage());
            await App.Current.MainPage.Navigation.PopAsync();
            
        }
        private async void editMachineButton_Clicked(object sender, EventArgs e)
        {
            availableMachines_owner.machines_list = new List<Models.Machine>();

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
                    command.CommandText = @"SELECT idmachine,name,working FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int available = reader.GetInt32(2);
                            Models.Machine temp = new Models.Machine(id_machine,name,available);
                            availableMachines_owner.machines_list.Add(temp);
                        }
                    }


                }
            }
            // need to push a new page for the owner only
            //await Navigation.PushAsync(new UsedMachines());
        }


    }

    
}