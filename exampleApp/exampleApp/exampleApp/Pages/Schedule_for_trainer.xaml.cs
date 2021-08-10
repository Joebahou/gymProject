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
    public partial class Schedule_for_trainer : ContentPage
        /*page for the schedule of the trainer's trainees for the loged in trainer */
    {
        ObservableCollection<Schedule> list_bind = new ObservableCollection<Schedule>();
        public ObservableCollection<Schedule> List_bind { get { return list_bind; } }

        ObservableCollection<string> list_trainee_bind = new ObservableCollection<string>();
        public ObservableCollection<string> List_trainee_bind { get { return list_trainee_bind; } }
       
        public ObservableCollection<string> List_trainee_filter_bind { get { return list_trainee_filter_bind; } } 
        ObservableCollection<string> list_trainee_filter_bind = new ObservableCollection<string>();//list for the picker to filter.
   


        public Schedule selected_schedue_edit;
        public Dictionary<int, Models.Machine> dict_machines = new Dictionary<int, Models.Machine>();
        public string Selected_trainee { get; set; }
        public Schedule_for_trainer()
        {
            InitializeComponent();
           
            Init_Table_Schedule();
            Init_list_trainee();
            
        }
        //Inits the list of trainer's trainees to the filter picker and to the edit picker.
        public void Init_list_trainee()
        {
            foreach (Models.Trainee t in Models.User.Trainees)
            {
                list_trainee_bind.Add(t.Name + " " + t.Id);
                list_trainee_filter_bind.Add(t.Name + " " + t.Id);

            }
            picker_Trainee.ItemsSource = list_trainee_bind;
            list_trainee_filter_bind.Add("See All");
            picker_Trainee_Filter.ItemsSource = list_trainee_filter_bind;
         
        }
        //Loads the schedule of the trainer's trainees to the grid.
        public void Init_Table_Schedule()
        {
            list_bind= new ObservableCollection<Schedule>();
            DateTime today = DateTime.Now;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
            foreach (Models.Trainee t in Models.User.Trainees)
            {
                string parameters = "id_member=" + t.Id;
                string req = "https://gymfuctions.azurewebsites.net/api/select_schedule_for_trainee?query=select_schedule_for_trainee&" + parameters;
                string result = Models.Connection.get_result_from_http(req, true);
                List<Schedule> trainee_schedule = JsonConvert.DeserializeObject<List<Schedule>>(result);
                foreach (Schedule s in trainee_schedule)
                {
                    //int id_machine = s.id_machine.ToString();

                    DateTime start_time = s.date_time;
                    int working = s.available;
                    string name_machine = s.name_machine;
                    Schedule current;
                    if (working == 0)
                    {
                        current = new Schedule { color_row = Color.Yellow, id_machine =s.id_machine, id_trainee = t.Id.ToString(), date_time_string = start_time.ToString(), name_trainee = t.Name, date_time = start_time, name_machine = name_machine + "- broken" };
                    }
                    else
                    {
                        current = new Schedule { color_row = Color.White, id_machine = s.id_machine, id_trainee = t.Id.ToString(), date_time_string = start_time.ToString(), name_trainee = t.Name, date_time = start_time, name_machine = name_machine };

                    }

                    list_bind.Add(current);
                }
               


            }
            Schedule_view.ItemsSource = list_bind;
        }

       
    
        public class Schedule
        {
            public string id_trainee { get; set; }
            public string name_trainee { get; set; }
            public string id_machine { get; set; }
            public string date_time_string { get; set; }
            public string name_machine { get; set; }
            public int available { get; set; }

            public DateTime date_time { get; set; }
            public Color color_row { get; set; }
            [JsonConstructor]
            public Schedule(int id_machine, DateTime start_time, int availabe, string name_machine)
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
      
        
      
        //clicked to delete a schedule
        public void delete_clicked_image(Object sender, System.EventArgs e)
        {
            Image image_delete = (Image)sender;
            Schedule schedule = image_delete.BindingContext as Schedule;
            string parameters = "id_machine=" + schedule.id_machine +
                "&id_member=" + schedule.id_trainee +
                "&start_time=" + schedule.date_time.ToString();
            string req = "https://gymfuctions.azurewebsites.net/api/delete_sql?query=delete_schedule&" + parameters;
            string result = Models.Connection.get_result_from_http(req,false);
            if (result == "1")
            {
                list_bind.Remove(schedule);
                Schedule_view.ItemsSource = list_bind;
            }
            else
            {
                Console.WriteLine("delte machine didnt wen well");
            }
           

        
        }
        
        public void edit_clicked(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = true;
            popupEdit.Focus();
            Button thebutton = (Button)sender;
            Schedule schedule = thebutton.BindingContext as Schedule;
            Selected_trainee = schedule.name_trainee + " " + schedule.id_trainee;
            selected_schedue_edit = schedule;
            picker_Trainee.SelectedItem = Selected_trainee;
        }
        //clicked to edit the trainee of the schedule.
        // a pop up for the edit will be visble.
        public void edit_clicked_image(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = true;
            popupEdit.Focus();
            Image image_edit = (Image)sender;
            Schedule schedule = image_edit.BindingContext as Schedule;
            Selected_trainee = schedule.name_trainee + " " + schedule.id_trainee;
            selected_schedue_edit = schedule;
            picker_Trainee.SelectedItem = Selected_trainee;
        }
        //clicked to cancle the edit window.
        public void click_button_cancel(Object sender, System.EventArgs e)
        {
            popupEdit.IsVisible = false;
        
        }
        //clicked to filter the shown schedule with a trainee
        public async void click_button_filter(Object sender, System.EventArgs e)
        {
            if (picker_Trainee_Filter.SelectedIndex == -1)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Choose Trainee", "OK");

            }
            else
            {
                list_bind = new ObservableCollection<Schedule>();
                DateTime today = DateTime.Now;
                today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                string selected_Trainee =picker_Trainee_Filter.SelectedItem.ToString();
                if(selected_Trainee=="See All")
                {
                    Init_Table_Schedule();
                }
                else
                {
                    string[] selected_Trainee_array = selected_Trainee.Split(' ');
                    string trainee_name = "";
                    for (int i = 0; i < selected_Trainee_array.Length - 2; i++)
                    {
                        trainee_name += selected_Trainee_array[i] + " ";
                    }
                    trainee_name += selected_Trainee_array[selected_Trainee_array.Length - 2];
                    int id_Trainee = Int32.Parse(selected_Trainee_array[selected_Trainee_array.Length - 1]);

                    string parameters = "id_member=" + id_Trainee;
                    string req = "https://gymfuctions.azurewebsites.net/api/select_schedule_for_trainee?query=select_schedule_for_trainee&" + parameters;
                    string result = Models.Connection.get_result_from_http(req, true);
                    List<Schedule> trainee_schedule = JsonConvert.DeserializeObject<List<Schedule>>(result);
                    foreach(Schedule s in trainee_schedule)
                    {
                        string id_machine = s.id_machine;
                        int id_member = id_Trainee;
                        DateTime start_time = s.date_time;
                        int working = s.available;
                        string name_machine = s.name_machine;
                        Schedule current;
                        if (working == 0)
                        {
                            current = new Schedule { color_row = Color.Yellow, id_machine = id_machine.ToString(), id_trainee = id_member.ToString(), date_time_string = start_time.ToString(), name_trainee = trainee_name, date_time = start_time, name_machine = name_machine + "- broken" };
                        }
                        else
                        {
                            current = new Schedule { color_row = Color.White, id_machine = id_machine.ToString(), id_trainee = id_member.ToString(), date_time_string = start_time.ToString(), name_trainee = trainee_name, date_time = start_time, name_machine = name_machine };

                        }

                        list_bind.Add(current);
                    }
                   


                    Schedule_view.ItemsSource = list_bind;

                }
               
            }

        }
        //clicked to save the edit.
        //the function checks if the new trainee has other schedule on the same time.
        public async void click_button_save(Object sender, System.EventArgs e)
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
            if (id_Trainee == Int32.Parse(selected_schedue_edit.id_trainee))
            {
                await App.Current.MainPage.DisplayAlert("No change", "It's the same trainee, try again", "OK");
            }
            else
            {
              

                string parameters = "id_machine=" + selected_schedue_edit.id_machine +
                    "&new_id_member=" + id_Trainee +
                    "&old_id_member=" + selected_schedue_edit.id_trainee +
                    "&start_time=" + selected_schedue_edit.date_time.ToString() +
                    "&new_name_member=" + trainee_name;
                string req = "https://gymfuctions.azurewebsites.net/api/update_schedule_of_trainer?query=update_new_schedule&" + parameters;
                string result = Models.Connection.get_result_from_http(req, true);
                if(result== "machine_not_exists")
                {
                    string caching_msg = "The machine has been deleted,please choose again";
                    await App.Current.MainPage.DisplayAlert("Error", caching_msg, "OK");
                    await App.Current.MainPage.Navigation.PopAsync();
                   
                }
                else
                {
                    if (result == "trainee is not free")
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "The trainee has different schedult on that time, try again", "OK");
                    }
                    else
                    {
                        if (result == "1")
                        {
                            list_bind.Remove(selected_schedue_edit);
                            selected_schedue_edit.id_trainee = id_Trainee.ToString();
                            selected_schedue_edit.name_trainee = trainee_name;
                            list_bind.Add(selected_schedue_edit);
                            Schedule_view.ItemsSource = list_bind;
                            popupEdit.IsVisible = false;

                        }
                        else
                        {
                            Console.WriteLine("update schedule didnt went well");

                        }
                    }

                }
                


         



            }



        }
    }
}