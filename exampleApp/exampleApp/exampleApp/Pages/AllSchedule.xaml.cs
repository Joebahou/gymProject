
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllSchedule : ContentPage
    {
        ObservableCollection<Temp> list_bind = new ObservableCollection<Temp>();
        public ObservableCollection<Temp> List_bind { get { return list_bind; } }
        public AllSchedule()
        {
            InitializeComponent();
            Peopleview.ItemsSource = list_bind;
       



        }
        private void scheduleButton_Clicked(object sender, EventArgs e)
        {
            list_bind = new ObservableCollection<Temp>();
            Peopleview.ItemsSource = list_bind;
            List<String> l_temp = new List<string>();
           for(int i = 0; i <= 10; i++)
            {
                l_temp.Add(i.ToString());
            }
            list_bind.Add(new Temp { Li = l_temp });
            list_bind.Add(new Temp { Li = l_temp });
            frame.IsVisible = true;
            Peopleview.IsVisible = true;
        }
        public class Temp
        {
            public List<String> Li { get; set; }
        }
    }
}