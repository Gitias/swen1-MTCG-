using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    internal class Stack
    {
        internal List<Card> cards = new List<Card>();
        public int UserId { get; set; }
    }
}
