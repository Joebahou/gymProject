using MySqlConnector;
using PowerArgs.Cli;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;
using Xamarin.Forms.Maps;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using System.ComponentModel;

namespace exampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public static HubConnection connection;
        private string name_log;
        public string Name_log
        {
            get { return name_log; }
            set
            {
                name_log = value;
               // PropertyChanged(this, new PropertyChangedEventArgs("Name_login"));

            }
        }

        public Page1()
        {
            InitializeComponent();
            Name_log = Models.User.Name;
            BindingContext = this;

            
            set_color();
          

        }
        private void update_circle(int id_machine, int taken)
        {
            if (id_machine == 1)
            {
                if (taken == 1)
                {
                    m_1.Fill = new SolidColorBrush(Color.Red);
                }
                else
                {
                    m_1.Fill = new SolidColorBrush(Color.Green);
                }
            }
            if (id_machine == 7)
            {
                if (taken == 1)
                {
                    m_7.Fill = new SolidColorBrush(Color.Red);
                }
                else
                {
                    m_7.Fill = new SolidColorBrush(Color.Green);
                }
            }

        }
        private async void  set_color()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://gymfuctions.azurewebsites.net/api")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int[]>("newMessage", (msgupdate) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                   
                        int id_machine = msgupdate[0];
                        int taken = msgupdate[1];
                        update_circle(id_machine, taken);

                    


                });
            });
            //await  connection.StartAsync();


            var builder = new MySqlConnectionStringBuilder
            {
                Server = "gymserver.mysql.database.azure.com",
                Database = "gym_schema",
                UserID = "gymAdmin",
                Password = "gym1Admin",
                SslMode = MySqlSslMode.Required,
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                 conn.Open();
                using (var command = conn.CreateCommand())
                {
                    
                    command.CommandText = @"SELECT idmachine,taken FROM gym_schema.machines;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id_machine = reader.GetInt32(0);
                            int taken = reader.GetInt32(1);
                            update_circle(id_machine, taken);


                        }
                    }
              


                }
            }
            await connection.StartAsync();

        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Title", "Hello world", "ok");
           
        }
        async void OnLogout_Clicked(object sender, EventArgs e)
        {

            Navigation.InsertPageBefore(new Pages.LoginPage(), this);
            await Navigation.PopAsync();

        }
    }
}