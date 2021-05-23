using GymApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GymApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}