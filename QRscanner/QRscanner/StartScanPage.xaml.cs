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
        public static int id_machine = 7;
        public static int id_member;
        int[] data = new int[5];
        
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
            data[1] = id_machine;
            data[2] = 0;
            data[3] = 0;
            data[4] = 0;
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
                        data[0] = id_member;
                        //caching_msg = "id_member = " + id_member + " has finished using id machine " + id_machine;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            App.finished = true;
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            await Navigation.PopAsync();
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
                            //THE MSG ISNT SHOWNG ON THE SCREEN, PLS FIX
                            caching_msg = "id_member = " + App.member_from_table + " is using id machine " + id_machine;
                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", caching_msg, "OK");
                        });
                    }
                }
                else
                {
                    //the machine is free to use
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                  
                        Application.Current.MainPage = new NavigationPage(new InfoUsage());
                        await App.Current.MainPage.Navigation.PopAsync();
                    });

                }


            };

                 
                


                //need to add check if the machine is taken
                //need to add navigation to submit button
                //what happens when finish usage



                /*data[0]=id_member;
                data[1] = id_machine;
                
                string messageJson = JsonConvert.SerializeObject(data);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);*/
                

            

            Navigation.PushModalAsync(scanPage);
            

        }
      
    }
}