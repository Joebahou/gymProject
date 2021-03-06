
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllSchedule : ContentPage
        /*page to see all schedule */
    {
        ObservableCollection<Temp> list_bind = new ObservableCollection<Temp>();
        public ObservableCollection<Temp> List_bind { get { return list_bind; } }
        public ICommand Add_schedule_Command { get; set; }
        private Boolean loadingVisbile;
        public Boolean LoadingVisbile { get { return loadingVisbile; }
            set { loadingVisbile = value; }
        }

        List<Models.Machine> machines;
        Dictionary<int, Models.Machine> dict_machines; // dicitionary with id_machine(key):object of the machine(value)
        Dictionary<string, int> times = new Dictionary<string, int>(); //dicitionary of time as a string(key) : index of the column from the grid(value)

        string selected_date_string;
        public  AllSchedule()
        {
            InitializeComponent();

            Peopleview.ItemsSource = list_bind;
     
            pickerDateInit();
            machines = new List<Models.Machine>();
            dict_machines = new Dictionary<int, Models.Machine>();
            get_machines();
            Init_times();
            Init_schedule_Table(); 
            
      
        }
        
        //Inits the row with the hours of the schedule
        public void Init_schedule_Table()
        {
            scheduleTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//hours
            scheduleTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(13.5, GridUnitType.Star) });//empty
            for (int i = 1; i < 38; i++)
            {
                scheduleTable.ColumnDefinitions.Add(new ColumnDefinition{Width= new GridLength(12, GridUnitType.Star) });

            }
            var label = new Label
            {
               
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize=20
            };
            scheduleTable.Children.Add(label, 0, 0);

            foreach (string time in times.Keys){
                label = new Label
                {
                    Text =time,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 20
                };
                scheduleTable.Children.Add(label, times[time], 0);
            }
        }
        private void Init_times()
        {
            int index = 1;
            for(int i = 8; i < 20; i++)
            {
                for(int j = 0; j < 60; j += 20)
                {
                    string temp;
                    if (i < 10)
                    {
                        if (j == 0)
                        {
                            temp = "0" + i + ":00";
                        }
                        else
                        {
                            temp = "0" + i +":"+ j;
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            temp = i + ":00";
                        }
                        else
                        {
                            temp = i.ToString() +":"+ j.ToString();
                        }
                    }
                    times[temp] = index;
                    index++;

                }
            }
            times["20:00"] = index;
        }

        //class for json object from the http request
        public class Machine_from_sql
        {
            public string name { get; set; }
            public int id_machine { get; set; }
            public int working { get; set; }
        }

        //init dict_machines
        private  void get_machines()
        {
            dict_machines = new Dictionary<int, Models.Machine>();
            machines = new List<Models.Machine>();
            string req = "https://gymfuctions.azurewebsites.net/api/initListMachines?query=select_machines";
            string result = Models.Connection.get_result_from_http(req, true);
            machines = JsonConvert.DeserializeObject<List<Models.Machine>> (result);
            foreach(Models.Machine m in machines)
            {
                int id_machine = m.Id_machine;
                dict_machines[id_machine] = m;
            }
          


        }

        //Inits the date picker.
        private void pickerDateInit()
        {
            DateTime date_now = DateTime.Now.Date;
            List<DateTime> list_dates = new List<DateTime>();
            for(int i = 1; i <= 7; i++)
            {
                list_dates.Add(date_now);
                date_now = date_now.AddDays(1.0);
            }
          
         
            pickerDate.Items.Clear();
            foreach (DateTime name in list_dates)
            {
                pickerDate.Items.Add(name.ToShortDateString());
            }
        }

    
        //The trainer clicked on the button to add schedule.
        // if the schedule is free to choose - the function navigate to the Add_to_schedule page and saves the picked information.
        // if not - there is an alert.
        public async void handle_clicked_image(Object sender, System.EventArgs e)
        {
            popuploading2.IsVisible = true;
            
            popuploading.IsVisible = true;
            await Task.Delay(20);
            Image image_add = (Image)sender;
            int col = Grid.GetColumn(image_add) - 1;
            int hour = (col * 20) / 60 + 8;
            int minutes = (col * 20) % 60;
            string time = "";
            if (hour < 10)
            {
                if (minutes == 0)
                {
                    time = "0" + hour + ":00";
                }
                else
                {
                    time = "0" + hour + ":" + minutes;
                }
            }
            else
            {
                if (minutes == 0)
                {
                    time = hour + ":00";
                }
                else
                {
                    time = hour.ToString() + ":" + minutes.ToString();
                }
            }
            DateTime time_and_date = Convert.ToDateTime(selected_date_string + " " + time);
            Add_to_schedule.time_to_schedule = time_and_date;
            Temp row = image_add.BindingContext as Temp;
            Add_to_schedule.id_machine = row.id_machine;
            Add_to_schedule.name_machine_chosen = dict_machines[row.id_machine].Name;
            Boolean ready_to_choose_trainee = true;

            string parameters = "id_machine=" + row.id_machine +
               "&time_to_schedule=" + time_and_date.ToString();
            string req = "https://gymfuctions.azurewebsites.net/api/check_schedule?query=ready_to_choose_trainee&" + parameters;
            string result = Models.Connection.get_result_from_http(req, false);
            if(result== "machine_not_exists")
            {
                await App.Current.MainPage.DisplayAlert("Error", "The machine id deleted", "OK");
                await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                if (result == "false")
                {
                    ready_to_choose_trainee = false;
                }
                if (ready_to_choose_trainee)
                {
                    await Navigation.PushAsync(new Add_to_schedule());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "The schedule already taken. pleae refresh the page", "OK");

                }


                popuploading.IsVisible = false;
                popuploading2.IsVisible = false;
            }
          
          
          
        }
        /*class for the list to bind the grid */
        public class Temp
        {
            public string[] Li { get; set; }
            public bool[] button_arr { get; set; } //visability of the buttons in the schedule
            public int id_machine { get; set; }
            public Color name_color { get; set; }
            public Boolean text_visble_broken { get; set; }
            public Color color_row { get; set; }
        }

        // class for json object from the http request
        public class future_schedule
        {
            public string name_member { get; set; }
            public int id_machine { get; set; }
            public DateTime start_time { get; set; }
            public int id_member { get; set; }

            [JsonConstructor]
            public future_schedule(string name_member, int id_machine, DateTime start_time, int id_member)
            {
                this.name_member = name_member;
                this.id_machine = id_machine;
                this.start_time = start_time;
                this.id_member = id_member;
            }
        }
        //The trainer picked a date to see its schedule.
        // the function loads the grid wit the schedule of that date.
        private async void pickerDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            popuploading2.IsVisible = true;
            popuploading.IsVisible = true;
            await Task.Delay(20);
            //LoadingVisbile = true;
            selected_date_string = pickerDate.SelectedItem.ToString();
            DateTime selected_date = Convert.ToDateTime(selected_date_string);
            list_bind = new ObservableCollection<Temp>();
            Peopleview.ItemsSource = list_bind;
            get_machines();
            foreach(Models.Machine machine in machines)
            {
                machine.Init_schedule();

            }
            List<future_schedule> future_schedule_query = new List<future_schedule>();
            string parameters = "start_time=" + selected_date.ToString();
               
            string req = "https://gymfuctions.azurewebsites.net/api/select_all_schedule?query=select_schedule_for_date&"+parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            future_schedule_query = JsonConvert.DeserializeObject<List<future_schedule>>(result);
            foreach(future_schedule f in future_schedule_query)
            {
                int id_machine = f.id_machine;
                int id_member = f.id_member;
                DateTime start_time = f.start_time;
                string name_member = f.name_member;
                string hour = start_time.ToString("HH:mm");
                int index = times[hour];
                dict_machines[id_machine].schedule_machine[index] = name_member;
            }
     



            foreach (Models.Machine machine in machines)
            { 
                bool[] button_arr = new bool[38];
                button_arr[0] = false;
                if (machine.Available == 1)
                {
                    for (int i = 1; i < 38; i++)
                    {
                        if (machine.schedule_machine[i] != "")
                        {
                            button_arr[i] = false;
                        }
                        else
                        {
                            if (Models.User.Type == 1)
                            {
                                button_arr[i] = true;
                            }
                            else
                            {
                                button_arr[i] = false;

                            }

                        }
                    }
                }
                //if machine is broken-the row will be empty
                if (machine.Available == 0)
                {
                    
                    for (int i = 1; i < machine.schedule_machine.Length; i++)
                    {
                        machine.schedule_machine[i] = "broken";
                    }
                    list_bind.Add(new Temp {color_row=Color.Yellow, Li = machine.schedule_machine, button_arr = button_arr, id_machine = machine.Id_machine, name_color = Color.OrangeRed}); ;

                }
                else
                {
                    list_bind.Add(new Temp {color_row=Color.White, Li = machine.schedule_machine, button_arr = button_arr, id_machine = machine.Id_machine});
                }
            }

            popuploading.IsVisible = false;
            popuploading2.IsVisible = false;
            //LoadingVisbile = false;


        }
    }
}