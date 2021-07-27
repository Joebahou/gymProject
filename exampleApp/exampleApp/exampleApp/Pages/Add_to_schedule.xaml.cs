using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Add_to_schedule : ContentPage
    {
        MySqlConnection conn;
        public static DateTime time_to_schedule;
        public static int id_machine;
        public Add_to_schedule()
        {
            InitializeComponent();
            ConnectDataBase();
            Init_picker_trainee();
        }
        private async void picker_Trainee_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_Trainee = picker_Trainee.SelectedItem.ToString();
            string[] selected_Trainee_array = selected_Trainee.Split(' ');
            string trainee_name = "";
            for (int i = 0; i < selected_Trainee_array.Length - 2; i++)
            {
                trainee_name += selected_Trainee_array[i] + " ";
            }
            trainee_name += selected_Trainee_array[selected_Trainee_array.Length - 2];
            int id_Trainee = Int32.Parse(selected_Trainee_array[selected_Trainee_array.Length - 1]);
            bool ready_to_add = true;
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_member=@id_member and start_time=@time_to_schedule;";
                command.Parameters.AddWithValue("@id_member", id_Trainee);
                command.Parameters.AddWithValue("@time_to_schedule", time_to_schedule);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        ready_to_add = false;
                    }
                }


            }
            if (ready_to_add)
            {
                await add_new_schedule(id_Trainee,trainee_name);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "The trainee has different schedult on that time, try again", "OK");
            }
     


        }
        private async Task add_new_schedule(int id_Trainee, string name_trainee )
        {
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"INSERT INTO future_schedule_machines(id_machine,id_member,start_time,name_member) VALUES(@id_machine,@id_member,@start_time,@name_member);";
                command.Parameters.AddWithValue("@id_machine", id_machine);
                command.Parameters.AddWithValue("@id_member", id_Trainee);
                command.Parameters.AddWithValue("@start_time", time_to_schedule);
                command.Parameters.AddWithValue("@name_member", name_trainee);
                command.ExecuteNonQuery();



            }
            string caching_msg = "you succesfully added in date " + time_to_schedule.ToString() + ",machine " + id_machine + " with trainee " +name_trainee;
            await App.Current.MainPage.DisplayAlert("Update Schedule", caching_msg, "OK");
            await App.Current.MainPage.Navigation.PopAsync();
            await App.Current.MainPage.Navigation.PopAsync();

        }
        private void Init_picker_trainee()
        {
            foreach(Models.Trainee t in Models.User.Trainees)
            {
                picker_Trainee.Items.Add(t.Name+" "+t.Id);
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

    }
}