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
    public partial class StatisticsNavigationTrainee : ContentPage
    {
        public StatisticsNavigationTrainee()
        {
            InitializeComponent();
        }
        private void MachinesStats_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MachineStatsTrainee());
        }

        private void MachinesProgress_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MachineProgressTrainee());
        }
    }
}