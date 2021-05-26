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
            //MainPage page = new MainPage();

           MainPage = new NavigationPage(new  Pages.LoginPage());
            //MainPage = new Page1();
        }

        protected override async void OnStart()
        {
           /* int[] data=new int[2];
            data[0] = 5;
            data[1] = 7;
            string messageJson = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            await Client.SendEventAsync(message);*/
            //await exampleApp.Page1.connection.StartAsync();
          


        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
