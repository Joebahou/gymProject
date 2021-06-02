using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homePage : ContentPage
    {
        private string name_log;
        public string Name_log
        {
            get { return name_log; }
            set
            {
                name_log = value;
               

            }
        }
        public homePage()
        {
            InitializeComponent();
            Name_log = Models.User.Name;
            BindingContext = this;

        }
        private void statisticsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatsPage());
        }
        private void machinesButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Page1());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new Pages.LoginPage(), this);
            await Navigation.PopAsync();
        }

        
    }
}