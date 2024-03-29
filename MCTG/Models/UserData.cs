﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MCTG.Models
{
    internal class UserData
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}, Bio: {Bio}, Image: {Image}";
        }
    }
}
