using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySqlConnector;
using Xamarin.Essentials;
using System.Linq;

namespace QRscanner
{
    public partial class App : Application
    {
        public static HubConnection connection;
        public static bool finished;
        public static int member_from_table;
        public static int taken;
        public static string name_of_member;
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new welcomePage());
        }

        protected override async void OnStart()
        {
            
            connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<object[]>("newMessage", (msgupdate) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    string member_name = msgupdate[4].ToString();
                    string machine_name =msgupdate[3].ToString() ;
                    String resultusage = "";
                    int usage = Int32.Parse(msgupdate[1].ToString()); 
                    if (usage == 1)
                    {
                        resultusage = member_name + " started using the " + machine_name + " machine";
                        App.member_from_table = Int32.Parse(msgupdate[2].ToString());
                        App.name_of_member = member_name;
                        App.taken = 1;
                        foreach (Models.Machine m in checkboxmachinePage.machines_list)
                        {
                            if (m.Id_machine == Int32.Parse(msgupdate[0].ToString()))
                            {
                                m.Taken = 1;
                                m.Id_member = Int32.Parse(msgupdate[2].ToString());
                                break;
                            }
                        }
                    }

                    else
                    { 
                        resultusage = member_name + " finished using the " + machine_name + " machine";
                        App.taken = 0;
                        foreach (Models.Machine m in checkboxmachinePage.machines_list)
                        {
                            if (m.Id_machine == Int32.Parse(msgupdate[0].ToString()))
                            {
                                m.Taken = 0;
                                m.Id_member = -1;
                                break;
                            }
                        }
                    }
                    await App.Current.MainPage.DisplayAlert("Scanned Barcode", resultusage, "OK");


                });
            });
            connection.On<object[]>("deleteMachine", (result) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    
                    String resultusage = "";

                    resultusage = result[1] + " machine is deleted. please refresh your app";
                    await App.Current.MainPage.DisplayAlert("Scanned Barcode", resultusage, "OK");
                    
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                    
                    /*while (App.Current.MainPage.Navigation.NavigationStack.Last() != App.Current.MainPage)
                    {
                        App.Current.MainPage.Navigation.PopToRootAsync();
                    }*/
                    

                });
            });



            await connection.StartAsync();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
