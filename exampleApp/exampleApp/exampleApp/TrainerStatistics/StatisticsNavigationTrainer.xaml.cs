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
       
            getTrainees();
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