using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts;
using SkiaSharp;

namespace exampleApp.TrainerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenderStatistics : ContentPage
    {
        public GenderStatistics()
        {
            InitializeComponent();
            showGenderCount();
        }

        /*This method uses the lists built in the statisticsNavigationTrainer*/
        private void showGenderCount()
        {
            int femaleCount = 0;
            int maleCount = 0;
            foreach (string gen in StatisticsNavigationTrainer.TraineesListGenders)
            {
                if (gen.Equals("female")) { femaleCount++; }
                if (gen.Equals("male")) { maleCount++; }
            }
            List<ChartEntry> entries = new List<ChartEntry>();
            ChartEntry femaleEntry = new ChartEntry(femaleCount)
            {
                Label = "Female",
                TextColor = SKColor.Parse("#f542ad"),
                Color = SKColor.Parse("#f542ad")
            };
            ChartEntry maleEntry = new ChartEntry(maleCount)
            {
                Label = "Male",
                TextColor = SKColor.Parse("#429ef5"),
                Color = SKColor.Parse("#429ef5")
            };
            entries.Add(femaleEntry);
            entries.Add(maleEntry);
            chartViewPie.Chart = new PieChart
            {
                Entries = entries,                
                LabelTextSize = 40,
                BackgroundColor = SKColor.Parse("#00ffffff"),
            };
        }
    }
}