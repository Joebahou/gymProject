using System;
using System.Collections.Generic;
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
        public static List<Models.Machine> machines_list;
        public availableMachines_owner()
        {
            InitializeComponent();
        }
    }
}