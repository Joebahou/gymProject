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

            //need to check if working
            // need to check if needed new connection
            connection.On<object[]>("BrokenMachine_real", (broken_msg) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    string id_machine_msg_string = broken_msg[0].ToString();
                    int id_machine_msg = Int32.Parse(id_machine_msg_string);


                    foreach (Models.Machine temp in machines_list)
                    {
                        if (temp.Id_machine == id_machine_msg)
                        {
                            temp.FColor = Color.Yellow;
                        }

                    }


                });
            });
            connection.On<object[]>("BrokenMachine_fixed", (broken_msg) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                
                    string id_machine_msg_string = broken_msg[0].ToString();
                    int id_machine_msg = Int32.Parse(id_machine_msg_string);


                    foreach (Models.Machine temp in machines_list)
                    {
                        if (temp.Id_machine == id_machine_msg)
                        {
                            temp.FColor = Color.Green;
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
                Text = "Gym Machines",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };




            // Create the ListView.
            ListView listView = new ListView
            {
                RowHeight=60,
                HeightRequest = 60,
                
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
                    boxView.HeightRequest = 60;
                    boxView.WidthRequest = 60;
                    
                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                       
                        Height = 60,
                        View = new StackLayout

                        {
                            
                            VerticalOptions=LayoutOptions.FillAndExpand,
                            HorizontalOptions=LayoutOptions.FillAndExpand,
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            HeightRequest = 60,
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

            BoxView b_green = new BoxView
            {
                Color = Color.Green
            };
            BoxView b_red = new BoxView
            {
                Color = Color.Red
            };
            BoxView b_yellow = new BoxView
            {
                Color = Color.Yellow
            };
            Label l_green = new Label
            {
                Text = "The machine is avalibale to use",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions=LayoutOptions.Center
                
            };
            Label l_red = new Label
            {
                Text = "The machine is now being used",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center

            };
            Label l_yellow = new Label
            {
                Text = "The machine is broken",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center

            };



            StackLayout mikra = new StackLayout
            {
                
                HeightRequest=200,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new StackLayout
                    {
                        Padding = 10,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            b_green,
                            l_green
                        }


                    },
                    new StackLayout
                    {
                        Padding = 10,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            b_red,
                            l_red
                        }


                    },
                    new StackLayout
                    {
                        Padding = 10,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            b_yellow,
                            l_yellow
                        }


                    }

                }

            };
            this.Content = new StackLayout
            {
                Children =
                {
                    new StackLayout
                    {
                        HeightRequest=600,
                        Children={
                        header,
                       listView
                        }
                     
                    },
                    
                    mikra
                }
            };
            
            Set_signalR();
            

        }
    }
}