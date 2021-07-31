using System;
using System.Collections.Generic;
using System.Text;

namespace exampleApp.Models
{
    class User
    {
        private static string name;
        public static string Name
        {
            get { return name; }
            set
            {
                name= value;

            }
        }
        private static string gender;
        public static string Gender
        {
            get { return gender; }
            set
            {
                gender= value;

            }
        }



        private static int id;
        public static int Id
        {
            get { return id; }
            set
            {
                id = value;
               
            }
        }
        private static List<Trainee> trainees;
        public static List<Trainee> Trainees
        {
            get { return trainees; }
            set
            {
               trainees = value;

            }
        }
        private static int type;
        public static int Type
        {
            get { return type; }
            set
            {
                type = value;

            }
        }
        private static int age;
        public static int Age
        {
            get { return age; }
            set
            {
                age = value;

            }
        }
    }
}
