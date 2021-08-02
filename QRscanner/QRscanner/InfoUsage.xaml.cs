using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoUsage : ContentPage
    {
        DateTime submiting_time = DateTime.Now;
        DateTime nearest_future_schedule= new DateTime(2024, 7, 19, 0, 0, 0);
        TimeSpan hour_1 = new TimeSpan(1, 0, 0);
        string catching_msg;
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "gymservernew.mysql.database.azure.com",
            Database = "gym_schema",
            UserID = "gymAdmin",
            Password = "gym1Admin",
            SslMode = MySqlSslMode.Required,
        };
        public InfoUsage()
        {
            InitializeComponent();
            var vm = new Models.Usage();
            this.BindingContext = vm;
            


        }

        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            
            activityIndicator.IsVisible = true;
            await Task.Delay(10);
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = @"select min(start_time) as nearestFutureSchedule from gym_schema.future_schedule_machines where start_time> @submit_time and id_machine=@id_machine and id_member!=@id_member;";
                    command.Parameters.AddWithValue("@id_machine", MainPage.id_machine);
                    command.Parameters.AddWithValue("@id_member", StartScanPage.id_member);
                    command.Parameters.AddWithValue("@submit_time", submiting_time);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())

                        {
                            if (!reader.IsDBNull(0))

                                nearest_future_schedule = reader.GetDateTime(0);

                        }
                    }

                }
            }
            if((nearest_future_schedule-submiting_time)<hour_1)
            {
                catching_msg = "This machine has been booked by another trainer in about "+ (nearest_future_schedule - submiting_time).Minutes+" minutes";
                await App.Current.MainPage.DisplayAlert("Reminder", catching_msg, "I'll finish in time");
            }
            await Task.Delay(2000);
            activityIndicator.IsVisible = false;
            await App.Current.MainPage.Navigation.PopAsync();
            await App.Current.MainPage.Navigation.PopAsync();
            


        }
        


    }
}