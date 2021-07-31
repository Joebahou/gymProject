using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homePage : ContentPage
    {
        public   HubConnection connection;
        //MySqlConnection conn;
        public static ObservableCollection<Msg> list_bind = new ObservableCollection<Msg>();

        public static ObservableCollection<Msg> List_bind { get { return list_bind; } }
        private string name_log;
        public string Name_log
        {
            get { return name_log; }
            set
            {
                name_log = value;
               

            }
        }
        public string notifications_count { get; set; }
        
        public Boolean IsOwner { get; set; }
        public class Msg
        {
            public string msg { get; set; }
            public string type { get; set; }
            public int id_machine { get; set; }
            public Boolean clear_msg_icon { get; set; }
            
        }
        public homePage()
        {
            list_bind = new ObservableCollection<Msg>();
            notifications_count = "0";
            InitializeComponent();
            Name_log ="Hello "+ Models.User.Name;
            Init_alert_list();
            if (Models.User.Type == 0)
            {
                Set_signalR_to_Trainee();
                editMachineButton.IsVisible = false;
                scheduleForTrainerButton.IsVisible = false;
               
            }

            if (Models.User.Type == 1)
            {
                Set_signalR_to_trainer();
                editMachineButton.IsVisible = false;
                //ConnectDataBase();
            }
            if (Models.User.Type == 2)
            {
                //ConnectDataBase();
                Set_signalR_to_owner(); 
                
                IsOwner = true;
                machinesButton.IsVisible = false;
                scheduleButton.IsVisible = false;
                scheduleForTrainerButton.IsVisible = false;


            }
            BindingContext = this;
           
           
        }
        private void Init_alert_list()
        {
            if (Models.User.Type == 2)
            {
                using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT idmachine,name,alert_broken " +
                        "FROM machines " +
                        "WHERE alert_broken=1;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(0);
                                string name_machine = reader.GetString(1);
                                int alert_broken = reader.GetInt32(2);
                                string msg = "someone alerted that " + " machine, id " + id_machine + " isn't working";
                                Msg temp = new Msg { msg = msg, type = "alert", id_machine = id_machine, clear_msg_icon = false };
                                list_bind.Add(temp);
                                notification_view.ItemsSource = list_bind;
                                int current_count = Int32.Parse(notifications_count);
                                current_count++;
                                notifications_count = current_count.ToString();





                            }
                        }


                    }
                }

                OnPropertyChanged("notifications_count");
                notification_view.ItemsSource = list_bind;

            }
            else
            {
                using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT idmachine,name,working " +
                        "FROM machines " +
                        "WHERE working=0;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(0);
                                string name_machine = reader.GetString(1);
                                int working = reader.GetInt32(2);
                                string msg = name_machine + " machine, id " + id_machine + " isn't working, check your schedule";
                                Msg temp = new Msg { msg = msg, type = "alert", id_machine = id_machine, clear_msg_icon = true };
                                list_bind.Add(temp);
                                notification_view.ItemsSource = list_bind;
                                int current_count = Int32.Parse(notifications_count);
                                current_count++;
                                notifications_count = current_count.ToString();





                            }
                        }


                    }
                }

                OnPropertyChanged("notifications_count");
                notification_view.ItemsSource = list_bind;
            }
            
        }
        /*
        private void ConnectDataBase()
        {
            try
            {

                Console.WriteLine("Trying to connect");
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "gymservernew.mysql.database.azure.com",
                    Database = "gym_schema",
                    UserID = "gymAdmin",
                    Password = "gym1Admin",
                    SslMode = MySqlSslMode.Required,
                };

                conn = new MySqlConnection(builder.ConnectionString);

                conn.Open();
                Console.WriteLine(conn.State.ToString() + Environment.NewLine);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }*/
        public async void Set_signalR_to_Trainee()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);

            };

           

            //message that machine is really broken
            this.connection.On<object[]>("BrokenMachine_real", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {


                    if (Models.User.Type == 0)
                    {

                        String resultusage = "";
                        string id_machine_msg_string = broken_msg[0].ToString();
                        int id_machine_msg = Int32.Parse(id_machine_msg_string);


                        resultusage = broken_msg[1] + " machine, id " + broken_msg[0] + " isn't working, check your schedule";
                        Msg temp = new Msg { msg = resultusage, type = "broken", id_machine = id_machine_msg ,clear_msg_icon=true};
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");


                    }


                });

            });
            //message that machine is now fixed.
            // alert is shown to the trainer if the machine is in h
            this.connection.On<object[]>("BrokenMachine_fixed", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {


                    if (Models.User.Type == 0)
                    {


                        String resultusage = "";
                        string id_machine_msg_string = broken_msg[0].ToString();
                        int id_machine_msg = Int32.Parse(id_machine_msg_string);


                        resultusage = broken_msg[1] + " machine, id " + broken_msg[0] + " is fixed, check your schedule";
                        Msg temp = new Msg { msg = resultusage, type = "fixed", id_machine = id_machine_msg,clear_msg_icon=true };
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");



                    }


                });

            });
            await this.connection.StartAsync();
        }
        public async void Set_signalR_to_trainer()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                
            };

            this.connection.On<object[]>("helpMessage", (help_msg) =>
            {
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {

                        if (Models.User.Type == 1)
                        {
                            string id_machine_msg_string = help_msg[0].ToString();
                            int id_machine_msg = Int32.Parse(id_machine_msg_string);
                            String resultusage = "";
                            
                            resultusage = "need help in "+help_msg[1]+ " machine, id " + help_msg[0];
                            Msg temp = new Msg { msg = resultusage ,type="help",id_machine=id_machine_msg,clear_msg_icon=true};
                            list_bind.Add(temp);
                            notification_view.ItemsSource = list_bind;
                            int current_count = Int32.Parse(notifications_count);
                            current_count++;
                            notifications_count = current_count.ToString();
                            OnPropertyChanged("notifications_count");
                          

                            await App.Current.MainPage.DisplayAlert("HELP ME", resultusage, "OK");
                        }
                        

                    });
                
            });

            //message that machine is really broken
            this.connection.On<object[]>("BrokenMachine_real", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                   

                    if (Models.User.Type ==1)
                    {
                        
                           String resultusage = "";
                            string id_machine_msg_string = broken_msg[0].ToString();
                            int id_machine_msg = Int32.Parse(id_machine_msg_string);


                            resultusage = broken_msg[1] + " machine, id " + broken_msg[0] + " isn't working, check your schedule";
                            Msg temp = new Msg { msg = resultusage ,type="broken",id_machine=id_machine_msg,clear_msg_icon=true};
                            list_bind.Add(temp);
                            notification_view.ItemsSource = list_bind;
                            int current_count = Int32.Parse(notifications_count);
                            current_count++;
                            notifications_count = current_count.ToString();
                            OnPropertyChanged("notifications_count");

                            await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");

                       
                    }


                });

            });
            //message that machine is now fixed.
            // alert is shown to the trainer if the machine is in h
            this.connection.On<object[]>("BrokenMachine_fixed", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    

                    if (Models.User.Type == 1)
                    {
                      
                        
                            String resultusage = "";
                            string id_machine_msg_string = broken_msg[0].ToString();
                            int id_machine_msg = Int32.Parse(id_machine_msg_string);


                            resultusage = broken_msg[1] + " machine, id " + broken_msg[0] + " is fixed, check your schedule";
                            Msg temp = new Msg { msg = resultusage, type = "fixed", id_machine = id_machine_msg,clear_msg_icon=true };
                            list_bind.Add(temp);
                            notification_view.ItemsSource = list_bind;
                            int current_count = Int32.Parse(notifications_count);
                            current_count++;
                            notifications_count = current_count.ToString();
                            OnPropertyChanged("notifications_count");

                            await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");

                        

                    }


                });

            });
            await this.connection.StartAsync();
        }
        public async void Set_signalR_to_owner()
        {
            this.connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            this.connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);

            };


            this.connection.On<object[]>("BrokenMachine_alert", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    if (Models.User.Type == 2)
                    {
                        String resultusage = "";
                        string id_machine_msg_string = broken_msg[0].ToString();
                        int id_machine_msg = Int32.Parse(id_machine_msg_string);
                        Boolean found = false;
                        foreach(Msg m in list_bind)
                        {
                            if (m.id_machine == id_machine_msg)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            resultusage = "someone alerted that " + broken_msg[1] + " machine, id " + broken_msg[0] + " isn't working";
                            Msg temp = new Msg { msg = resultusage, type = "alert", id_machine = id_machine_msg, clear_msg_icon = false };
                            list_bind.Add(temp);
                            notification_view.ItemsSource = list_bind;
                            int current_count = Int32.Parse(notifications_count);
                            current_count++;
                            notifications_count = current_count.ToString();
                            OnPropertyChanged("notifications_count");

                            await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                        }

                        
                    }


                });

            });
            //message that machine is really broken
            //alert is shown to the owner and the message of the alert is deleted.
            this.connection.On<object[]>("BrokenMachine_real", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    if (Models.User.Type == 2)
                    {
                        String resultusage = "";
                        string id_machine_msg_string = broken_msg[0].ToString();
                        int id_machine_msg = Int32.Parse(id_machine_msg_string);

                        resultusage = broken_msg[1] + " machine, id " + broken_msg[0] + " broken";
                        Msg msg_to_delete = null;
                       
                        foreach(Msg m in list_bind)
                        {
                            if(m.type=="alert" && m.id_machine== id_machine_msg)
                            {
                                msg_to_delete = m;
                                break;

                            }
                        }

                        list_bind.Remove(msg_to_delete);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count = Math.Max(0, current_count - 1);
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                    }


                });

            });

            //there was an alert on machine but the machine is ok.
            this.connection.On<object[]>("BrokenMachine_ignore", (broken_msg) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    if (Models.User.Type == 2)
                    {
                        String resultusage = "";
                        string id_machine_msg_string = broken_msg[0].ToString();
                        int id_machine_msg = Int32.Parse(id_machine_msg_string);

                
                        Msg msg_to_delete = null;

                        foreach (Msg m in list_bind)
                        {
                            if (m.type == "alert" && m.id_machine == id_machine_msg)
                            {
                                msg_to_delete = m;
                                break;

                            }
                        }

                        list_bind.Remove(msg_to_delete);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count = Math.Max(0, current_count - 1);
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                       
                    }


                });

            });
            await this.connection.StartAsync();
        }
        private void statisticsButton_Clicked(object sender, EventArgs e)
        {
            if (Models.User.Type == 0) { Navigation.PushAsync(new StatisticsNavigationTrainee()); }
            if (Models.User.Type == 1) { Navigation.PushAsync(new TrainerStatistics.StatisticsNavigationTrainer()); }
            if (Models.User.Type == 2) { Navigation.PushAsync(new OwnerStatistics.StatisticsNavigationOwner()); }
        }
        private void scheduleButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AllSchedule());
        }
        private async void schedule_for_trainerButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            await Task.Delay(20);
            await Navigation.PushAsync(new Schedule_for_trainer());
            activityIndicator.IsVisible = false;
        }

        private async void machinesButton_Clicked(object sender, EventArgs e)
        {
            UsedMachines.machines_list = new List<Models.Machine>();

         

            using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,taken,name,working FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            int taken = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            int available= reader.GetInt32(3);
                            Models.Machine temp;
                            if (available == 1)
                            {
                                if (taken == 0)
                                {
                                    temp = new Models.Machine(name, Color.Green, id_machine);

                                }
                                else
                                {
                                    temp = new Models.Machine(name, Color.Red, id_machine);
                                }
                                
                            }
                            else
                            {
                                temp = new Models.Machine(name, Color.Yellow, id_machine);
                            }
                            UsedMachines.machines_list.Add(temp);

                        }
                    }


                }
            }

           await  Navigation.PushAsync(new UsedMachines());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            if ( connection.State==HubConnectionState.Connected)
            {
                await this.connection.StopAsync();
            }
            else
            {
                this.connection = null;
            }

            App.Current.MainPage = new NavigationPage(new Pages.LoginPage());
            await App.Current.MainPage.Navigation.PopAsync();
            
        }
        private void notification_clicked(object sender, EventArgs e)
        {
           
            notification_view.ItemsSource = list_bind;
            popupNotifications.IsVisible = !popupNotifications.IsVisible;
           
        }
        public void helped_clicked(Object sender, System.EventArgs e)
        {
            Button thebutton = (Button)sender;
            Msg msg = thebutton.BindingContext as Msg;
            list_bind.Remove(msg);
            notification_view.ItemsSource = list_bind;
        }
        public void clear_clicked_image(Object sender, System.EventArgs e)
        {
           Image clear_image = (Image)sender;
            Msg msg = clear_image.BindingContext as Msg;
            list_bind.Remove(msg);
            notification_view.ItemsSource = list_bind;
            int current_count= Int32.Parse(notifications_count);
            current_count--;
            notifications_count = current_count.ToString();
            OnPropertyChanged("notifications_count");
        }

        private async void editMachineButton_Clicked(object sender, EventArgs e)
        {
            availableMachines_owner.machines_list = new List<Models.Machine>();


            using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,name,working FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int available = reader.GetInt32(2);
                            Models.Machine temp = new Models.Machine(id_machine,name,available);
                            availableMachines_owner.machines_list.Add(temp);
                        }
                    }


                }
            }
            
            await Navigation.PushAsync(new availableMachines_owner());
        }


    }

    
}