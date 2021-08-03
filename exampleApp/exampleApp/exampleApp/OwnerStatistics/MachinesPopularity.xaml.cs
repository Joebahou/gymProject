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
using Newtonsoft.Json;

namespace exampleApp.OwnerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MachinesPopularity : ContentPage
    {
        public MachinesPopularity()
        {
            InitializeComponent();
            //ConnectDataBase();            
        }

        private DateTime DateStart = DateTime.Now;
        private DateTime DateEnd = DateTime.Now;

        /*connect to dataBase*/
        private MySqlConnection conn;
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
        /*This method gets the popularity of every machine
         and present it in the microchart*/
        public class machines_popularity_from_sql
        {
            public string machinename { get; set; }
            public long machinenumuses { get; set; }

            [JsonConstructor]
            public machines_popularity_from_sql(string machinename,long machinenumuses)
            {
                this.machinename = machinename;
                this.machinenumuses = machinenumuses;
            }
        }
        private void MachinesPop()
        {
            string start_date = DateStart.ToString("yyyy-MM-dd");
            string end_date = DateEnd.ToString("yyyy-MM-dd");

            string parameters = "start_date=" + start_date +
                "&end_date=" + end_date;
            string req = "https://gymfuctions.azurewebsites.net/api/machines_popularity?query=select_popular_machines&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            List<machines_popularity_from_sql> machine_uses = JsonConvert.DeserializeObject<List<machines_popularity_from_sql>>(result);
            List<Tuple<string, long>> MachineNumUses = new List<Tuple<string, long>>();
            foreach(machines_popularity_from_sql m in machine_uses)
            {
                string machineName = m.machinename;
                Console.WriteLine(machineName);
                long machineNumUses = m.machinenumuses;
                MachineNumUses.Add(Tuple.Create(machineName, machineNumUses));

            }

            /*
            string cmd_text = $"select machines.name, count(*) " +
                $"from usage_gym, machines " +
                $"where machines.idmachine = usage_gym.idmachine " +
                $"and date(usage_gym.start) >= '{start_date}' " +
                $"and date(usage_gym.start) <= '{end_date}' " +
                $"group by usage_gym.idmachine order by count(*) desc ";
            List<Tuple<string, long>> MachineNumUses = new List<Tuple<string, long>>();            
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {                    
                    string machineName = (string)rdr[0];
                    Console.WriteLine(machineName);
                    long machineNumUses = (long)rdr[1];
                    MachineNumUses.Add(Tuple.Create(machineName, machineNumUses));                    
                }                
            }
            rdr.Close();
            */
            int count = 0;
            List<ChartEntry> entries = new List<ChartEntry>();
            foreach (Tuple<string, long> tuple in MachineNumUses)
            {                
                string machine_name = tuple.Item1.ToString();
                string new_name = machine_name.Replace('_', ' ');
                ChartEntry ce = new ChartEntry(tuple.Item2)
                {
                    Label = new_name,
                    ValueLabel = tuple.Item2.ToString(),
                    Color = SKColor.Parse("#ffca28")
                };
                Console.WriteLine(ce.Label);
                entries.Add(ce);
            }
            chartViewBar.Chart = new BarChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Vertical,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 40,                
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
            MachinesPop();
        }
    }
}