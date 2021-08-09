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
    public partial class QrImage : ContentPage
    {
        public QrImage()
        {
            InitializeComponent();
            barcode.BarcodeValue = Models.User.Id.ToString();//set the id_member loged in the be the value of the qr.
        }
    }
}