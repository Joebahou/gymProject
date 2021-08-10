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
      

       public App()
        {
            InitializeComponent();
            //MainPage page = new MainPage();

           MainPage = new NavigationPage(new  Pages.LoginPage());
            //MainPage = new Page1();
        }

        protected override  void OnStart()
        {
        
          


        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
