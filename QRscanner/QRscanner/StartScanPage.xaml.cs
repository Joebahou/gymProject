using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using ZXing.Net.Mobile.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using MySqlConnector;
using ZXing.Mobile;

namespace QRscanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartScanPage : ContentPage
    {
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        
        ZXingScannerPage scanPage;
        public static int id_member;
        DateTime scanning_time;
        DateTime nearest_schedule = new DateTime(2021, 7, 19, 0, 0, 0);
        private int id_member_of_the_nearest_schedule=-1;
        int[] dataUsage = new int[5];
        TimeSpan time_has_passed;
        
        TimeSpan five_min = new TimeSpan(0, 5, 0);
        //id machine of the member who has been scanned, if none eaquals to -1
        int id_machine_of_member_fromDB=-1;

       
        public  StartScanPage()
        {
            InitializeComponent();
            String catching_msg = "";
            scanning_time = DateTime.Now;
            dataUsage[1] = MainPage.id_machine;
            dataUsage[2] = 0;
            dataUsage[3] = 0;
            dataUsage[4] = 0;
            MobileBarcodeScanningOptions options = new MobileBarcodeScanningOptions();
            options.DelayBetweenContinuousScans = 5000;
          
            scanPage = new ZXingScannerPage(options);
            
            scanPage.OnScanResult += async (result) =>
            {
                String res = result.Text;
                scanPage.IsScanning = false;
                //Do something with result
                id_member = int.Parse(res);
                // check that id_member is from the members table
                if (App.taken == 1)
                {
                   
                    if (App.member_from_table == id_member)
                    {
                        //user is now finishing using the machine. do not enter the usagepage
                        dataUsage[0] = id_member;
                        //caching_msg = "id_member = " + id_member + " has finished using id machine " + id_machine;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            App.finished = true;
                            string messageJson = JsonConvert.SerializeObject(dataUsage);
                            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
                            ////an usage event handled by eventhub function
                            await Client.SendEventAsync(message);
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            activityIndicator.IsVisible = true;
                            await Task.Delay(2500);
                            activityIndicator.IsVisible = false;
                            await Navigation.PopAsync();


                        });

                    }
                    else
                    {
                        //somebody alse is using the machine,mabey pop a msg 
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            
                            catching_msg = App.name_of_member + " is already using this machine ";
                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", catching_msg, "OK");
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            await Navigation.PopAsync();
                        });
                    }
                }
                else
                {
                    //http requests to get info from the DB
                    string answer;
                    string req = "https://gymfuctions.azurewebsites.net/api/selecet_QRscanner?query=id_machine_of_member_fromDB&id_member=" + id_member;
                    answer = Models.Connection.get_result_from_http(req, false);
                    id_machine_of_member_fromDB = Int32.Parse(answer);

                    req = "https://gymfuctions.azurewebsites.net/api/selecet_QRscanner?query=nearest_schedule&id_machine=" + MainPage.id_machine + "&scanning_time="+ scanning_time.ToString();
                    answer = Models.Connection.get_result_from_http(req, false);
                    nearest_schedule = Convert.ToDateTime(answer);

                    req = "https://gymfuctions.azurewebsites.net/api/selecet_QRscanner?query=id_member_of_the_nearest_schedule&id_machine=" + MainPage.id_machine + "&nearest_schedule=" + nearest_schedule.ToString();
                    answer = Models.Connection.get_result_from_http(req, false);
                    id_member_of_the_nearest_schedule = Int32.Parse(answer);

                    time_has_passed = scanning_time - nearest_schedule;
                    Console.WriteLine(time_has_passed.ToString());
                    Console.WriteLine(nearest_schedule.ToString());
                    Console.WriteLine(five_min.ToString());
                    //the machine is free to use. need to cheack that member is not using other machine at the same time
                    if (id_machine_of_member_fromDB == -1)
                    {
                        // check if the same member scheduled the machine and it's still relevent
                        if (id_member_of_the_nearest_schedule == id_member)
                        {

                            Device.BeginInvokeOnMainThread(async () =>
                            {

                               
                                await App.Current.MainPage.Navigation.PopModalAsync();
                                    
                                await App.Current.MainPage.Navigation.PushAsync(new InfoUsage());


                            });
                        }
                        else
                        {
                            // schedule isnt relevent enymore
                            if(time_has_passed > five_min)
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {

                                    await App.Current.MainPage.Navigation.PopModalAsync();
                                    
                                    await App.Current.MainPage.Navigation.PushAsync(new InfoUsage());


                                });
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {

                                    catching_msg = "this machine has been scheduled by onther trainer at this time. If he will not come in "+(five_min-time_has_passed).Minutes+" minutes, you can scan again and use the machine";
                                    await App.Current.MainPage.DisplayAlert("Scanned Barcode", catching_msg, "OK");
                                    await App.Current.MainPage.Navigation.PopModalAsync();
                                    await App.Current.MainPage.Navigation.PopAsync();
                                });
                            }
                        }
                    }
                    //member is trying to use 2 machines at the same time
                    else
                    {
                        
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            
                            catching_msg = "you can't use more than one machine at the same time!";
                            await App.Current.MainPage.DisplayAlert("Scanned Barcode", catching_msg, "OK");
                            await App.Current.MainPage.Navigation.PopModalAsync();
                            await App.Current.MainPage.Navigation.PopAsync();
                        });
                    }

                }


            };


           App.Current.MainPage.Navigation.PushModalAsync(scanPage);
            

        }
      
    }
}