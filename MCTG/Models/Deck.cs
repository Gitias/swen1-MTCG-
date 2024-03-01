using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    internal class Deck
    {

        public int UserId {  get; set; }
        public string? Name { get; set; }
        public List<Card> deck {  get; set; } = new List<Card>();
        /*private const int MaxDeckSize = 4;
        private List<Card> CardDeck = new List<Card>();

        public void AddCardToDeck(Card card)
        {
            if (CardDeck.Count < MaxDeckSize)
            {
                CardDeck.Add(card);
            }
            else
            {
                throw new InvalidOperationException("Deck is full.");
            }
        }

        public bool RemoveCardFromDeck(Card card)
        {
            return CardDeck.Remove(card);
        }
        //IEnumerable<Card> Collection of cards which can be iterated
        public IEnumerable<Card> GetCardDeck()
        {
            return CardDeck.AsReadOnly();
        }

        // Weitere Methoden nach Bedarf*/
    }
}
