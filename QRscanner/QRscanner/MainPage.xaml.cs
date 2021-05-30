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
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Xamarin.Essentials;

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
        int[] data=new int[6];
        public MainPage()
        {
                InitializeComponent();
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
                            resultusage = "id_member = "+id_member+" is using id_machine "+id_machine;
                        else
                            resultusage = "id_member = " + id_member + " is not using id_machine " + id_machine;
                        await DisplayAlert("Scanned Barcode", resultusage , "OK");


                    });
                });
        }

        private async void ButtonScanDefault_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();

            scanPage.OnScanResult += async (result) =>
            {
                String res = result.Text; 
                scanPage.IsScanning = false;
                //Do something with result
                id_member = int.Parse(res);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    /*await Navigation.PopModalAsync();

                    await DisplayAlert("Scanned Barcode", res, "OK");*/
                    Application.Current.MainPage = new NavigationPage(new InfoUsage());
                    await App.Current.MainPage.Navigation.PopAsync();
                });

                //need to add check if the machine is taken
                //need to add navigation to submit button
                //what happens when finish usage

               
                
                /*data[0]=id_member;
                data[1] = id_machine;
                da
                string messageJson = JsonConvert.SerializeObject(data);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);*/
                

            };

            await Navigation.PushModalAsync(scanPage);
        }
    }
}
