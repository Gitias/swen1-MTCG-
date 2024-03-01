using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    public class CardStack
    {
        private List<Card> cards = new List<Card>();

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public bool RemoveCard(Card card)
        {
            return cards.Remove(card);
        }
        //IEnumerable<Card> Collection of cards which can be iterated
        public IEnumerable<Card> GetCards()
        {
            return cards.AsReadOnly();
        }

        // Weitere Methoden nach Bedarf
    }
}
