using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using Microsoft.AspNetCore.SignalR.Client;

using Xamarin.Essentials;
using MySqlConnector;

namespace QRscanner
{
    public partial class MainPage : ContentPage
    {
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        public static HubConnection connection;
        
        public static int id_machine;
        public static int id_member;
        int[] data=new int[5];
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymserver.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public MainPage()
        {
                InitializeComponent();
            /*if (App.finished)
            {
                App.finished = false;

                DisplayAlert("BAD","BADBAD", "OK");
            }*/
            Set_signalR();
        }

        public async void Set_signalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int[]>("newMessage", (msgupdate) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    String resultusage = "";
                    int usage = msgupdate[1];
                    if (usage == 1)
                        resultusage = "id_member = " + msgupdate[2] + " has started using id machine " + msgupdate[0];
                    else
                        resultusage = "id_member = " + msgupdate[2] + " has finished using id machine " + msgupdate[0];
                    await App.Current.MainPage.DisplayAlert("Scanned Barcode", resultusage, "OK");


                });
            });
            await connection.StartAsync();
        }

            private async void scanButton_Clicked(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmember,taken FROM gym_schema.machines WHERE idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            App.member_from_table = reader.GetInt32(0);
                            App.taken = reader.GetInt32(1);
                        }
                    }
                }
            }
            await Navigation.PushAsync(new StartScanPage());
            
        }
    }
}
