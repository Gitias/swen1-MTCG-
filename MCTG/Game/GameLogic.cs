using MCTG.Game;
using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Logic
{
    internal class GameLogic
    {
        internal Tuple<bool, int, int> Battle(Deck Player, Deck Opponent)
        {
            int Balance = 0;
            int rounds = 0;
            int player_index, opponent_index;
            Random rand = new Random();
            Console.WriteLine("_____START_BATTLE_____ \n");
            while (rounds <= 100)
            {
                if (Player.deck.Count == 0 || Opponent.deck.Count == 0)
                {
                    // Ends loop if one player has no more cards in his deck
                    break;
                }

                player_index = rand.Next(Player.deck.Count);
                opponent_index = rand.Next(Opponent.deck.Count);

                if (Player.deck[player_index].Type == Opponent.deck[opponent_index].Type) //Fight of same CardType (Monster or Spell)
                {
                    if (Player.deck[player_index].Type == CardType.MonsterCard) //Fight of two Monsters
                    {
                        MonsterFight monsterFight = new MonsterFight();

                        int FightOutcome = monsterFight.Fight(Player, Opponent, player_index, opponent_index);

                        if(FightOutcome == 1)
                        {
                            Console.WriteLine(Player.Name + " card is better:\n");
                            Console.WriteLine(Player.deck[player_index].Name + " wins against " + Opponent.deck[opponent_index].Name + "\n");
                            // Player wins, add opponent's card to the player's deck and remove it from your opponent's deck.
                            Player.deck.Add(Opponent.deck[opponent_index]);
                            Opponent.deck.RemoveAt(opponent_index);
                        }
                        else if (FightOutcome == -1) 
                        {
                            Console.WriteLine(Opponent.Name + " card is better:\n");
                            Console.WriteLine(Player.deck[player_index].Name + " looses against " + Opponent.deck[opponent_index].Name + "\n");
                            // Opponent wins, add the player's card to the opponent's deck and remove it from the player's deck.
                            Opponent.deck.Add(Player.deck[player_index]);
                            Player.deck.RemoveAt(player_index);
                        }
                        else //FightOutcome == 0
                        {
                            Console.WriteLine("AMAZING\n");
                            Console.WriteLine(Player.deck[player_index].Name + " is just as strong as " + Opponent.deck[opponent_index].Name + ", nothing happends \n");
                        }

                        Balance += FightOutcome;
                    }
                    else if (Player.deck[player_index].Type == CardType.SpellCard)
                    {
                        SpellFight spellFight = new SpellFight();

                        int FightOutcome = spellFight.Fight(Player, Opponent, player_index, opponent_index);

                        if (FightOutcome == 1)
                        {
                            Console.WriteLine(Player.Name + " card is better:\n");
                            Console.WriteLine(Player.deck[player_index].Name + " wins against " + Opponent.deck[opponent_index].Name + "\n");
                            // Player wins, add opponent's card to the player's deck and remove it from your opponent's deck.
                            Player.deck.Add(Opponent.deck[opponent_index]);
                            Opponent.deck.RemoveAt(opponent_index);
                        }
                        else if (FightOutcome == -1)
                        {
                            Console.WriteLine(Opponent.Name + " card is better:\n");
                            Console.WriteLine(Player.deck[player_index].Name + " looses against " + Opponent.deck[opponent_index].Name + "\n");
                            // Opponent wins, add the player's card to the opponent's deck and remove it from the player's deck.
                            Opponent.deck.Add(Player.deck[player_index]);
                            Player.deck.RemoveAt(player_index);
                        }
                        else //FightOutcome == 0
                        {
                            Console.WriteLine("AMAZING\n");
                            Console.WriteLine(Player.deck[player_index].Name + " is just as strong as " + Opponent.deck[opponent_index].Name + ", nothing happends \n");
                        }

                        Balance += FightOutcome;
                    }
                        
                }
                else
                {
                    MixedFight mixedFight = new MixedFight();
                    int FightOutcome = mixedFight.Fight(Player, Opponent, player_index, opponent_index);

                    if (FightOutcome == 1)
                    {
                        Console.WriteLine(Player.Name + " card is better:\n");
                        Console.WriteLine(Player.deck[player_index].Name + " wins against " + Opponent.deck[opponent_index].Name + "\n");
                        // Player wins, add opponent's card to the player's deck and remove it from your opponent's deck.
                        Player.deck.Add(Opponent.deck[opponent_index]);
                        Opponent.deck.RemoveAt(opponent_index);
                    }
                    else if (FightOutcome == -1)
                    {
                        Console.WriteLine(Opponent.Name + " card is better:\n");
                        Console.WriteLine(Player.deck[player_index].Name + " looses against " + Opponent.deck[opponent_index].Name + "\n");
                        // Opponent wins, add the player's card to the opponent's deck and remove it from the player's deck.
                        Opponent.deck.Add(Player.deck[player_index]);
                        Player.deck.RemoveAt(player_index);
                    }
                    else //FightOutcome == 0
                    {
                        Console.WriteLine("AMAZING\n");
                        Console.WriteLine(Player.deck[player_index].Name + " is just as strong as " + Opponent.deck[opponent_index].Name + ", nothing happends \n");
                    }

                    Balance += FightOutcome;
                }
                rounds++;
            }
            Console.WriteLine("\n _____END_BATTLE_____");
            if (Balance == 0)
            {
                Console.WriteLine("It's a DRAW, no one wins!");
                return Tuple.Create(true, Player.UserId, Opponent.UserId);
            }
            else if(Balance > 0) 
            {
                Console.WriteLine("\n The WINNER is: " + Player.Name + "!");
                return Tuple.Create(false, Player.UserId, Opponent.UserId);
            }
            else //(Balance < 0)
            {
                Console.WriteLine("\n The WINNER is: " + Opponent.Name + "!");
                return Tuple.Create(false, Opponent.UserId, Player.UserId);
            }

        }
    }
}
