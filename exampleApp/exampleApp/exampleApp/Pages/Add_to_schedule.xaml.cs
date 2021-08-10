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
        /*page for the trainer to pick trainee for a new schedule */
      
        public static DateTime time_to_schedule;
        public static int id_machine;
        public static string name_machine_chosen;
        public string chosen_date { get; set; }
        public string chosen_machine { get; set; }

        
        public Add_to_schedule()
        {
            InitializeComponent();
            
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
        //the trainer picked a trainee to add schedule for him
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
            bool machine_exists = false;
            string parameters = "id_member=" + id_Trainee +
                "&time_to_schedule=" + time_to_schedule.ToString() +
                "&id_machine=" + id_machine;
            string req = "https://gymfuctions.azurewebsites.net/api/check_schedule?query=check_schedule_for_trainee&" + parameters;
            String result = Models.Connection.get_result_from_http(req, true);
            Result[] reshima = JsonConvert.DeserializeObject<Result[]>(result);

            ready_to_add = reshima[0].isTrue; // if the picked trainee has othe schedule on the same time
            other_already_taken = reshima[1].isTrue; // if the machine is already taken on that time
            machine_exists = reshima[2].isTrue;

            if (machine_exists)
            {
                if (ready_to_add)
                {
                    if (!other_already_taken)
                    {
                        await add_new_schedule(id_Trainee, trainee_name);

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

                    await App.Current.MainPage.DisplayAlert("Error", "The trainee has different schedule on that time, try again", "OK");
                }
            }
            else
            {
                string caching_msg = "The machine has been deleted,please choose again";
                await App.Current.MainPage.DisplayAlert("Error", caching_msg, "OK");
                await App.Current.MainPage.Navigation.PopAsync();
                await App.Current.MainPage.Navigation.PopAsync();

            }
           
     


        }

        // insert new schedule to the sql table with http request to function app
        private async Task add_new_schedule(int id_Trainee, string name_trainee )
        {
            string parameters = "id_machine=" + id_machine.ToString() + "&id_Trainee=" + id_Trainee + "&time_to_schedule=" + time_to_schedule.ToString() + "&name_trainee=" + name_trainee;
            string req= "https://gymfuctions.azurewebsites.net/api/insert_sql?query=insert_new_schedule&"+parameters;
            System.Net.WebRequest request = System.Net.WebRequest.Create(req);
            System.Net.WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
            response.Close();
            reader.Close();
            if(result== "machine_not_exists")
            {
                string caching_msg = "The machine has been deleted,please choose again";
                await App.Current.MainPage.DisplayAlert("Error", caching_msg, "OK");
                await App.Current.MainPage.Navigation.PopAsync();
                await App.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                if (result == "1")
                {
                    string caching_msg = "you succesfully added in date " + time_to_schedule.ToString() + ",machine " + id_machine + " with trainee " + name_trainee;
                    await App.Current.MainPage.DisplayAlert("Update Schedule", caching_msg, "OK");
                    await App.Current.MainPage.Navigation.PopAsync();
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    Console.WriteLine("the insert didnt went well");
                }
            }
         
          
           
            
            

        }
        //Init the picker of the trainees
        private void Init_picker_trainee()
        {
            foreach(Models.Trainee t in Models.User.Trainees)
            {
                picker_Trainee.Items.Add(t.Name+" "+t.Id);
            }
        }
        
      

    }
}