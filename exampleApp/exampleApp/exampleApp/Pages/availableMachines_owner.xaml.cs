using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class availableMachines_owner : ContentPage
    {

        public class Machinebind
        {
            public string name { get; set; }
            public int id_machine { get; set; }
            public bool available { get; set; }

        }
        ObservableCollection<Machinebind> list_bind;
        public ObservableCollection<Machinebind> List_bind { get { return list_bind; } }

        public static List<Models.Machine> machines_list;
        public availableMachines_owner()
        {
            InitializeComponent();
            list_bind = new ObservableCollection<Machinebind>();
            InitList();
        }
        public void InitList()
        {
            foreach (Models.Machine m in machines_list)
            {
                if (m.Available == 1)
                {
                    Machinebind temp = new Machinebind { name = m.Name, id_machine = m.Id_machine, available = true };
                    list_bind.Add(temp);

                }
                else
                {
                    Machinebind temp = new Machinebind { name = m.Name, id_machine = m.Id_machine, available = false };
                    list_bind.Add(temp);
                }
            }
            available_machines.ItemsSource = list_bind;

        }
        public void delete_clicked(Object sender, System.EventArgs e)
        {
           
        }
        public void addMachineButton_Clicked(Object sender, System.EventArgs e)
        {

        }
        
    }
}