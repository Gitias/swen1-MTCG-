using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    //abstract --> Only base calss for Monster and Spellcards
    public class Card
    {
          public string Name { get; set; }
          public CardType Type { get; set; }
          public ElementType Element { get; set; }
          public Monsters? Monster { get; set; }
          public float Damage { get; set; }
          public string Id { get; set; }
          public int? UserId { get; set; }
          public bool? InDeck { get; set; }


    }
}
