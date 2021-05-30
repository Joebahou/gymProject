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
        public int[] data = new int[6];
        public const string DeviceConnectionString = @"HostName=GymIotHub.azure-devices.net;DeviceId=MyAndroidDevice;SharedAccessKey=+AOL7RsMUcFFwF+tCUzGS3+8IuPD27FfyUegMvKEtHo=";
        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);


        private string weight;
        public string Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Weight"));
            }
        }
        private string speed;
        public string Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Speed"));
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
      
        public ICommand SubmitCommand { set; get; }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Usage()
        {
            SubmitCommand = new Command(OnSubmit);

        }
        public int additional_info(String info)
        {
            if (info != null)
            {
                return int.Parse(info);
            }
            else
                return 0;

        }
        public async void OnSubmit()
        {

            data[0] = MainPage.id_member;
            data[1] = MainPage.id_machine;
            data[2] = additional_info(speed);
            data[3] = additional_info(weight);
            data[4] = additional_info(reps);
            data[5] = additional_info(sets);


            string messageJson = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.ASCII.GetBytes(messageJson)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            await Client.SendEventAsync(message);
            



        }

    }
}
