using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace exampleApp
{
    public partial class App : Application
    {
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

       public App()
        {
            InitializeComponent();

            MainPage = new Page1();
        }

        protected override async void OnStart()
        {
            /*string data = "5,7";
            string messageJson = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            await Client.SendEventAsync(message);*/
            await exampleApp.Page1.connection.StartAsync();
          


        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
