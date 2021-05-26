using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoUsage : ContentPage
    {
       
        public InfoUsage()
        {
            InitializeComponent();
            var vm = new Models.Usage();
            this.BindingContext = vm;


        }
     

    }
}