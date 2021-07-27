
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Windows.Input;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllSchedule : ContentPage
    {
        ObservableCollection<Temp> list_bind = new ObservableCollection<Temp>();
        public ObservableCollection<Temp> List_bind { get { return list_bind; } }
        public ICommand Add_schedule_Command { get; set; }
        private Boolean loadingVisbile;
        public Boolean LoadingVisbile { get { return loadingVisbile; }
            set { loadingVisbile = value; }
        }

        List<Models.Machine> machines;
        Dictionary<int, Models.Machine> dict_machines;
        Dictionary<string, int> times = new Dictionary<string, int>();
        MySqlConnection conn;
        ActivityIndicator activityIndicator;
        string selected_date_string;
        public  AllSchedule()
        {
            InitializeComponent();

            Peopleview.ItemsSource = list_bind;
            ConnectDataBase();
            pickerDateInit();
            machines = new List<Models.Machine>();
            dict_machines = new Dictionary<int, Models.Machine>();
            get_machines();
            Init_times();
            Init_schedule_Table();
            
      
        }
        

        public void Init_schedule_Table()
        {
            scheduleTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//hours
            scheduleTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12, GridUnitType.Star) });//empty
            activityIndicator = new ActivityIndicator { IsRunning = true };
            activityIndicator.IsVisible = false;
            for (int i = 1; i < 38; i++)
            {
                scheduleTable.ColumnDefinitions.Add(new ColumnDefinition{Width= new GridLength(11, GridUnitType.Star) });

            }
            var label = new Label
            {
               
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontSize=27
            };
            scheduleTable.Children.Add(label, 0, 0);

            foreach (string time in times.Keys){
                label = new Label
                {
                    Text =time,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 24
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
        private  void get_machines()
        {
                using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"SELECT * FROM machines;";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while ( reader.Read())
                        {
                            int id_machine = reader.GetInt32(0);
                            string name= reader.GetString(1);
                            Models.Machine temp = new Models.Machine(name,id_machine);
                            machines.Add(temp);
                            dict_machines[id_machine] = temp;


                        }
                    }





                }

        }
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
        }
   
        public void Add_schedule_button_command(object obj)
        {

        }
        public void handle_clicked(Object sender, System.EventArgs e)
        {
            Button thebutton = (Button)sender;
            int col = Grid.GetColumn(thebutton) - 1;
            int hour=(col*20)/60 +8;
            int minutes = (col * 20) % 60;
            string time = "";
            if (hour< 10)
            {
                if (minutes==0)
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
                if (minutes==0)
                {
                    time = hour + ":00";
                }
                else
                {
                    time = hour.ToString() + ":" + minutes.ToString();
                }
            }
            DateTime time_and_date = Convert.ToDateTime(selected_date_string +" "+ time);
            Add_to_schedule.time_to_schedule = time_and_date;
            Add_to_schedule.id_machine = (int)thebutton.CommandParameter;
            Navigation.PushAsync(new Add_to_schedule());

        }
        public class Temp
        {
            public string[] Li { get; set; }
            public bool[] button_arr { get; set; }
            public int id_machine { get; set; }
        }
        private void pickerDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            activityIndicator.IsVisible =true ;
            //LoadingVisbile = true;
            selected_date_string = pickerDate.SelectedItem.ToString();
            DateTime selected_date = Convert.ToDateTime(selected_date_string);
            list_bind = new ObservableCollection<Temp>();
            Peopleview.ItemsSource = list_bind;
            foreach(Models.Machine machine in machines)
            {
                machine.Init_schedule();

            }
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"SELECT id_machine,id_member,start_time,name_member FROM future_schedule_machines WHERE start_time>=@selected_date AND start_time<=@tomorrow;";
                command.Parameters.AddWithValue("@selected_date", selected_date);
                command.Parameters.AddWithValue("@tomorrow", selected_date.AddDays(1.0));


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id_machine = reader.GetInt32(0);
                        int id_member = reader.GetInt32(1);
                        DateTime start_time = reader.GetDateTime(2);
                        string name_member = reader.GetString(3);
                        string hour= start_time.ToString("HH:mm");
                        int index = times[hour];
                        dict_machines[id_machine].schedule_machine[index] = name_member;
                    


                    }
                }



            }
           
            
            foreach(Models.Machine machine in machines)
            { 
                bool[] button_arr = new bool[38];
                button_arr[0] = false;
                for(int i = 1; i < 38; i++)
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
                list_bind.Add(new Temp { Li = machine.schedule_machine,button_arr=button_arr ,id_machine=machine.Id_machine});
            }
            //LoadingVisbile = false;
            activityIndicator.IsVisible = false;

        }
    }
}