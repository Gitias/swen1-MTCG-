using MCTG.Interfaces;
using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Game
{
    internal class SpellFight : IFight
    {
        //Spell Fight return 1 (win), -1(loose) or 0(draw)
        public int Fight(Deck Player_one, Deck Player_two, int CardIndexPl_one, int CardIndexPl_two)
        {
            if (CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) 
                && CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return CompareDamage(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]);
            }
            else if (CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) //not relevant because spells can always attack each other (but might add some special effects later)
                && !CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }



        //Compares Damage of two cards
        public int CompareDamage(Card Player_one, Card Player_two)
        {
            (float Pl_one_multi, float Pl_two_multi) = CalculateEffect(Player_one.Element, Player_two.Element);
            if(Player_one.Damage * Pl_one_multi > Player_two.Damage * Pl_two_multi)
            {
                return 1;
            }
            else if(Player_one.Damage * Pl_one_multi < Player_two.Damage * Pl_two_multi)
            {
                return -1;
            }
            else
            {
                return 0; //draw (Damage is equal)
            }
        }

        //calculate element effects
        public Tuple<float, float> CalculateEffect(ElementType element_one, ElementType element_two)
        {
            if(element_one != element_two)
            {
                if((element_one == ElementType.Water && element_two == ElementType.Fire) || 
                    (element_one == ElementType.Fire && element_two == ElementType.Normal) ||
                    (element_one == ElementType.Normal && element_two == ElementType.Water))
                {
                    return Tuple.Create(2.0f, 0.5f);
                }
                else if((element_one == ElementType.Fire && element_two == ElementType.Water) ||
                    (element_one == ElementType.Normal && element_two == ElementType.Fire) ||
                    (element_one == ElementType.Water && element_two == ElementType.Normal))
                {
                    return Tuple.Create(0.5f, 2.0f);
                }
                else
                {
                    return Tuple.Create(1.0f, 1.0f); //no effect on damage
                }
            }
            else
            {
                return Tuple.Create(1.0f, 1.0f); //no effect on damage
            }
        }

        //Spells can always fight against each other (no ecxeptions)
        public bool CanAttack(Card Player_one, Card Player_two)
        {
            return true;
        }
        public void Print(Card Player_one, Card Player_two)
        {

        }
    }
}
