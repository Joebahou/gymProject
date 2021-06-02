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

namespace exampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPage : ContentPage
    {
        public StatsPage()
        {
            InitializeComponent();
            Console.WriteLine("Hello-");
            ConnectDataBase();
            
        }

        void InitPickerStats()
        {
            pickerStats.Items.Clear();
            pickerStats.Items.Add("Most Popular Machines");
            
        }
        

        MySqlConnection conn;
        private void ConnectDataBase()
        {
            try {
                
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void LoadMachines(string column)
        {
            
            string cmd_text = $"select {column} from machines";
            ArrayList machines = new ArrayList();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);            
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0].ToString() + rdr.ToString());
                    machines.Add(rdr[0]);
                    
                }                
                machinesListView.ItemsSource = machines;
            }
            rdr.Close();

        }

        private void TimesUsingMachine()
        {
            string cmd_text = $"select machines.name, count(*) " +
                $"from usage_gym, machines, members " +
                $"where machines.idmachine = usage_gym.idmachine and members.idmember = usage_gym.idmember " +
                $"and members.name = '{textboxName.Text}' " +
                $"group by usage_gym.idmachine order by count(*) desc ";
            List<Tuple<string, long>> MachineNumUses = new List<Tuple<string, long>>();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0].ToString() + " "+ rdr[1].ToString() + " " + rdr[1].GetType());
                    string machineName = (string)rdr[0];
                    long machineNumUses = (long) rdr[1];
                    MachineNumUses.Add(Tuple.Create(machineName, machineNumUses));

                }                
                machinesListView.ItemsSource = MachineNumUses;
            }
            rdr.Close();
            List<ChartEntry> entries = new List<ChartEntry>();
            foreach (Tuple<string, long> tuple in MachineNumUses)
            {
                ChartEntry ce = new ChartEntry(tuple.Item2)
                {
                    Label = tuple.Item1.ToString(),
                    ValueLabel = tuple.Item2.ToString(),
                    Color = SKColor.Parse("#3498db")
                };
                entries.Add(ce);
            }
            chartViewBar.Chart = new BarChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 40
            };
        }

        private void SoWperMachine()
        {
            string machineName = pickerMachines.SelectedItem.ToString();
            string memberName = textboxName.Text;
            string cmd_text = $"select weight_or_speed, usage_gym.start " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.name = '{memberName}' " +
                $"and machines.name = '{machineName}' " +
                $"limit 20";
            Console.WriteLine("name = " + textboxName.Text + " machine= " + machineName);
            List<ChartEntry> entries = new List<ChartEntry>();
            List<Tuple<long, DateTime>> MachineNumUses = new List<Tuple<long, DateTime>>();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[1].ToString()  + " " + rdr[1].GetType() + " " + rdr[1]);
                    long speedOrWeight = -1;
                    DateTime date = DateTime.Now;
                    if(rdr[0] != DBNull.Value)
                    {
                        speedOrWeight = rdr.GetInt32(0);
                    }          
                    if (rdr[1] != DBNull.Value)
                    {
                        date = (DateTime)rdr[1];
                    }


                    MachineNumUses.Add(Tuple.Create(speedOrWeight, date));

                }
                rdr.Close();
                foreach(Tuple<long, DateTime> entry in MachineNumUses)
                {
                    if(entry.Item1 != -1)
                    {
                        ChartEntry ce = new ChartEntry(entry.Item1)
                        {
                            Label = entry.Item2.ToString("d/M/yy"),
                            ValueLabel = entry.Item1.ToString(),
                            Color = SKColor.Parse("#b455b6")
                        };
                        entries.Add(ce);
                    }                                        
                }
                chartViewBar.Chart = new LineChart
                {
                    Entries = entries,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelTextSize = 40                    
                };
                machinesListView.ItemsSource = MachineNumUses;
            }
        }

        // clean all stats
        private void freshPage()
        {
            // clean all pickers
            pickerStats.SelectedIndex = -1;
            pickerMachines.SelectedIndex = -1;
            pickerStats.Items.Clear();
            pickerMachines.Items.Clear();
            pickerMachines.IsVisible = false;
            chartViewBar.Chart = null;
            machinesListView.ItemsSource = null;
        }
        private async void Entry_Completed(object sender, EventArgs e)
        {
            //fresh start
            freshPage();
            // checking if name is valid
            bool valid = false;
            string cmd_text = $"select members.idmember " +
                $"from members " +
                $"where members.name = '{textboxName.Text}' ";
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (rdr[0] != DBNull.Value)
                    {
                        valid = true;
                    }

                }                
            }
            rdr.Close();
            if (valid == true)
            {
                InitPickerStats();
                pickerMachineInit(textboxName.Text);
                pickerMachines.IsVisible = true;

            }
            else
            {
                //display alert
                await DisplayAlert("Warning:", $"{textboxName.Text} Is Not A Valid Member! ", "Type Again");
            }
        }

        private void pickerMachineInit(string memberName)
        {
            string cmd_text = $"select distinct machines.name " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.name = '{memberName}' ";
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

        private void pickerStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fresh stats
            chartViewBar.Chart = null;
            machinesListView.ItemsSource = null;

            if (pickerStats.SelectedIndex != -1)
            {
                string selected = pickerStats.SelectedItem.ToString();
                Console.WriteLine(selected);
                if (selected.Equals("Most Popular Machines"))
                {
                    TimesUsingMachine();
                }
                if (selected.Equals("Weight Or Speed History Per Machine"))
                {
                    SoWperMachine();
                }
            }       
        }

        private void pickerMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fresh start
            pickerStats.SelectedIndex = -1;            
            chartViewBar.Chart = null;
            machinesListView.ItemsSource = null;

            if (!string.IsNullOrEmpty(textboxName.Text))
            {
                if(!pickerStats.Items.Contains("Weight Or Speed History Per Machine"))
                {
                    pickerStats.Items.Add("Weight Or Speed History Per Machine");
                }               
            }
        }
    }
}
