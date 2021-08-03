using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Schedule_for_trainee : ContentPage
    {
        ObservableCollection<Schedule> list_bind = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> List_bind { get { return list_bind; } }
        public class Schedule
        {

            public string id_machine { get; set; }
            public string date_time_string { get; set; }
            public string name_machine { get; set; }
            public int available { get; set; }

            public DateTime date_time { get; set; }
            public Color color_row { get; set; }

            [JsonConstructor]
            public Schedule(int id_machine, DateTime start_time, int availabe,string name_machine)
            {
                this.id_machine = id_machine.ToString();
                this.date_time = start_time;
                this.available = available;
                this.name_machine = name_machine;

            }
          
            public Schedule()
            {

            }
        }
        public Schedule_for_trainee()
        {
            InitializeComponent();
            Init_Table_Schedule();
        }
        public void Init_Table_Schedule()
        {
            list_bind = new ObservableCollection<Schedule>();
            string parameters = "id_member=" + Models.User.Id;
            string req = "https://gymfuctions.azurewebsites.net/api/select_schedule_for_trainee?query=select_schedule_for_trainee&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            List<Schedule> trainee_schedule = JsonConvert.DeserializeObject<List<Schedule>>(result);
            foreach(Schedule s in trainee_schedule)
            {
                //int id_machine = s.id_machine;

                DateTime start_time = s.date_time;
                int working = s.available;
                string name_machine = s.name_machine;
                Schedule current;
                if (working == 0)
                {
                    current = new Schedule { color_row = Color.Yellow, id_machine = s.id_machine, date_time_string = start_time.ToString(), date_time = start_time, name_machine = name_machine + "- broken" };
                }
                else
                {
                    current = new Schedule { color_row = Color.White, id_machine = s.id_machine, date_time_string = start_time.ToString(), date_time = start_time, name_machine = name_machine };

                }

                list_bind.Add(current);
            }
            /*
            DateTime today = DateTime.Now;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
            
                using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"SELECT future_schedule_machines.id_machine,future_schedule_machines.start_time,machines.working,machines.name " +
                        "FROM future_schedule_machines,machines " +
                        "WHERE future_schedule_machines.id_machine=machines.idmachine and future_schedule_machines.id_member=@id_member and future_schedule_machines.start_time>=@today "+
                        "order by start_time;";
                        command.Parameters.AddWithValue("@id_member", Models.User.Id);
                        command.Parameters.AddWithValue("@today", today);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id_machine = reader.GetInt32(0);
                               
                                DateTime start_time = reader.GetDateTime(1);
                                int working = reader.GetInt32(2);
                                string name_machine = reader.GetString(3);
                                Schedule current;
                                if (working == 0)
                                {
                                    current = new Schedule { color_row = Color.Yellow, id_machine = id_machine.ToString(), date_time_string = start_time.ToString(), date_time = start_time, name_machine = name_machine + "- broken" };
                                }
                                else
                                {
                                    current = new Schedule { color_row = Color.White, id_machine = id_machine.ToString(), date_time_string = start_time.ToString(), date_time = start_time, name_machine = name_machine };

                                }

                                list_bind.Add(current);


                            }
                        }


                    }
                }*/
            Schedule_view.ItemsSource = list_bind;
        }

    }
}