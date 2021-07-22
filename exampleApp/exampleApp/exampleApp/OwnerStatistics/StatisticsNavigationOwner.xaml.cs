using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.OwnerStatistics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsNavigationOwner : ContentPage
    {
        public StatisticsNavigationOwner()
        {
            InitializeComponent();
        }

        private void MachinesStats_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OwnerStatistics.MachinesPopularity());
        }

        private void HoursStats_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OwnerStatistics.HoursPopularityxaml());
        }
    }
}