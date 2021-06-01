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
        ZXingScannerPage scanPage;
        public static int id_machine = 7;
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
                ButtonScanDefault.Clicked += ButtonScanDefault_Clicked;
                connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
                connection.Closed += async(error) =>
                 {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
                   };

                connection.On<int[]>("newMessage", (msgupdate) =>
                {
                MainThread.BeginInvokeOnMainThread(async () =>
                    {

                        String resultusage="";
                        int usage = msgupdate[1];
                        if(usage==1)
                            resultusage = "id_member = "+msgupdate[2]+" has started using id machine "+msgupdate[0];
                        else
                            resultusage = "id_member = " + msgupdate[2] + " has finished using id machine " + msgupdate[0];
                        await App.Current.MainPage.DisplayAlert("Scanned Barcode", resultusage , "OK");


                    });
                });
        }

        private async void ButtonScanDefault_Clicked(object sender, EventArgs e)
        {
            String caching_msg = "";
            scanPage = new ZXingScannerPage();
            
            scanPage.OnScanResult += async (result) =>
            {
                String res = result.Text; 
                scanPage.IsScanning = false;
                //Do something with result
                id_member = int.Parse(res);
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
                                int member_from_table = reader.GetInt32(0);
                                int taken = reader.GetInt32(1);
                                if (taken==1)
                                {
                                    if (member_from_table == id_member)
                                    {
                                        //user is now finishing using the machine. do not enter the usagepage
                                        data[0] = id_member;
                                        data[1] = id_machine;
                                        data[2] = 0;
                                        data[3] = 0;
                                        data[4] = 0;
                                        caching_msg = "id_member = " + id_member + " has finished using id machine " + id_machine;
                                        Device.BeginInvokeOnMainThread(async () =>
                                        {

                                            string messageJson = JsonConvert.SerializeObject(data);
                                            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                                            await Client.SendEventAsync(message);                                            
                                            
                                           
                                        });
                                        

                                    }
                                    else
                                    {
                                        //somebody alse is using the machine,mabey pop a msg 
                                        Device.BeginInvokeOnMainThread(async () =>
                                        {
                                            caching_msg = "id_member = " + member_from_table + " is using id machine " + id_machine;
                                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", caching_msg, "OK");
                                        });
                                    }
                                }
                                else
                                {
                                    //the machine is free to use
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        /*await Navigation.PopModalAsync();

                                        await DisplayAlert("Scanned Barcode", res, "OK");*/
                                        Application.Current.MainPage = new NavigationPage(new InfoUsage());
                                        await App.Current.MainPage.Navigation.PopAsync();
                                    });

                                }

                            }
                        }

                    }
                }
                

                //need to add check if the machine is taken
                //need to add navigation to submit button
                //what happens when finish usage

               
                
                /*data[0]=id_member;
                data[1] = id_machine;
                
                string messageJson = JsonConvert.SerializeObject(data);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);*/
                

            };

            await Navigation.PushModalAsync(scanPage);
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
            await App.Current.MainPage.Navigation.PopAsync();
        }
    }
}
