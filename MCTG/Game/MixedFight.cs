using MCTG.Interfaces;
using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Game
{
    internal class MixedFight : IFight
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
            else if(!CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) && CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
            {
                return -1;
            }
            else if(!CanAttack(Player_one.deck[CardIndexPl_one], Player_two.deck[CardIndexPl_two]) && !CanAttack(Player_two.deck[CardIndexPl_two], Player_one.deck[CardIndexPl_one]))
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
            (float Pl_one_multi, float Pl_two_multi) = CalculateEffect(Player_one.Element, Player_two.Element);
            if (Player_one.Damage * Pl_one_multi > Player_two.Damage * Pl_two_multi)
            {

                return 1; //Player_one Card wins
            }
            else if (Player_one.Damage * Pl_one_multi < Player_two.Damage * Pl_two_multi)
            {
                return -1; //Player_tw0 wins
            }
            else
            {
                return 0; //draw (Damage is equal)
            }
        }

        //calculate element effects (mixed fight --> elemt type plays a role)
        public Tuple<float, float> CalculateEffect(ElementType element_one, ElementType element_two)
        {
            if (element_one != element_two)
            {
                if ((element_one == ElementType.Water && element_two == ElementType.Fire) ||
                    (element_one == ElementType.Fire && element_two == ElementType.Normal) ||
                    (element_one == ElementType.Normal && element_two == ElementType.Water))
                {
                    return Tuple.Create(2.0f, 0.5f);
                }
                else if ((element_one == ElementType.Fire && element_two == ElementType.Water) ||
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

        //Checks for Attack Exceptions(Specialitites)
        public bool CanAttack(Card Player_one, Card Player_two)
        {
            //The Kraken is immune against spells
            if (Player_one.Type == CardType.SpellCard && Player_two.Monster == Monsters.Kraken) 
            {
                return false;
            }//The armor of Knights is so heavy that WaterSpells make them drown them instantly.
            else if((Player_one.Monster == Monsters.Knight && Player_two.Element == ElementType.Water) && Player_two.Type == CardType.SpellCard)
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
