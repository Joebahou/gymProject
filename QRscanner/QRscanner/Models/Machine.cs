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
        public Machine(string name, int id_machine,int available)
        {
            name_ = name;
            id_machine_ = id_machine;
            available_ = available;
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
    }
}
