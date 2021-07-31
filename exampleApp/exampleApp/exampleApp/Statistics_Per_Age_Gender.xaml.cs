using Microcharts;
using MySqlConnector;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Statistics_Per_Age_Gender : ContentPage
    {
        private int IdMember;
        private string gender;
        private int age;
        private string machineName;
        /*connect to data base*/
        MySqlConnection conn;
        public Statistics_Per_Age_Gender()
        {
            InitializeComponent();
            int type = Models.User.Type;
            Console.WriteLine("type: " + type);
            if (type == 1)
            {
                IdMember = TrainerStatistics.StatsPerTrainee.idTrainee;
                gender = TrainerStatistics.StatsPerTrainee.genderTrainee;
                age = TrainerStatistics.StatsPerTrainee.ageTrainee;
            }
            else
            { 
                IdMember = Models.User.Id;
                gender = Models.User.Gender;
                age = Models.User.Age;
            }
            ConnectDataBase();
            pickerMachineInit();
        }
      
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
        private void pickerMachineInit()
        {
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
            pickerMachines.Items.Clear();
            foreach (string name in MachineNames)
            {
                pickerMachines.Items.Add(name);
            }
        }
        private void pickerMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineName = pickerMachines.SelectedItem.ToString();

        }
        private void ButtonShowProgress_Clicked(object sender, EventArgs e)
        {
        
            if (pickerMachines.SelectedIndex == -1)
            {
                DisplayAlert("Alert", "Select Machine", "OK");
                return;
            }
            UploadGlobalProgress(machineName);
        }
        private void UploadGlobalProgress(string machineName)
        {
            string cmd_text = $"select usage_gym.score, members.idmember " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.age = '{age}' " +
                 $"and members.gender = '{gender}' " +
                $"and machines.name = '{machineName}' " +
                $"limit 50";
            Dictionary<int, double> Avg_Per_member = new Dictionary<int, double>();
            Dictionary<int, int> Times_Usg_Per_member = new Dictionary<int, int>();
            List<ChartEntry> entries = new List<ChartEntry>();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    double score = rdr.GetDouble(0);
                    int id_member = rdr.GetInt32(1);
                    if (score > 0)
                    {
                        if (Avg_Per_member.ContainsKey(id_member))
                        {
                            Avg_Per_member[id_member] += score;
                            Times_Usg_Per_member[id_member] += 1;
                        }
                        else
                        {
                            Avg_Per_member.Add(id_member, score);
                            Times_Usg_Per_member.Add(id_member, 1);

                        }

                    }

                }
            }
                rdr.Close();
            if (Avg_Per_member.Count == 0)
            {
                No_Progress_to_show();
            }
            else
            { //calculate avg
                foreach (int idm in Avg_Per_member.Keys.ToList())
                {
                    Avg_Per_member[idm] = Avg_Per_member[idm] / Times_Usg_Per_member[idm];
                }
                Dictionary<int, double> final_score_Per_member = new Dictionary<int, double>();
                double max_avg_score = Avg_Per_member.Values.Max();
                double min_avg_score = Avg_Per_member.Values.Min();
                foreach (int idm in Avg_Per_member.Keys)
                {
                    double current_avg_score = Avg_Per_member[idm];
                    double final_score = 0;
                    if (min_avg_score == max_avg_score)
                    {
                        final_score = 100;
                    }
                    else
                    {
                        final_score = ((current_avg_score - min_avg_score) / (max_avg_score - min_avg_score)) * 100;
                    }
                    final_score_Per_member.Add(idm, final_score);

                }

                Dictionary<int, int> NumberPerScala = new Dictionary<int, int>();
                for (int i = 0; i < 100; i += 10)
                {
                    NumberPerScala.Add(i, 0);

                }
                foreach (double final_score in final_score_Per_member.Values)
                {
                    int scala = (int)final_score / 10;
                    if (scala == 10)
                    {
                        NumberPerScala[90] += 1;
                    }
                    else
                    {
                        NumberPerScala[scala*10] += 1;
                    }
                
                }
                int scala_current_member = -1;
                if (final_score_Per_member.ContainsKey(IdMember))
                {
                    scala_current_member = (int)final_score_Per_member[IdMember]/10;
                    scala_current_member = scala_current_member * 10;
                    if (scala_current_member == 10)
                    {
                        scala_current_member = 90;
                    }
                }
            
                foreach (int scala in NumberPerScala.Keys)
                {
                    ChartEntry ce;
                    if (scala == scala_current_member)
                    {
                        ce = new ChartEntry(NumberPerScala[scala])
                        {
                            Label = scala.ToString() + "-" + (scala + 10).ToString(),
                            ValueLabel = NumberPerScala[scala].ToString(),
                            Color = SKColor.Parse("#ffca28")
                        };
                    }
                    else
                    {
                        ce = new ChartEntry(NumberPerScala[scala])
                        {
                        Label = scala.ToString() + "-" + (scala + 10).ToString(),
                        ValueLabel = NumberPerScala[scala].ToString(),
                        Color = SKColor.Parse("#eb5b34")
                        };
                    }
                
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
           

        }
        private void No_Progress_to_show()
        {
            DisplayAlert("Alert", "No Progress to show. All with score 0", "OK");
            return;
        }
    }
}