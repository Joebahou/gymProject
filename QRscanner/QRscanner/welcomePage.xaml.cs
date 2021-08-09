using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class welcomePage : ContentPage
    {
       
        public welcomePage()
        {

            InitializeComponent();
           
        }
       
        
        private async void availablemachinesButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            activityIndicatorLabel.IsVisible = true;
            await Task.Delay(20);
            checkboxmachinePage.machines_list = new List<Models.Machine>();


            List<Models.Machine> list_for_used = new List<Models.Machine>();
            string req = "https://gymfuctions.azurewebsites.net/api/initListMachines?query=select_machines";
            string result = Models.Connection.get_result_from_http(req, true);
            checkboxmachinePage.machines_list = JsonConvert.DeserializeObject<List<Models.Machine>>(result);
            await Navigation.PushAsync(new checkboxmachinePage());
            activityIndicator.IsVisible = false;
            activityIndicatorLabel.IsVisible = false;
        }
    }
}