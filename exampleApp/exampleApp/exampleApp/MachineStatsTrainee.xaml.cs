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

namespace exampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MachineStatsTrainee : ContentPage
    {
        private int IdMember;
        
        ObservableCollection<MachineData> listData = new ObservableCollection<MachineData>();
        public ObservableCollection<MachineData> ListData { get { return listData; } }

        public MachineStatsTrainee()
        {
            InitializeComponent();
            BindingContext = this;
            /*check if it is trainer or trainee*/
            int type = Models.User.Type;
            Console.WriteLine("type: " + type);
            if (type == 1)
            {
                IdMember = TrainerStatistics.StatsPerTrainee.idTrainee;
            }
            else { IdMember = Models.User.Id; }
            Console.WriteLine("id trainee: " + IdMember);


            ConnectDataBase();
            pickerMachineInit();
        }
        public class MachineData
        {
            public int Sets { get; set; }
            public int Reps { get; set; }
            public int WeightOrSpeed { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public string Date { get; set; }
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
            string machineName = pickerMachines.SelectedItem.ToString();

            MachineUploadData(machineName);

        }

        /**/


        /*machine data grid*/
        private void MachineUploadData(string machineName)
        {
            string cmd_text = $"select sets, reps, weight_or_speed, usage_gym.start, usage_gym.end " +
                $"from members, machines, usage_gym " +
                $"where members.idmember = usage_gym.idmember " +
                $"and machines.idmachine = usage_gym.idmachine " +
                $"and members.idmember = {IdMember} " +
                $"and machines.name = '{machineName}'" +
                $"order by usage_gym.start";

            ListData.Clear();
            MySqlCommand cmd = new MySqlCommand(cmd_text, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    long sets = 0;
                    long reps = 0;
                    long speedOrWeight = 0;
                    DateTime dateStart = DateTime.Now;
                    DateTime dateEnd = DateTime.Now;

                    if (rdr[0] != DBNull.Value)
                    {
                        sets = rdr.GetInt32(0);
                    }
                    if (rdr[1] != DBNull.Value)
                    {
                        reps = rdr.GetInt32(1);
                    }
                    if (rdr[2] != DBNull.Value)
                    {
                        speedOrWeight = rdr.GetInt32(2);
                    }
                    if (rdr[3] != DBNull.Value)
                    {
                        dateStart = (DateTime)rdr[3];
                    }
                    if (rdr[4] != DBNull.Value)
                    {
                        dateEnd = (DateTime)rdr[4];
                    }

                    MachineData temp = new MachineData()
                    {
                        Sets = (int)sets,
                        Reps = (int)reps,
                        WeightOrSpeed = (int)speedOrWeight,
                        Start = dateStart.ToString("HH:mm:ss"),
                        End = dateEnd.ToString("HH:mm:ss"),
                        Date = dateStart.ToString("dd/MM/yy")
                    };
                    Console.WriteLine("Date " + temp.Date);
                    Console.WriteLine("Reps " + temp.Reps);
                    ListData.Add(temp);

                }
                rdr.Close();
                MachineStatsListView.ItemsSource = ListData;

            }
        }
    }
}