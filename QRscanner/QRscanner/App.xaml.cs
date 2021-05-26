using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace QRscanner
{
    public partial class App : Application
    {
        public static HubConnection connection;
        public App()
        {
            InitializeComponent();

           
            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            await QRscanner.MainPage.connection.StartAsync();

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
