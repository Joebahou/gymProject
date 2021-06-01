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
        public static bool finished;
        public static int member_from_table;
        public static int taken;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            finished = false;
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
