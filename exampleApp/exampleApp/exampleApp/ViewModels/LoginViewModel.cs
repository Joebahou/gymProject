using MySqlConnector;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace exampleApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        /*model for the login */
        public Action DisplayInvalidLoginPrompt;
        public event PropertyChangedEventHandler PropertyChanged;
        public static int id_member_login=0;
        public static int Ttype= 0;
        private  string name_login;

        public  string Name_login
        {
            get { return name_login; }
            set
            {
                name_login = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name_login"));

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
        public ICommand SubmitCommand { set; get; }
        public LoginViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }
        public class User_from_sql
        {
            public int id_member { get; set; }
            public string name { get; set; }
            public int type { get; set; }
            public int age { get; set; }
            public string gender { get; set; }
            
            [JsonConstructor]
            public User_from_sql(int id_member,int age, string name,string gender,int type)
            {
                this.id_member = id_member;
                this.age = age;
                this.name = name;
                this.gender = gender;
                this.type = type;
            }

        }
        //clicked submit in the login page
        public async void OnSubmit()
        {
        
            

            string parameters = "username=" + username +
                "&password=" + password;
            string req = "https://gymfuctions.azurewebsites.net/api/login_select?query=check_login&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            User_from_sql user = JsonConvert.DeserializeObject<User_from_sql>(result);
            //check if the user is in the sql table
            if (user.id_member == -1)
            {
                DisplayInvalidLoginPrompt();
            }
        
            else
            {
                //saves the loged user's info
                id_member_login = user.id_member;
                name_login = user.name;
                Ttype = user.type;
                //need to save info on app
                Models.User.Name = name_login;
                Models.User.Id = id_member_login;
                Models.User.Type = Ttype;
                Models.User.Age = user.age;
                Models.User.Gender = user.gender;
                if (Models.User.Type == 1)
                {
                    Models.User.Trainees = new List<Models.Trainee>();
                    parameters = "id_member=" + Models.User.Id;
                    req = "https://gymfuctions.azurewebsites.net/api/login_select?query=select_trainees_for_trainer&" + parameters;
                    result = Models.Connection.get_result_from_http(req, true);
                    Models.User.Trainees = JsonConvert.DeserializeObject<List<Models.Trainee>>(result);
                 
                }
                
                
                Application.Current.MainPage = new NavigationPage(new Pages.homePage());
                await App.Current.MainPage.Navigation.PopAsync();


            }
        }

     

     
    }
}