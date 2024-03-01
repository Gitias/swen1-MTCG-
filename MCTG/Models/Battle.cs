using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    internal class Battle
    {
        public string Id { get; set; }
        public int User_id { get; set; }
        public int? Challenger_id { get; set; }
        public bool is_Active { get; set; }
        public int? Rounds { get; set; }
        public bool? is_Draw { get; set; }
        public int? Winner { get; set; }
        public bool is_Completed { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, User_id: {User_id}, Challenger: {Challenger_id}, Active: {is_Active}";
        }
    }
}
