using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;


namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class signUp : ContentPage
    {
        /*page to add new member */
        class User : INotifyPropertyChanged
        {
            private string id;
            public string ID
            {
                get { return id; }
                set
                {
                    id = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }
            }
            private string name;
            public string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }

            private string username;
            public string Username
            {
                get { return username; }
                set
                {
                    username = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                }
            }
            private string gender;
            public string Gender
            {
                get { return gender; }
                set
                {
                    gender = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Gender"));
                }
            }

            private string password;
            public string Password
            {
                get { return password; }
                set
                {
                    password = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                }
            }
            private string age;
            public string Age
            {
                get { return age; }
                set
                {
                    age = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Age"));
                }
            }
            private string type;
            public string Type
            {
                get { return type; }
                set
                {
                    type = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                }
            }
            private string trainer;
            public string Trainer
            {
                get { return trainer; }
                set
                {
                    trainer = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Trainer"));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

        }
        User vm;
        Dictionary<string, string> type;
        ObservableCollection<string> list_trainer_bind = new ObservableCollection<string>();
        public ObservableCollection<string> List_trainer_bind { get { return list_trainer_bind; } }
        public signUp()
        {
            InitializeComponent();
            Init_list_trainers();
            type = new Dictionary<string, string>();
            vm = new User();
            this.BindingContext = vm;
            type.Add("trainee", "0");
            type.Add("trainer", "1");
            type.Add("owner","2");
        }

        private void pickerDate_SelectedGender(object sender, EventArgs e)
        {
            string gender = genderPicker.Items[genderPicker.SelectedIndex];
            vm.Gender = gender;
        }

        private void userType_SelectedType(object sender, EventArgs e)
        {
            string typeS = (string)userType.SelectedItem;
            vm.Type = type[typeS];
            if (vm.Type == "0")
            {
                //show list of trainers
                picker_Trainer.IsVisible = true;
            }
            else
            {
                picker_Trainer.IsVisible = false;
                vm.Trainer ="0";
            }
        }
        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            int age,id;
            bool isAge_number = Int32.TryParse(vm.Age, out age);
            bool isID_number = Int32.TryParse(vm.ID, out id);
            if (vm.Name == null || vm.Password == null || vm.Username == null
                || vm.Gender == null || vm.Age == null || vm.Trainer == null
                || vm.Type == null || !isAge_number || !isID_number)
            {
                string catching_msg = "please fill in all the entries correctly";
                await App.Current.MainPage.DisplayAlert("Missing Info", catching_msg, "OK");

            }
            else
            {
                //call function app to update DB
                string parameters = "id=" + vm.ID + "&name=" + vm.Name +
                   "&username=" + vm.Username +
                   "&password=" + vm.Password + 
                   "&gender=" + vm.Gender + 
                   "&type=" + vm.Type+ 
                   "&trainer=" + vm.Trainer + 
                   "&age=" + vm.Age;
                string req = "https://gymfuctions.azurewebsites.net/api/insert_sql?query=insert_new_user&" + parameters;
                string result = Models.Connection.get_result_from_http(req, false);
                if (result != "-1")
                {
                    if(result != "isDuplicate")
                    {
                        
                        string catching_msg = "sign up succesfully";
                        await App.Current.MainPage.DisplayAlert("Success", catching_msg, "OK");

                        await App.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        string catching_msg = "there is already a user with the same ID, try a diffrent one";
                        await App.Current.MainPage.DisplayAlert("Fail", catching_msg, "OK");
                    }
                }
                else
                {
                    Console.WriteLine("the insert didnt went well");
                }
            }

        }

        private void trainer_selected(object sender, EventArgs e)
        {
            string trainer = (string)picker_Trainer.SelectedItem;
            string id_Trainer;
            if (trainer != "None")
            {
                string[] selected_Trainer_array = trainer.Split(' ');
                id_Trainer = selected_Trainer_array[selected_Trainer_array.Length - 1];
            }
            else id_Trainer = "0";

            vm.Trainer = id_Trainer;


        }
        public void Base64ToImage(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            using (var imageFile = new FileStream("C:\\Users\\ad\\Pictures", FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }
        public void Init_list_trainers()
        {
            List<Models.Trainer> list_of_trainers = new List<Models.Trainer>();
            string req = "https://gymfuctions.azurewebsites.net/api/initLIstTrainers?query=select_trainers";
            string result = Models.Connection.get_result_from_http(req, true);
            list_of_trainers = JsonConvert.DeserializeObject<List<Models.Trainer>>(result);
            foreach (Models.Trainer t in list_of_trainers)
            {
                list_trainer_bind.Add(t.Name + " " + t.Id);
               

            }
            
            list_trainer_bind.Add("None");
            picker_Trainer.ItemsSource = list_trainer_bind;

        }


    }
}