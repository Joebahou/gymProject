using Newtonsoft.Json;
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

        private string gender;
        public string Gender
        {
            get { return gender; }
            set
            {
                gender = value;

            }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set
            {
                age = value;

            }
        }

        [JsonConstructor]
        public Trainee(int id_member,string name,string gender,int age)
        {
            this.id = id_member;
            this.name = name;
            this.gender = gender;
            this.age = age;
        }
    }
}
