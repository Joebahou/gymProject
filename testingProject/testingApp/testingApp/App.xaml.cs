using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

namespace testingApp
{
    public partial class App : Application
    {
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            int [] data = {5,7};
            string messageJson = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            await Client.SendEventAsync(message);

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
