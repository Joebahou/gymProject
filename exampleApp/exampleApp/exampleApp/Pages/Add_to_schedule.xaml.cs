using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Add_to_schedule : ContentPage
    {
        //MySqlConnection conn;
        public static DateTime time_to_schedule;
        public static int id_machine;
        public static string name_machine_chosen;
        public string chosen_date { get; set; }
        public string chosen_machine { get; set; }

        public Add_to_schedule()
        {
            InitializeComponent();
            //ConnectDataBase();
            Init_picker_trainee();
            chosen_date = "the date you chose: " + time_to_schedule.ToString();
            chosen_machine = "the machine you chose: " + name_machine_chosen;
            OnPropertyChanged("chosen_date");
            OnPropertyChanged("chose_machine");
            BindingContext = this;
            
        }
        public class Result
        {
           public bool isTrue { get; set; }
        }
        public class L
        {
           public Result[] results { get; set; }
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
            bool other_already_taken = false;
            string parameters = "id_member=" + id_Trainee +
                "&time_to_schedule=" + time_to_schedule.ToString() +
                "&id_machine=" + id_machine;
            string req = "https://gymfuctions.azurewebsites.net/api/check_schedule?query=check_schedule_for_trainee&" + parameters;
            System.Net.WebRequest request = System.Net.WebRequest.Create(req);
            request.ContentType = "application/json; charset=utf-8";
            System.Net.WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
            Result[] reshima = JsonConvert.DeserializeObject<Result[]>(result);

            ready_to_add = reshima[0].isTrue;
            other_already_taken = reshima[1].isTrue;


            /*
            using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
            {
                conn.Open();
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
                using (MySqlCommand command = conn.CreateCommand())
                {

                    command.CommandText = @"SELECT * FROM future_schedule_machines WHERE id_machine=@id_machine and start_time=@time_to_schedule;";
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@time_to_schedule", time_to_schedule);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            other_already_taken = true;
                        }
                    }


                }

            }*/

            if (ready_to_add)
            {
                if (!other_already_taken)
                {
                    await add_new_schedule(id_Trainee,trainee_name);

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "The schedule already taken. pleae refresh the page", "OK");
                    await App.Current.MainPage.Navigation.PopAsync();
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            }
            else
            {
                
                await App.Current.MainPage.DisplayAlert("Error", "The trainee has different schedult on that time, try again", "OK");
            }
     


        }
        private async Task add_new_schedule(int id_Trainee, string name_trainee )
        {
            string parameters = "id_machine=" + id_machine.ToString() + "&id_Trainee=" + id_Trainee + "&time_to_schedule=" + time_to_schedule.ToString() + "&name_trainee=" + name_trainee;
            string req= "https://gymfuctions.azurewebsites.net/api/insert_sql?query=insert_new_schedule&"+parameters;
            System.Net.WebRequest request = System.Net.WebRequest.Create(req);
            System.Net.WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
            if (result == "1")
            {
                string caching_msg = "you succesfully added in date " + time_to_schedule.ToString() + ",machine " + id_machine + " with trainee " + name_trainee;
                await App.Current.MainPage.DisplayAlert("Update Schedule", caching_msg, "OK");
                await App.Current.MainPage.Navigation.PopAsync();
                await App.Current.MainPage.Navigation.PopAsync();
            }
            /*
            using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {

                    command.CommandText = @"INSERT INTO future_schedule_machines(id_machine,id_member,start_time,name_member) VALUES(@id_machine,@id_member,@start_time,@name_member);";
                    command.Parameters.AddWithValue("@id_machine", id_machine);
                    command.Parameters.AddWithValue("@id_member", id_Trainee);
                    command.Parameters.AddWithValue("@start_time", time_to_schedule);
                    command.Parameters.AddWithValue("@name_member", name_trainee);
                    command.ExecuteNonQuery();



                }
            }*/
            else
            {
                Console.WriteLine("the insert didnt went well");
            }
            
            

        }
        private void Init_picker_trainee()
        {
            foreach(Models.Trainee t in Models.User.Trainees)
            {
                picker_Trainee.Items.Add(t.Name+" "+t.Id);
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
        }
        */

    }
}