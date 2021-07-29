﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QRscanner.Models
{
    public class Machine
    {

        private string name_;
        private int id_machine_;
        private int available_;
        private int alert_broken;
        public Machine(string name, int id_machine,int available,int alret_broken_)
        {
            name_ = name;
            id_machine_ = id_machine;
            available_ = available;
            alert_broken = alret_broken_;
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
