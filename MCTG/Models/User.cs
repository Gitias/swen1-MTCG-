using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }



        /*public string PasswordHash { get; set; }
        public CardStack CardCollection { get; set; }
        public Deck Deck { get; private set; }

        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
            CardCollection = new CardStack();
            Deck = new Deck();
        }*/

        // Weitere Methoden wie Kartenkauf, Deck-Management usw.
    }
}
