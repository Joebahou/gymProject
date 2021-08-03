using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace exampleApp.Models
{
    public class Machine : INotifyPropertyChanged
    {  
        private Color f_Color;
        private string name_;
        private int id_machine_;
        private int available;
        public string[] schedule_machine;
        public event PropertyChangedEventHandler PropertyChanged;
        public Machine(string name, Color fColor, int id_machine)
        {
            name_ = name;
            f_Color = fColor;
            id_machine_ = id_machine;

        }
        public Machine(int id_machine, string name, int is_available)
        {
            name_ = name;
            available = is_available;
            id_machine_ = id_machine;

        }
        [JsonConstructor]
        public Machine(string name, int id_machine, int available)
        {
            name_ = name;
            this.available = available;
            id_machine_ = id_machine;

        }
     
        public Machine(string name,  int id_machine)
        {
            name_ = name;
            id_machine_ = id_machine;
            Init_schedule();

        }
        public void Init_schedule()
        {
            schedule_machine= new string[38];
            schedule_machine[0] = name_;
            for(int i = 1; i <= 37; i++)
            {
                schedule_machine[i] = "";
            }
        }

        public string Name {
            get { return name_; }
            set
            {
                name_ = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));

            }
        }

        public int Available
        {
            get { return available; }
            set
            {
                available = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Available"));

            }
        }
      


        public int Id_machine {
            get { return id_machine_; }
            set
            {
                id_machine_ = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Id_machine"));

            }
        }
        public Color FColor
        {
            set
            {
                f_Color = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FColor"));

            }

            get
            {
                return f_Color;
            }

        }
    }
}
