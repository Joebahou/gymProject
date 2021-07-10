using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using ZXing.Net.Mobile.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using MySqlConnector;
namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartScanPage : ContentPage
    {
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        public static HubConnection connection;
        ZXingScannerPage scanPage;
        public static int id_member;
        int[] dataUsage = new int[5];
        //id machine of the member who has been scanned, if none eaquals to -1
        int id_machine_of_member=-1;

        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymserver.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public  StartScanPage()
        {
            InitializeComponent();
            String caching_msg = "";
            dataUsage[1] = MainPage.id_machine;
            dataUsage[2] = 0;
            dataUsage[3] = 0;
            dataUsage[4] = 0;
           
            scanPage = new ZXingScannerPage();
            
            scanPage.OnScanResult += async (result) =>
            {
                String res = result.Text;
                scanPage.IsScanning = false;
                //Do something with result
                id_member = int.Parse(res);
                if (App.taken == 1)
                {
                   
                    if (App.member_from_table == id_member)
                    {
                        //user is now finishing using the machine. do not enter the usagepage
                        dataUsage[0] = id_member;
                        //caching_msg = "id_member = " + id_member + " has finished using id machine " + id_machine;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            App.finished = true;
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            await Navigation.PopAsync();
                            string messageJson = JsonConvert.SerializeObject(dataUsage);
                            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                            await Client.SendEventAsync(message);

                        });

                    }
                    else
                    {
                        //somebody alse is using the machine,mabey pop a msg 
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //THE MSG ISNT SHOWNG ON THE SCREEN, PLS FIX
                            caching_msg = "id_member = " + App.member_from_table + " is using id machine " + MainPage.id_machine;
                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", caching_msg, "OK");
                        });
                    }
                }
                else
                {
                    using (var conn = new MySqlConnection(builder.ConnectionString))
                    {
                        conn.Open();
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"SELECT idmachine FROM gym_schema.machines WHERE idmember = @id_member;";
                            command.Parameters.AddWithValue("@id_member", id_member);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())

                                {
                                    if (reader != null)
                                        id_machine_of_member = reader.GetInt32(0);

                                }
                            }

                        }

                    }
                    if (id_machine_of_member == -1)
                    {
                        //the machine is free to use. need to cheack that member is not using other machine at the same time
                        Device.BeginInvokeOnMainThread(async () =>
                        {

                            await App.Current.MainPage.Navigation.PopModalAsync();
                        //await App.Current.MainPage.Navigation.PopAsync();
                        await App.Current.MainPage.Navigation.PushAsync(new InfoUsage());


                        });
                    }
                    else
                    {
                        //member is trying to use 2 machines at the same time
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //THE MSG ISNT SHOWNG ON THE SCREEN, PLS FIX
                            caching_msg = "you can't use more than one machine at the same time!";
                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", caching_msg, "OK");
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            await App.Current.MainPage.Navigation.PopAsync();
                        });
                    }

                }


            };

  

                //need to add check if the machine is taken
                //need to add navigation to submit button
                //what happens when finish usage

            Navigation.PushModalAsync(scanPage);
            

        }
      
    }
}