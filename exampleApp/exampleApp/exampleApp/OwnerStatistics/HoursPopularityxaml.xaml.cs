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
using Color = Xamarin.Forms.Color;
using Newtonsoft.Json;

namespace exampleApp.OwnerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HoursPopularityxaml : ContentPage
    {
        private DateTime DateStart = DateTime.Now;
        private DateTime DateEnd = DateTime.Now;
        public HoursPopularityxaml()
        {
            InitializeComponent();
          
        }
        private MySqlConnection conn;
      
        public class popularityHours_from_sql{
            public string date { get; set; }
            public long count { get; set; }

            [JsonConstructor]
            public popularityHours_from_sql(string date,long count)
            {
                this.date = date;
                this.count = count;
            }
        }
        /*This method gets the popularity of every hour exists in the usage database
         and present it in the microchart*/
        private void HoursPop()
        {
            string start_date = DateStart.ToString("yyyy-MM-dd");
            string end_date = DateEnd.ToString("yyyy-MM-dd");

            string parameters = "start_date=" + start_date +
                "&end_date=" + end_date;
            string req = "https://gymfuctions.azurewebsites.net/api/select_hour_popularity?query=select_popular_hours&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            List<popularityHours_from_sql> hours = JsonConvert.DeserializeObject<List<popularityHours_from_sql>>(result);
            List<Tuple<string, long>> HourNumUses = new List<Tuple<string, long>>();
            foreach(popularityHours_from_sql h in hours)
            {
                long count = h.count;
                string date = h.date; ;
                HourNumUses.Add(Tuple.Create(date, count));
            }
          
            List<ChartEntry> entries = new List<ChartEntry>();
            int cnt = 0;            
            foreach (Tuple<string, long> tuple in HourNumUses)
            {
                //Random randomGen = new Random();
                //string color_name = String.Format("#{0:X6}", randomGen.Next(0x808080) & 0x7E7E7E);
                Color c = ClassColor.ListColors[cnt];
                ChartEntry ce = new ChartEntry(tuple.Item2)
                {
                    Label = "hour: " + tuple.Item1,                    
                    TextColor = SkiaSharp.SKColor.Parse(c.ToHex()),                    
                    Color = SkiaSharp.SKColor.Parse(c.ToHex())
                };
                Console.WriteLine(ce.Label);
                entries.Add(ce);
                cnt++;
            }
            chartViewPie.Chart = new RadarChart
            {
                Entries = entries,              
                LabelTextSize = 30,
                PointSize = 20,
                PointMode = PointMode.Circle,
                BackgroundColor = SKColor.Parse("#00ffffff")
            };
        }
        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateStart = DatePickerStart.Date;
        }

        private void DatePickerEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateEnd = DatePickerEnd.Date;
        }
        private void ButtonShowPop_Clicked(object sender, EventArgs e)
        {
            HoursPop();
        }
    }
}