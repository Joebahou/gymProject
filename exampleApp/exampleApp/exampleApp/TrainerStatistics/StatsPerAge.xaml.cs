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
    public partial class StatsPerAge : ContentPage
    {
        public StatsPerAge()
        {
            InitializeComponent();
            showAgeCount();
        }

        /*This method uses the lists built in the statisticsNavigationTrainer*/
        private void showAgeCount()
        {
            int Count1625 = 0;
            int Count2635 = 0;
            int Count3645 = 0;
            int Count4655 = 0;
            int Count5665 = 0;
            int Count6675 = 0;
            int Count7685 = 0;

            foreach (int age in StatisticsNavigationTrainer.TraineesListAges)
            {
                if (age >= 16 && age <= 25 ) { Count1625++; }
                if (age >= 26 && age <= 35 ) { Count2635++; }
                if (age >= 36 && age <= 45) { Count3645++; }
                if (age >= 46 && age <= 55) { Count4655++; }
                if (age >= 56 && age <= 65) { Count5665++; }
                if (age >= 66 && age <= 75) { Count6675++; }
                if (age >= 76 && age <= 85) { Count7685++; }

            }
            List<ChartEntry> entries = new List<ChartEntry>();
            ChartEntry Entry1 = new ChartEntry(Count1625)
            {
                Label = "16-25",
                TextColor = SKColor.Parse("#f542ad"),
                Color = SKColor.Parse("#f542ad")
            };
            ChartEntry Entry2 = new ChartEntry(Count2635)
            {
                Label = "26-35",
                TextColor = SKColor.Parse("#96f542"),
                Color = SKColor.Parse("#96f542")
            };
            ChartEntry Entry3 = new ChartEntry(Count3645)
            {
                Label = "36-45",
                TextColor = SKColor.Parse("#f56342"),
                Color = SKColor.Parse("#f56342")
            };
            ChartEntry Entry4 = new ChartEntry(Count4655)
            {
                Label = "46-55",
                TextColor = SKColor.Parse("#000000"),
                Color = SKColor.Parse("#000000")
            };
            ChartEntry Entry5 = new ChartEntry(Count5665)
            {
                Label = "56-65",
                TextColor = SKColor.Parse("#42f5cb"),
                Color = SKColor.Parse("#42f5cb")
            };
            ChartEntry Entry6 = new ChartEntry(Count6675)
            {
                Label = "66-75",
                TextColor = SKColor.Parse("#429ef5"),
                Color = SKColor.Parse("#429ef5")
            };
            ChartEntry Entry7 = new ChartEntry(Count7685)
            {
                Label = "76-85",
                TextColor = SKColor.Parse("#a142f5"),
                Color = SKColor.Parse("#a142f5")
            };

            entries.Add(Entry1);
            entries.Add(Entry2);
            entries.Add(Entry3);
            entries.Add(Entry4);
            entries.Add(Entry5);
            entries.Add(Entry6);
            entries.Add(Entry7);
            chartViewPie.Chart = new PieChart
            {
                Entries = entries,
                LabelTextSize = 40,
                BackgroundColor = SKColor.Parse("#00ffffff"),
            };
        }
    }
}