using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Interfaces
{
    internal interface IFight  //Fight Interface
    {
        public int CompareDamage(Card Player_one, Card Player_two); //maybe add dmg_multiplyer float Pl_one_multi, float Pl_two_multi
        public void Print(Card Player_one, Card Player_two);
        public int Fight(Deck Player_one, Deck Player_two, int CardIndexPl_one, int CardIndexPl_two);
        public bool CanAttack(Card Player_one, Card Player_two);
    }
}
