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
        private void pickerDate_SelectedIndexChanged(object sender, EventArgs e)
        {
           
    
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
                    Server = "gymserver.mysql.database.azure.com",
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