using Microsoft.Azure.Devices.Client;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QRscanner.Models
{
    class Usage : INotifyPropertyChanged
    {
        public int[] dataUsage = new int[5];
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);


        private string weight_or_speed;
        public string Weight_Or_Speed
        {
            get { return weight_or_speed; }
            set
            {
                weight_or_speed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Weight_Or_Speed"));
            }
        }
        
        private string reps;
        public string Reps
        {
            get { return reps; }
            set
            {
                reps = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Reps"));
            }
        }
        private string sets;
        public string Sets
        {
            get { return sets; }
            set
            {
                sets = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Sets"));
            }
        }
      
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Usage()
        {
            

        }
        public int additional_info(String info)
        {
            int result;
            if (info != null)
            {

                if (Int32.TryParse(info, out result))
                    return result;
                else
                {
                    return -1;
                }

            }
            else
                return 0;

        }
        

    }
}
