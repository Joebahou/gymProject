using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRscanner.Models
{
    public class Machine
    {

        private string name_;
        private int id_machine_;
        private int available_;
        private int id_member;
        private int taken;
        private int alert_broken;
        public Machine(string name, int id_machine,int available,int alret_broken_)
        {
            name_ = name;
            id_machine_ = id_machine;
            available_ = available;
            alert_broken = alret_broken_;
        }
        [JsonConstructor]
        public Machine(string name, int id_machine, int available, int taken, int id_member, int alert_broken)
        {
            name_ = name;
            this.available_ = available;
            id_machine_ = id_machine;
            this.taken = taken;
            this.id_member = id_member;
            this.alert_broken = alert_broken;

        }

        public string Name
        {
            get { return name_; }
            set
            {
                name_ = value;
            }
        }


        public int Id_machine
        {
            get { return id_machine_; }
            set
            {
                id_machine_ = value;

            }
        }
        public int Taken
        {
            get { return taken; }
            set
            {
                taken = value;

            }
        }
        public int Id_member
        {
            get { return id_member; }
            set
            {
                id_member = value;

            }
        }
        public int Available
        {
            get { return available_; }
            set
            {
                available_ = value;

            }
        }
        public int Alert_broken
        {
            get { return alert_broken; }
            set
            {
                alert_broken = value;

            }
        }
    }
}
