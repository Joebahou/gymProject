using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace exampleApp.Models
{
    class Trainer
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;

            }
        }


        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;

            }
        }
        [JsonConstructor]
        public Trainer(int id_member, string name)
        {
            this.id = id_member;
            this.name = name;
            
        }
    }
}
