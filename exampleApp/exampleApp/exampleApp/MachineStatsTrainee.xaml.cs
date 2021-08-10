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


            pickerMachineInit();
        }
        public class MachineData
        {
            public int sets { get; set; }
            public int reps { get; set; }
            public int weightorspeed { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string date { get; set; }
            [JsonConstructor]
            public MachineData(int sets, int reps, int weightorspeed, string start, string end, string date)
            {
                this.sets = sets;
                this.reps = reps;
                this.weightorspeed = weightorspeed;
                this.start = start;
                this.end = end;
                this.date = date;
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
            ListData.Clear();
            string parameters = "id_member=" + IdMember +
                "&machineName=" + machineName;
            string req = "https://gymfuctions.azurewebsites.net/api/select_used_machines?query=select_used_machines&" + parameters;
            string result = Models.Connection.get_result_from_http(req, true);
           List<MachineData> uses_of_machine= JsonConvert.DeserializeObject<List<MachineData>>(result);
            if (uses_of_machine.Count != 0)
            {
                foreach(MachineData m in uses_of_machine)
                {
                    ListData.Add(m);
                }
            }
            MachineStatsListView.ItemsSource = ListData;

        }
    }
}