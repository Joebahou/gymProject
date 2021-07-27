using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Schedule_for_trainer : ContentPage
    {
        ObservableCollection<Schedule> list_bind = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> List_bind { get { return list_bind; } }

        ObservableCollection<string> list_trainee_bind = new ObservableCollection<string>();
        public ObservableCollection<string> List_trainee_bind { get { return list_trainee_bind; } }
        MySqlConnection conn;


        public Schedule selected_schedue_edit;
        public Dictionary<int, Models.Machine> dict_machines = new Dictionary<int, Models.Machine>();
        public string Selected_trainee { get; set; }
        public Schedule_for_trainer()
        {
            InitializeComponent();
            ConnectDataBase();
            get_machines();
            Init_Table_Schedule();
            Init_list_trainee();
            
        }
        public void Init_list_trainee()
        {
            foreach (Models.Trainee t in Models.User.Trainees)
            {
                list_trainee_bind.Add(t.Name + " " + t.Id);
            }
            picker_Trainee.ItemsSource = list_trainee_bind;

        }
        public void Init_Table_Schedule()
        {
            list_bind= new ObservableCollection<Schedule>();
            DateTime today = DateTime.Now;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
            foreach (Models.Trainee t in Models.User.Trainees)
            {
                using (MySqlCommand command = conn.CreateCommand())
                {

                    command.CommandText = @"SELECT id_machine,id_member,start_time FROM future_schedule_machines WHERE id_member=@id_member and start_time>=@today;";
                    command.Parameters.AddWithValue("@id_member",t.Id);
                    command.Parameters.AddWithValue("@today",today);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id_machine = reader.GetInt32(0);
                            int id_member = reader.GetInt32(1);
                            DateTime start_time = reader.GetDateTime(2);
                            Schedule current = new Schedule { id_machine = id_machine.ToString(), id_trainee = id_member.ToString(), date_time_string = start_time.ToString(), name_trainee = t.Name, date_time = start_time, name_machine = dict_machines[id_machine].Name };
                            list_bind.Add(current);

                          
                        }
                    }


                }

            }
            Schedule_view.ItemsSource = list_bind;
        }

        private void get_machines()
        {
            dict_machines = new Dictionary<int, Models.Machine>();
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"SELECT * FROM machines;";
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id_machine = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        Models.Machine temp = new Models.Machine(name, id_machine);
                        dict_machines[id_machine] = temp;


                    }
                }
            }

        }
        public class Schedule
        {
            public string id_trainee { get; set; }
            public string name_trainee { get; set; }
            public string id_machine { get; set; }
            public string date_time_string { get; set; }
            public string name_machine { get; set; }

            public DateTime date_time { get; set; }
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
        public void delete_clicked(Object sender, System.EventArgs e)
        {
            Button thebutton = (Button)sender;
            Schedule schedule = thebutton.BindingContext as Schedule;
            int i = 0;
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"DELETE FROM future_schedule_machines WHERE id_machine=@id_machine and id_member=@id_member and start_time=@start_time;";
                command.Parameters.AddWithValue("@id_machine", Int32.Parse(schedule.id_machine));
                command.Parameters.AddWithValue("@id_member", Int32.Parse(schedule.id_trainee));
                command.Parameters.AddWithValue("@start_time",schedule.date_time);
                command.ExecuteNonQuery();


            }
            list_bind.Remove(schedule);
            Schedule_view.ItemsSource = list_bind;
        }
        public void delete_clicked_image(Object sender, System.EventArgs e)
        {
            Image image_delete = (Image)sender;
            Schedule schedule = image_delete.BindingContext as Schedule;
            int i = 0;
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"DELETE FROM future_schedule_machines WHERE id_machine=@id_machine and id_member=@id_member and start_time=@start_time;";
                command.Parameters.AddWithValue("@id_machine", Int32.Parse(schedule.id_machine));
                command.Parameters.AddWithValue("@id_member", Int32.Parse(schedule.id_trainee));
                command.Parameters.AddWithValue("@start_time", schedule.date_time);
                command.ExecuteNonQuery();


            }
            list_bind.Remove(schedule);
            Schedule_view.ItemsSource = list_bind;
        }
        public void edit_clicked(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = true;
            Button thebutton = (Button)sender;
            Schedule schedule = thebutton.BindingContext as Schedule;
            Selected_trainee = schedule.name_trainee + " " + schedule.id_trainee;
            selected_schedue_edit = schedule;
            picker_Trainee.SelectedItem = Selected_trainee;
        }
        public void edit_clicked_image(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = true;
            Image image_edit = (Image)sender;
            Schedule schedule = image_edit.BindingContext as Schedule;
            Selected_trainee = schedule.name_trainee + " " + schedule.id_trainee;
            selected_schedue_edit = schedule;
            picker_Trainee.SelectedItem = Selected_trainee;
        }
        public void click_button_cancel(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = false;
        
        }
        public void click_button_save(Object sender, System.EventArgs e)
        {
            string selected_Trainee = picker_Trainee.SelectedItem.ToString();
            string[] selected_Trainee_array = selected_Trainee.Split(' ');
            string trainee_name = "";
            for(int i = 0; i < selected_Trainee_array.Length - 2; i++)
            {
                trainee_name += selected_Trainee_array[i] + " ";
            }
            trainee_name += selected_Trainee_array[selected_Trainee_array.Length - 2];
            int id_Trainee = Int32.Parse(selected_Trainee_array[selected_Trainee_array.Length - 1]);
            //update the schedule table
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"UPDATE future_schedule_machines SET id_member=@new_id_member,name_member=@new_name_member WHERE id_member=@old_id_member and id_machine=@id_machine and start_time=@start_time;";
                command.Parameters.AddWithValue("@id_machine", Int32.Parse(selected_schedue_edit.id_machine));
                command.Parameters.AddWithValue("@new_id_member", id_Trainee);
                command.Parameters.AddWithValue("@old_id_member", Int32.Parse(selected_schedue_edit.id_trainee));
                command.Parameters.AddWithValue("@start_time", selected_schedue_edit.date_time);
                command.Parameters.AddWithValue("@new_name_member", trainee_name);
                command.ExecuteNonQuery();


            }
            list_bind.Remove(selected_schedue_edit);
            selected_schedue_edit.id_trainee = id_Trainee.ToString();
            selected_schedue_edit.name_trainee = trainee_name;
            list_bind.Add(selected_schedue_edit); 
            Schedule_view.ItemsSource = list_bind;
            popupEdit.IsVisible = false;
           
        }
    }
}