using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Generator
{
    internal class CardGenerator
    {
        readonly Random rand = new();
        private CardType _type;
        private ElementType _element;
        private Monsters _monster;
        private string _cardName;
        private int _damage;
        public Card CreateCard()
        {
            // Get all Enum Values
            CardType[] _cardTypes = (CardType[])Enum.GetValues(typeof(CardType));
            ElementType[] _elementTypes = (ElementType[])Enum.GetValues(typeof(ElementType));

            // create random indixes
            int cardTypeIndex = rand.Next(_cardTypes.Length);
            int elementTypeIndex = rand.Next(_elementTypes.Length);

            // Assign type and element for card (damage betwenn 0 and 50)
            _type = _cardTypes[cardTypeIndex];
            _element = _elementTypes[elementTypeIndex];
            _damage = rand.Next(50);

            if (_type == CardType.MonsterCard) //Monster Card
            {
                Monsters[] Monsters = (Monsters[])Enum.GetValues(typeof(Monsters));
                int monsterIndex = rand.Next(Monsters.Length);
                _monster = Monsters[monsterIndex];
                _cardName = $"{_element}-{_monster}";
                return new Card()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = _cardName,
                    Type = _type,
                    Element = _element,
                    Damage = _damage,
                    Monster = _monster
                };
            }
            else // Spell Card
            {
                _cardName = $"{_element}-{_type}";
                return new Card()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = _cardName,
                    Type = _type,
                    Element = _element,
                    Damage = _damage
                };
            }


        }
    }
}
