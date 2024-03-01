using MCTG.Interfaces;
using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Game
{
    internal class MonsterFight : IFight
    {
        public int Fight(Deck Player_one, Deck Player_two, int CardIndexPl_one, int CardIndexPl_two)
        {
            if (CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two])
               && CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return CompareDamage(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]);
            }
            else if (CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two])
                && !CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return 1;
            }
            else if (!CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) 
                && CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return -1;
            }
            else if (!CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) 
                && !CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return 0; //draw
            }
            else
            {
                throw new Exception("Error while Mixed-Fight");
            }
        }
        //Compares Damage of two cards
        public int CompareDamage(Card Player_one, Card Player_two)
        {
            if (Player_one.Damage > Player_two.Damage)
            {
                return 1; //Player_one Card wins
            }
            else if (Player_one.Damage < Player_two.Damage)
            {
                return -1; //Player_tw0 wins
            }
            else
            {
                return 0; //draw (Damage is equal)
            }
        }

        //Checks for Attack Exceptions(Specialitites)
        public bool CanAttack(Card Player_one, Card Player_two)
        {
            //Goblins are too afraid of Dragons to attack.
            if (Player_one.Monster == Monsters.Goblin && Player_two.Monster == Monsters.Dragon)
            {
                return false;
            }//Wizzard can control Orks so they are not able to damage them
            else if (Player_one.Monster == Monsters.Ork && Player_two.Monster == Monsters.Wizzard)
            {
                return false;
            }//The FireElves know Dragons since they were little and can evade their attacks.
            else if (Player_one.Monster == Monsters.Dragon && (Player_two.Monster == Monsters.Elve && Player_two.Element == ElementType.Fire))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void Print(Card Player_one, Card Player_two)
        {

        }
    }
}
