using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using MySqlConnector;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace exampleApp.TrainerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsNavigationTrainer : ContentPage
    {
        public static List<int> TraineesListIds { get; set; }
        public static List<string> TraineesListNames { get; set; }
        public static List<string> TraineesListGenders { get; set; }
        public static List<int> TraineesListAges { get; set; }
        
        public static int IdTrainer;
        public StatisticsNavigationTrainer()
        {
            InitializeComponent();
            IdTrainer = Models.User.Id;
            TraineesListIds = new List<int>();
            TraineesListNames = new List<string>();
            TraineesListGenders = new List<string>();
            TraineesListAges = new List<int>();
            //ConnectDataBase();
            getTrainees();
        }
        /*connect to data base*/
        MySqlConnection conn;
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

        /*This method gets the Trainees list of the specifiec trainer
         by 4 different orderd list that contains id, name, age and gender of every trainee*/

        private void getTrainees()
        {
            foreach(Models.Trainee t in Models.User.Trainees)
            {
                int id = -1;
                string name = "";
                string gender = "Unknown";
                int age = -1;

                id = t.Id;

                name = t.Name;

                gender = t.Gender;

                age = t.Age;
            
                TraineesListIds.Add(id);
                TraineesListNames.Add(name);
                TraineesListGenders.Add(gender);

                TraineesListAges.Add(age);
            }
            /*
            string cmd_text = $"select idmember, name, gender, age " +
                $"from members " +
                $"where trainer = {IdTrainer} ";            
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();            
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    long id = -1;
                    string name = "";
                    string gender = "Unknown";
                    long age = -1;
                    if (rdr[0] != DBNull.Value)
                    {
                        Console.WriteLine(rdr[0]);
                        id = rdr.GetInt32(0);
                    }
                    else { continue; }
                    if (rdr[1] != DBNull.Value)
                    {
                        Console.WriteLine(rdr[1]);
                        name = (string)rdr[1];
                    }
                    else { continue; }
                    if (rdr[2] != DBNull.Value)
                    {                        
                        gender = (string)rdr[2];
                    }
                    if (rdr[3] != DBNull.Value)
                    {                        
                        age = rdr.GetInt32(3);
                    }
                    TraineesListIds.Add((int)id);
                    TraineesListNames.Add(name);
                    TraineesListGenders.Add(gender);
                    TraineesListAges.Add((int)age);
                }
            }
            rdr.Close();
            */
        }

        private void StatsPerTrainee_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatsPerTrainee());
        }

        private void StatsPerGender_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GenderStatistics());
        }

        private void StatsPerAge_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatsPerAge());
        }
    }
}