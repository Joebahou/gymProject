using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class checkboxmachinePage : ContentPage
    {
        public static List<Models.Machine> machines_list;
        public string caching_msg;
        private Boolean loadingVisbile;
        public Boolean LoadingVisbile
        {
            get { return loadingVisbile; }
            set { loadingVisbile = value; }
        }

        public async void OnButttonClicked(object sender, EventArgs e)
        {
            Boolean available = false;
            String id = (sender as Button).Text;
            int temp = int.Parse(id);
            foreach (Models.Machine m in machines_list)
            {
                if (m.Id_machine == temp)
                {
                    if (m.Available == 1)
                    {
                        MainPage.id_machine = m.Id_machine;
                        MainPage.name_machine = m.Name;
                        available = true;
                        break;
                    }
                    else
                    {
                        caching_msg = "This machine has been set as not available by the owner";
                        await App.Current.MainPage.DisplayAlert("Not Working", caching_msg, "OK");
                    }
                }

            }
            if (available)
                await Navigation.PushAsync(new MainPage());
        }
        public checkboxmachinePage()
        {
            InitializeComponent();
          




            // Create the ListView.
            ListView listView = new ListView
            {
                RowHeight = 60,
                HeightRequest = 60,
                // Source of data items.
                ItemsSource = machines_list,
                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label { HeightRequest= 50 };
                    nameLabel.SetBinding(Label.TextProperty, "Name");
                    nameLabel.FontSize = 20;
                    Button button = new Button();
                    button.HeightRequest=50;
                    button.SetBinding(Button.TextProperty, "Id_machine");
                    button.Clicked += OnButttonClicked;

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {   
                        View = new StackLayout
                        {
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions=LayoutOptions.StartAndExpand,
                            Padding = 5,
                            HeightRequest = 60,
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                                    new StackLayout { HorizontalOptions=LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.FillAndExpand,HeightRequest=60, Spacing = 60, Children = { button } },


                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.CenterAndExpand,
                                        HeightRequest=50,
                                        Spacing = 50,
                                        
                                        Children =
                                        {
                                            nameLabel



                                        }
                                    }


                            }
                        }
                    };
                })
            };
            this.Content = new StackLayout
            {
                Children =
                {
                   
                    listView
                }
            };
        }
    }
}