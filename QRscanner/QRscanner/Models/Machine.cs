using System;
using System.Collections.Generic;
using System.Text;

namespace QRscanner.Models
{
    public class Machine
    {

        private string name_;
        private int id_machine_;

        public Machine(string name, int id_machine)
        {
            name_ = name;
            id_machine_ = id_machine;
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

    }
}
