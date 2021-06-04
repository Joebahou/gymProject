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
        public async void OnButttonClicked(object sender, EventArgs e)
        {
            String id = (sender as Button).Text;
            MainPage.id_machine = int.Parse(id);
            await Navigation.PushAsync(new MainPage());
        }
        public checkboxmachinePage()
        {
            InitializeComponent();
            Label header = new Label
            {
                Text = "Avilable Machines",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };




            // Create the ListView.
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = machines_list,
                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Name");




                    Button button = new Button();
                    button.SetBinding(Button.TextProperty, "Id_machine");
                    button.Clicked += OnButttonClicked;

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                              button,

                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
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
                    header,
                    listView
                }
            };
        }
    }
}