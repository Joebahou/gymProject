using MySqlConnector;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace exampleApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
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



        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
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
        public async void OnSubmit()
        {
        
            bool found = false;
            using (var conn = new MySqlConnection(Models.Connection.builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT email,password,idmember,name,type FROM gym_schema.members;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string email_DB = reader.GetString(0);
                            string password_DB = reader.GetString(1);
                            if(email == email_DB && password == password_DB)
                            {
                                found = true;
                                id_member_login = reader.GetInt32(2);
                                name_login=reader.GetString(3);
                                Ttype = reader.GetInt32(4);
                                //need to save info on app
                                Models.User.Name = name_login;
                                Models.User.Id = id_member_login;
                                Models.User.Type = Ttype;
                            }
                       

                        }
                    }



                }
                Models.User.Trainees = new List<Models.Trainee>();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmember,name FROM members WHERE trainer=@id_member;";
                    command.Parameters.AddWithValue("@id_member", Models.User.Id);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                           int id_trainee = reader.GetInt32(0);
                           string name = reader.GetString(1);
                           Models.Trainee temp = new Models.Trainee { Id = id_trainee, Name = name };
                           Models.User.Trainees.Add(temp);


                        }
                    }



                }
            }

            if (found==false)
            {
                DisplayInvalidLoginPrompt();
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new Pages.homePage());
                await App.Current.MainPage.Navigation.PopAsync();

                //await App.Current.MainPage.Navigation.PushAsync(new Page1());

            }
        }

     

     
    }
}