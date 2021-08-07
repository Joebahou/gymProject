using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoUsage : ContentPage
    {
        DateTime submiting_time = DateTime.Now;
        DateTime nearest_future_schedule= new DateTime(2024, 7, 19, 0, 0, 0);
        TimeSpan hour_1 = new TimeSpan(1, 0, 0);
        string catching_msg;
        public int[] dataUsage = new int[5];
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

       
        Models.Usage vm;
        public InfoUsage()
        {
            InitializeComponent();
            vm = new Models.Usage();
            this.BindingContext = vm;
            


        }
        public int additional_info(String info)
        {
            int result;
            if (info != null)
            {

                if (Int32.TryParse(info, out result))
                    return result;
                else
                {
                    return -1;
                }

            }
            else
                return 0;

        }

        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            
            activityIndicator.IsVisible = true;
            await Task.Delay(10);
            dataUsage[0] = StartScanPage.id_member;
            dataUsage[1] = MainPage.id_machine;
            dataUsage[2] = additional_info(vm.Weight_Or_Speed);
            dataUsage[3] = additional_info(vm.Reps);
            dataUsage[4] = additional_info(vm.Sets);
            if (dataUsage[2] == -1 || dataUsage[3] == -1 || dataUsage[4] == -1||!((dataUsage[2] != 0 && dataUsage[3] != 0 & dataUsage[4] != 0)|| (dataUsage[2] != 0 && dataUsage[3] == 0 & dataUsage[4] == 0)))

            {
                catching_msg = "All information must be numbers lager than 0";
                await App.Current.MainPage.DisplayAlert("Error", catching_msg, "ok");
                activityIndicator.IsVisible = false;

            }
            else
            {
                string messageJson = JsonConvert.SerializeObject(dataUsage);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);
                string req = "https://gymfuctions.azurewebsites.net/api/selecet_QRscanner?query=select_nearest_future_schdule&id_machine=" + MainPage.id_machine + "&id_member=" + StartScanPage.id_member + "&submiting_time=" + submiting_time.ToString();
                string answer = Models.Connection.get_result_from_http(req, false);
                nearest_future_schedule = Convert.ToDateTime(answer);


                if ((nearest_future_schedule - submiting_time) < hour_1)
                {
                    catching_msg = "This machine has been booked by another trainer in about " + (nearest_future_schedule - submiting_time).Minutes + " minutes";
                    await App.Current.MainPage.DisplayAlert("Reminder", catching_msg, "I'll finish in time");
                }
                await Task.Delay(2000);
                activityIndicator.IsVisible = false;

                await App.Current.MainPage.Navigation.PopAsync();
                await App.Current.MainPage.Navigation.PopAsync();
            }

            


        }
        


    }
}