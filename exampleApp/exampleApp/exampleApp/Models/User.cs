﻿using System;
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



        private static int id;
        public static int Id
        {
            get { return id; }
            set
            {
                id = value;
               
            }
        }
       
    }
}