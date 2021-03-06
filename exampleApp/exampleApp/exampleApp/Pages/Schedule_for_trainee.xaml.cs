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
        /*page of the schedule of the trainee that loged in */
    {
        ObservableCollection<Schedule> list_bind = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> List_bind { get { return list_bind; } }
        /*class for the list to bind the gris schedule */
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
        //Inits the schedule of the trainee.
        //the data is loaded to the grid
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
          
            Schedule_view.ItemsSource = list_bind;
        }

    }
}