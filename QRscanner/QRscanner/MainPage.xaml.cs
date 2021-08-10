using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using Microsoft.AspNetCore.SignalR.Client;

using Xamarin.Essentials;
using MySqlConnector;


namespace QRscanner
{
    public partial class MainPage : ContentPage
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
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        //public HubConnection connection;
        public static string name_machine;
        public static int id_machine;
        public static int id_member;
        private int alert = 1;
        public static Models.Machine this_machine;
        public static bool is_working;
        int [] dataClose = new int[5];
        public bool Is_working { get; set; }
        int[] dataHelp = new int[1];
        int[] dataBrokenMachine = new int[2];
       
        public MainPage()
        {   
            //changing availablilty status button binding
            Is_working = is_working;
            OnPropertyChanged("Is_working");
            InitializeComponent();
            //buttons visibilty init
            if (!is_working)
            {
                not_working_label.IsVisible = true;
                broken_machine_Button_set_by_owner.IsVisible = true;
            }
            if (this_machine.Alert_broken==1)
                broken_machine_Button_set_by_owner.IsVisible = true;
            Name_log = name_machine;
            BindingContext = this;
            
        }
     
        
        private async void scanButton_Clicked(object sender, EventArgs e)
        {

            App.member_from_table = this_machine.Id_member;
            App.taken = this_machine.Taken;
            if (App.member_from_table != -1)
            {
                string req = "https://gymfuctions.azurewebsites.net/api/selecet_QRscanner?query=select_name_member&id_member="+ App.member_from_table;
                App.name_of_member = Models.Connection.get_result_from_http(req, false);
            }


                await Navigation.PushAsync(new StartScanPage());

        }
        public async void helpButton_Clicked(object sender, EventArgs e)
        {
            if (App.connection.State == HubConnectionState.Connected)
            {
                activityIndicator.IsVisible = true;
                dataHelp[0] = id_machine;
                string messageJson = JsonConvert.SerializeObject(dataHelp);
                //an help event handled by eventhub function
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                
                await Client.SendEventAsync(message);
                String resultusage = "A help message has been sent to all trainers";
                await App.Current.MainPage.DisplayAlert("HELP ME", resultusage, "OK");
                activityIndicator.IsVisible = false;
            }

        }
        public async void broken_machine_Button_alert_Clicked(object sender, EventArgs e)
        {
            if (App.connection.State == HubConnectionState.Connected)
            {
                activityIndicator.IsVisible = true;
                dataBrokenMachine[0] = id_machine;
                dataBrokenMachine[1] = 0;
                //a broken machine event handled by eventhub function
                string messageJson = JsonConvert.SerializeObject(dataBrokenMachine);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);
                String resultusage = "An alert has been sent to the owner";
                await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                this_machine.Alert_broken = 1;
                broken_machine_Button_set_by_owner.IsVisible = true;
                activityIndicator.IsVisible = false;
            }

        }
        public void broken_machine_Button_set_by_owner_Clicked(object sender, EventArgs e)
        {
            popupLogin.IsVisible = true;
        }
        public void click_button_cancel(Object sender, System.EventArgs e)
        {
            popupLogin.IsVisible = false;

        }

        public async void click_button_ignore(Object sender, System.EventArgs e)
        {

            string req = "https://gymfuctions.azurewebsites.net/api/check_isOwner?query=check_isOwner&username=" +Email.Text+"&password="+Password.Text;
            string isOwner = Models.Connection.get_result_from_http(req, false);

            
            if (isOwner=="true"|| isOwner == "True")
                {

                    popupLogin.IsVisible = false;
                    if(is_working)
                        broken_machine_Button_set_by_owner.IsVisible = false;
                    //update
                    this_machine.Alert_broken = 0;
                    dataBrokenMachine[0] = id_machine;
                    dataBrokenMachine[1] = 3;
                    string messageJson = JsonConvert.SerializeObject(dataBrokenMachine);
                    Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                    await Client.SendEventAsync(message);
                    String resultusage;
                    if (this_machine.Available == 1)
                        resultusage = "Now the machine is set as working";
                    else resultusage = "Now the machine is set as not working";
                    await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                }
                else
                {
                    string msg = "you are not the owner, you can't change this property";
                    await Application.Current.MainPage.DisplayAlert("Not An Owner", msg, "OK");
                }
            


        }
        public async void click_button_change(Object sender, System.EventArgs e)
        {

            string req = "https://gymfuctions.azurewebsites.net/api/check_isOwner?query=check_isOwner&username=" + Email.Text + "&password=" + Password.Text;
            string isOwner = Models.Connection.get_result_from_http(req, false);
            int new_working;


            if (isOwner == "true" || isOwner == "True")
            {
                    
                    popupLogin.IsVisible = false;
                    is_working = !is_working;
                    Is_working = !Is_working;
                    if (is_working)
                        broken_machine_Button_set_by_owner.IsVisible = false;
                    not_working_label.IsVisible = !not_working_label.IsVisible;
                    OnPropertyChanged("Is_working");
                    BindingContext = this;
                    if (is_working)
                        new_working = 1;
                    else new_working = 0;
                    //update
                    
                    dataBrokenMachine[0] = id_machine;
                    if (is_working)
                    {
                        dataBrokenMachine[1] = 2;
                        this_machine.Available = 1;
                        

                }
                    else
                    {
                        dataBrokenMachine[1] = 1;
                        this_machine.Available = 0;
                    if (this_machine.Id_member != -1)
                    {
                        dataClose[0] = this_machine.Id_member;
                        dataClose[1] = this_machine.Id_machine;
                        dataClose[2] = 0;
                        dataClose[3] = 0;
                        dataClose[4] = 0;
                        string messageJson1 = JsonConvert.SerializeObject(dataClose);
                        Message message1 = new Message(Encoding.ASCII.GetBytes(messageJson1)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                        await Client.SendEventAsync(message1);
                        //a broken machine event handled by eventhub function
                        activityIndicator.IsVisible = true;
                        await Task.Delay(2500);
                        activityIndicator.IsVisible = false;
                    }

                }
                    string messageJson = JsonConvert.SerializeObject(dataBrokenMachine);
                    Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                    await Client.SendEventAsync(message);
                    String resultusage;
                    if (this_machine.Available==1)
                            resultusage = "Now the machine is set as working";
                    else resultusage = "Now the machine is set as not working. stopping the usage of this machine";
                    await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                }
                else
                {
                    string msg = "you are not the owner, you can't change this property";
                    await Application.Current.MainPage.DisplayAlert("Not An Owner", msg, "OK");
                }
            }
            

        
    }
}
