using Microsoft.AspNetCore.SignalR.Client;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsedMachines : ContentPage
    {
        public static List<Models.Machine> machines_list;
        public static HubConnection connection;
      
    
        public async void Set_signalR()
        {
            connection = new HubConnectionBuilder()
               .WithUrl("https://gymfuctions.azurewebsites.net/api")
               .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<int[]>("newMessage", (msgupdate) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {

                    int id_machine = msgupdate[0];
                    int taken = msgupdate[1];
                    
                    foreach(Models.Machine temp in machines_list)
                    {
                        if(temp.Id_machine == id_machine)
                        {
                            if(taken == 0)
                            {
                                temp.FColor = Color.Green;
                                
                            }
                            else
                            {
                                temp.FColor = Color.Red;
                            }
                        }

                    }
                  





                });
            });
            await connection.StartAsync();

        }
        public  UsedMachines()
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

                  

                    BoxView boxView = new BoxView();
                    boxView.SetBinding(BoxView.ColorProperty, "FColor");


                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                               boxView,

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
            
            Set_signalR();
            

        }
    }
}