using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.TrainerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPerTrainee : ContentPage
    {
        public static int idTrainee {get; set;}
        
        public StatsPerTrainee()
        {
            InitializeComponent();
            idTrainee = -1;
            pickerTrainees.ItemsSource = StatisticsNavigationTrainer.TraineesListNames;
        }
        /*This method uses the lists built in the statisticsNavigationTrainer*/
        private void pickerTrainees_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = pickerTrainees.SelectedIndex;
            Console.WriteLine("index: " + index);
            idTrainee = StatisticsNavigationTrainer.TraineesListIds[index];
        }

        private void ButtonShowTraineeProgress_Clicked(object sender, EventArgs e)
        {
            if (idTrainee == -1)
            {
                DisplayAlert("Alert", "Select Trainee", "OK");
                return;
            }
            Navigation.PushAsync(new StatisticsNavigationTrainee());
        }
    }
}