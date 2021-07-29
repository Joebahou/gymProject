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
        public static HubConnection connection;
        public static string name_machine;
        public static int id_machine;
        public static int id_member;
        private int alert = 1;
        public static Models.Machine this_machine;
        public static bool is_working;
        public bool Is_working { get; set; }
        int[] dataHelp = new int[1];
        int[] dataBrokenMachine = new int[2];
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public MainPage()
        {
            Is_working = is_working;
            OnPropertyChanged("Is_working");
            InitializeComponent();
            if (!is_working)
            {
                not_working_label.IsVisible = true;
                broken_machine_Button_set_by_owner.IsVisible = true;
            }
            if (this_machine.Alert_broken==1)
                broken_machine_Button_set_by_owner.IsVisible = true;
            Name_log = name_machine;
            BindingContext = this;
            Set_signalR();
        }

        public async void Set_signalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int[]>("newMessage", (msgupdate) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    String resultusage = "";
                    int usage = msgupdate[1];
                    if (usage == 1)
                        resultusage = "id_member = " + msgupdate[2] + " has started using id machine " + msgupdate[0];
                    else
                        resultusage = "id_member = " + msgupdate[2] + " has finished using id machine " + msgupdate[0];
                    await App.Current.MainPage.DisplayAlert("Scanned Barcode", resultusage, "OK");


                });
            });
            await connection.StartAsync();
        }

        private async void scanButton_Clicked(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmember,taken FROM gym_schema.machines WHERE idmachine = @id_machine;";
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            App.member_from_table = reader.GetInt32(0);
                            App.taken = reader.GetInt32(1);
                        }
                    }

                }


            }

            await Navigation.PushAsync(new StartScanPage());

        }
        public async void helpButton_Clicked(object sender, EventArgs e)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                dataHelp[0] = id_machine;
                string messageJson = JsonConvert.SerializeObject(dataHelp);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);
                String resultusage = "A help message has been sent to all trainers";
                await App.Current.MainPage.DisplayAlert("HELP ME", resultusage, "OK");
            }

        }
        public async void broken_machine_Button_alert_Clicked(object sender, EventArgs e)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                dataBrokenMachine[0] = id_machine;
                dataBrokenMachine[1] = 0;
                string messageJson = JsonConvert.SerializeObject(dataBrokenMachine);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                await Client.SendEventAsync(message);
                String resultusage = "An alert has been sent to the owner";
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"UPDATE machines SET alert_broken=@alert WHERE idmachine=@id_machine and name=@name;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.Parameters.AddWithValue("@alert", alert);
                        command.Parameters.AddWithValue("@name", name_machine);
                        command.ExecuteNonQuery();


                    }
                }
                await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                broken_machine_Button_set_by_owner.IsVisible = true;

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
        public async void click_button_change(Object sender, System.EventArgs e)
        {
            bool isOwner=false;
            int new_working;
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT type FROM gym_schema.members WHERE email = @email and password=@password;";
                    command.Parameters.AddWithValue("@email", Email.Text);
                    command.Parameters.AddWithValue("@password", Password.Text);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                if (reader.GetInt32(0) == 2)
                                    isOwner = true;
                            }

                                
                        }
                    }

                }
                if (isOwner)
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
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"UPDATE machines SET working=@new_working, alert_broken=@alert WHERE idmachine=@id_machine and name=@name;";
                        command.Parameters.AddWithValue("@id_machine", id_machine);
                        command.Parameters.AddWithValue("@new_working", new_working);
                        command.Parameters.AddWithValue("@alert", 1-alert);
                        command.Parameters.AddWithValue("@name", name_machine);
                        command.ExecuteNonQuery();


                    }
                    
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
                    }
                    string messageJson = JsonConvert.SerializeObject(dataBrokenMachine);
                    Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                    await Client.SendEventAsync(message);
                    String resultusage;
                    if (this_machine.Available==1)
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
            

        }
    }
}
