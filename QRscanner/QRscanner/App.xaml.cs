using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySqlConnector;

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

            MainPage = new NavigationPage(new welcomePage());
        }

        protected override void OnStart()
        {
            finished = false;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
