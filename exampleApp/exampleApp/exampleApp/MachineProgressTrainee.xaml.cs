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
using Newtonsoft.Json;

namespace exampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MachineProgressTrainee : ContentPage
    {
        private int IdMember;
        
        public MachineProgressTrainee()
        {
            InitializeComponent();
            /*check if it is trainer or trainee*/
            int type = Models.User.Type;
            Console.WriteLine("type: " + type);
            if (type == 1)
            {
                IdMember = TrainerStatistics.StatsPerTrainee.idTrainee;
            }
            else { IdMember = Models.User.Id; }
            Console.WriteLine("id trainee: " + IdMember);
            
            

            //ConnectDataBase();
            pickerMachineInit();
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

        /*load machine to picker*/
        private void pickerMachineInit()
        {
            List<string> MachineNames = new List<string>();
            string parameters = "id_member=" + IdMember;
            string req = "https://gymfuctions.azurewebsites.net/api/machine_used_by_trainee?query=machine_used_by_trainee&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            MachineNames = JsonConvert.DeserializeObject<List<string>>(result);
            /*
            string cmd_text = $"select distinct machines.name " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.idmember = {IdMember} ";
            List<string> MachineNames = new List<string>();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (rdr[0] != DBNull.Value)
                    {
                        MachineNames.Add(rdr[0].ToString());
                    }
                }
            }
            rdr.Close();
            */
            pickerMachines.Items.Clear();
            foreach (string name in MachineNames)
            {
                pickerMachines.Items.Add(name);
            }
        }
        private string machineName;
        private DateTime DateStart = DateTime.Now;
        private DateTime DateEnd = DateTime.Now;
        private void pickerMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineName = pickerMachines.SelectedItem.ToString();

            

        }
        public class progress_from_sql
        {
            public double score { get; set; }
            public DateTime start { get; set; }

            [JsonConstructor]
            public progress_from_sql(double score, DateTime start)
            {
                this.score = score;
                this.start = start;
            }
        }

        /*this method load the machine progress to microchart*/
        private void MachineUploadProgress(string machineName)
        {
            List<ChartEntry> entries = new List<ChartEntry>();
            List<Tuple<double, DateTime>> MachineNumUses = new List<Tuple<double, DateTime>>();
            string parameters = "id_member=" + IdMember +
               "&machineName=" + machineName;
            string req = "https://gymfuctions.azurewebsites.net/api/machine_used_by_trainee?query=machine_progress&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
            List<progress_from_sql> progress = JsonConvert.DeserializeObject<List<progress_from_sql>>(result);
            if (progress.Count != 0)
            {
                foreach(progress_from_sql p in progress)
                {
                    double score = -1;
                    DateTime date = DateTime.Now;

                    score = p.score;

                    date = p.start; 
                    MachineNumUses.Add(Tuple.Create(score, date));
                }
                foreach (Tuple<double, DateTime> entry in MachineNumUses)
                {
                    if (entry.Item1 != -1)
                    {
                        /*DATES*/
                        DateTime currDate = entry.Item2;
                        int res = DateTime.Compare(currDate.Date, DateStart.Date);
                        Console.WriteLine("curr= " + currDate.Date + " start= " + DateStart.Date + " res= " + res);
                        if (res < 0) { continue; }
                        res = DateTime.Compare(currDate.Date, DateEnd.Date);
                        Console.WriteLine("curr= " + currDate.Date + " end= " + DateEnd.Date + " res= " + res);
                        if (res > 0) { continue; }
                        /*DATES*/
                        ChartEntry ce = new ChartEntry((float)entry.Item1)
                        {
                            Label = entry.Item2.ToString("d/M/yy"),
                            ValueLabel = entry.Item1.ToString(),
                            Color = SKColor.Parse("#eb5b34")
                        };
                        entries.Add(ce);
                    }
                }


                chartViewBar.Chart = new LineChart
                {
                    Entries = entries,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                    BackgroundColor = SKColor.Parse("#00ffffff"),
                    LabelTextSize = 40,
                    PointMode = PointMode.Square
                };
            }
            /*
            string cmd_text = $"select weight_or_speed, usage_gym.start " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.idmember = {IdMember} " +
                $"and machines.name = '{machineName}' " +
                $"limit 20";
            List<ChartEntry> entries = new List<ChartEntry>();
            List<Tuple<long, DateTime>> MachineNumUses = new List<Tuple<long, DateTime>>();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    long speedOrWeight = -1;
                    DateTime date = DateTime.Now;
                    if (rdr[0] != DBNull.Value)
                    {
                        speedOrWeight = rdr.GetInt32(0);
                    }
                    if (rdr[1] != DBNull.Value)
                    {
                        date = (DateTime)rdr[1];
                    }


                    MachineNumUses.Add(Tuple.Create(speedOrWeight, date));

                }
                rdr.Close();*/
           
            
        }

        private void ButtonShowProgress_Clicked(object sender, EventArgs e)
        {
            bool okay = CheckAllSelected();
            if(okay == false)
            {
                DisplayAlert("Alert", "Select Machine, Start And End Date", "OK");
                return;
            }
            MachineUploadProgress(machineName);
        }

        private void DatePickerStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateStart = DatePickerStart.Date;
        }

        private void DatePickerEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateEnd = DatePickerEnd.Date;
        }
        
        /*check if the user selected the required fields*/
        private bool CheckAllSelected()
        {
            if (pickerMachines.SelectedIndex == -1)
            {
                return false;
            }
            if (DateEnd == null)
            {
                return false;
            }
            if(DateStart == null)
            {
                return false;
            }
            return true;
        }
    }
}