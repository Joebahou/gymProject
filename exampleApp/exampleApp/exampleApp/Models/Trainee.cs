using System;
using System.Collections.Generic;
using System.Text;

namespace exampleApp.Models
{
    class Trainee
    {
        private  string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;

            }
        }


        private  int id;
        public  int Id
        {
            get { return id; }
            set
            {
                id = value;

            }
        }
    }
}
