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
        private int taken;
        private int id_member;
        private int alert_broken;
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
        public Machine(string name, int id_machine, int available,int taken,int id_member,int alert_broken)
        {
            name_ = name;
            this.available = available;
            id_machine_ = id_machine;
            this.taken = taken;
            this.id_member = id_member;
            this.alert_broken = alert_broken;

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
        public int Taken
        {
            get { return taken; }
            set
            {
                taken = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Taken"));

            }
        }
        public int Id_member
        {
            get { return id_member; }
            set
            {
                id_member = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Id_member"));

            }
        }
        public int Alert_broken
        {
            get { return alert_broken; }
            set
            {
                alert_broken = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Alert_broken"));

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
