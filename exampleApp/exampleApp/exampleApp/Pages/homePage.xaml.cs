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
using System.IO;
using Newtonsoft.Json;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /* Home page to every user */
    public partial class homePage : ContentPage
    {
        public   HubConnection connection;
       
        public static ObservableCollection<Msg> list_bind = new ObservableCollection<Msg>();

        public static ObservableCollection<Msg> List_bind { get { return list_bind; } }
        private string name_log;
        public string Name_log //name of the user loged in
        {
            get { return name_log; }
            set
            {
                name_log = value;
               

            }
        }
        public string notifications_count { get; set; } 
        
        public Boolean IsOwner { get; set; }
        //class for the msg in the notification
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

            //set the home page to a trainee
            if (Models.User.Type == 0)
            {
                Set_signalR_to_Trainee();
                seeQRbutton.IsVisible = true;
                editMachineButton.IsVisible = false;
                scheduleForTrainerButton.IsVisible = false;
               
            }
            //set the home page to a trainer
            if (Models.User.Type == 1)
            {
                Set_signalR_to_trainer();
                editMachineButton.IsVisible = false;
                scheduleForTraineeButton.IsVisible = false;
                //ConnectDataBase();
            }
            //set the home page to the owner
            if (Models.User.Type == 2)
            {
                //ConnectDataBase();
                Set_signalR_to_owner(); 
                
                IsOwner = true;
                machinesButton.IsVisible = false;
                scheduleButton.IsVisible = false;
                scheduleForTrainerButton.IsVisible = false;
                scheduleForTraineeButton.IsVisible = false;


            }
            BindingContext = this;
           
           
        }

        //Init the alert list according to the type of user.
        //Traine and trainee : broken machines.
        //Owner : alerts on broken machines
        private void Init_alert_list()
        {
            List<Models.Machine> list_for_notification = new List<Models.Machine>();
            string req = "https://gymfuctions.azurewebsites.net/api/initListMachines?query=select_machines";
            string result = Models.Connection.get_result_from_http(req, true);
            list_for_notification = JsonConvert.DeserializeObject<List<Models.Machine>>(result);

            //alerts for owner
            if (Models.User.Type == 2)
            {
               foreach(Models.Machine m in list_for_notification)
                {
                    if (m.Alert_broken == 1)
                    {
                        int id_machine = m.Id_machine;
                        string name_machine = m.Name;
                        int alert_broken = m.Alert_broken;
                        string msg = "someone alerted that " +name_machine+ " machine isn't working";
                        Msg temp = new Msg { msg = msg, type = "alert", id_machine = id_machine, clear_msg_icon = false };
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                    }
                }

              

                OnPropertyChanged("notifications_count");
                notification_view.ItemsSource = list_bind;

            }
            //alerts for trainer and trainee
            else
            {
                foreach(Models.Machine m in list_for_notification)
                {
                    if (m.Available == 0)
                    {
                        int id_machine = m.Id_machine;
                        string name_machine = m.Name;
                        int working = m.Available;
                        string msg = name_machine + " machine, id " + id_machine + " isn't working, check your schedule";
                        Msg temp = new Msg { msg = msg, type = "alert", id_machine = id_machine, clear_msg_icon = true };
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                    }
                }
               

                OnPropertyChanged("notifications_count");
                notification_view.ItemsSource = list_bind;
            }
            
        }
        

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
                        //if the user not in the used_machines page - go back to the home page
                        if (App.Current.MainPage.Navigation.NavigationStack.Last() is  UsedMachines)
                        {
                           
                        }
                        else
                        {
                            await App.Current.MainPage.Navigation.PopToRootAsync();
                        }
                      


                    }


                });

            });
            //message that machine is now fixed.
          
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
                        //if the user not in the used_machines page - go back to the home page
                        if (App.Current.MainPage.Navigation.NavigationStack.Last() is UsedMachines)
                        {

                        }
                        else
                        {
                            await App.Current.MainPage.Navigation.PopToRootAsync();
                        }



                    }


                });

            });

            //machine is delted
            
            this.connection.On<object[]>("deleteMachine", (result) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {


                    if (Models.User.Type == 0)
                    {


                        String resultusage = "";
                        string id_machine_string = result[0].ToString();
     
                        resultusage = result[1]+ " machine is deleted. please refresh your app";
                        Msg temp = new Msg { msg = resultusage, type = "deleted", clear_msg_icon = true, id_machine = Int32.Parse(id_machine_string) };
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                        await App.Current.MainPage.Navigation.PopToRootAsync();



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

            //help message
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
                          //if the user not in the used_machines page - go back to the home page
                            if (App.Current.MainPage.Navigation.NavigationStack.Last() is UsedMachines)
                            {

                            }
                            else
                            {
                            await App.Current.MainPage.Navigation.PopToRootAsync();
                            }


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
                            //if the user not in the used_machines page - go back to the home page
                            if (App.Current.MainPage.Navigation.NavigationStack.Last() is UsedMachines)
                            {

                            }
                            else
                            {
                            await App.Current.MainPage.Navigation.PopToRootAsync();
                            }



                    }


                });

            });
            this.connection.On<object[]>("deleteMachine", (result) =>
            {

                MainThread.BeginInvokeOnMainThread(async () =>
                {


                    if (Models.User.Type == 1)
                    {


                        String resultusage = "";
                        string id_machine_string = result[0].ToString();

                        resultusage = result[1] + " machine is deleted. please refresh your app";
                        Msg temp = new Msg { msg = resultusage, type = "deleted", clear_msg_icon = true, id_machine = Int32.Parse(id_machine_string) };
                        list_bind.Add(temp);
                        notification_view.ItemsSource = list_bind;
                        int current_count = Int32.Parse(notifications_count);
                        current_count++;
                        notifications_count = current_count.ToString();
                        OnPropertyChanged("notifications_count");

                        await App.Current.MainPage.DisplayAlert("Alert", resultusage, "OK");
                        await App.Current.MainPage.Navigation.PopToRootAsync();



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
                        await App.Current.MainPage.Navigation.PopToRootAsync();
                    }


                });

            });

            //there was an alert on machine but the machine is ok.
            //athe message of the alert is deleted
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
        private async void schedule_for_traineeButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            await Task.Delay(20);
            await Navigation.PushAsync(new Schedule_for_trainee());
            activityIndicator.IsVisible = false;
        }

        //clicked to see the real_time page (UsedMachines)
        private async void machinesButton_Clicked(object sender, EventArgs e)
        {
            UsedMachines.machines_list = new List<Models.Machine>();
            List<Models.Machine> list_for_used = new List<Models.Machine>();
            string req = "https://gymfuctions.azurewebsites.net/api/initListMachines?query=select_machines";
            string result = Models.Connection.get_result_from_http(req, true);
            list_for_used = JsonConvert.DeserializeObject<List<Models.Machine>>(result);

            foreach (Models.Machine m in list_for_used)
            {
                int id_machine = m.Id_machine;
                int taken = m.Taken;
                string name = m.Name;
                int available = m.Available;
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

          

            await  Navigation.PushAsync(new UsedMachines());
        }

        //logout from the app
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
            Models.User.Type = -1;
            App.Current.MainPage = new NavigationPage(new Pages.LoginPage());
            await App.Current.MainPage.Navigation.PopAsync();
            
        }

        //see the qr code of a trainee
        private async void seeQRbutton_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new QrImage());

        }

        private void notification_clicked(object sender, EventArgs e)
        {
           
            notification_view.ItemsSource = list_bind;
            popupNotifications.IsVisible = !popupNotifications.IsVisible;
           
        }
        private async void signUp_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new signUp());

        }
       
        public void helped_clicked(Object sender, System.EventArgs e)
        {
            Button thebutton = (Button)sender;
            Msg msg = thebutton.BindingContext as Msg;
            list_bind.Remove(msg);
            notification_view.ItemsSource = list_bind;
        } 
        //clicked on the "vi" to delete notification
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

        //clicked to see avaliable machine to the owner
        private async void editMachineButton_Clicked(object sender, EventArgs e)
        {
            availableMachines_owner.machines_list = new List<Models.Machine>();
            List<Models.Machine> list_for_owner = new List<Models.Machine>();
            string req = "https://gymfuctions.azurewebsites.net/api/initListMachines?query=select_machines";
            string result = Models.Connection.get_result_from_http(req, true);
            list_for_owner = JsonConvert.DeserializeObject<List<Models.Machine>>(result);
            foreach (Models.Machine m in list_for_owner)
            {
                int id_machine = m.Id_machine;
                string name = m.Name;
                int available = m.Available;
                Models.Machine temp = new Models.Machine(id_machine, name, available);
                availableMachines_owner.machines_list.Add(temp);

            }


                await Navigation.PushAsync(new availableMachines_owner());
        }


    }

    
}