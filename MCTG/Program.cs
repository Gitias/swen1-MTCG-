// See https://aka.ms/new-console-template for more information
using MCTG.Models;
using MCTG.Server;

internal class Programm
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        ServerInit.StartServer();
    }
}
    
 //Hier sind ein Paar Spielereien damit ich weiß wie ich mit den Klassen Interagieren kann.
 /*
    Card Mage = new Card("Magier", CardType.MonsterCard, 20, ElementType.Fire, Species.Wizzard);
    Card Viking = new Card("Vikinger Häuptling", CardType.MonsterCard, 25, ElementType.Normal, Species.Knight);
    Card WaterDrake = new Card("Wasserdrache", CardType.MonsterCard, 60, ElementType.Water, Species.Dragon);
    Card Fireball = new Card("Feuerball", CardType.SpellCard, 45, ElementType.Fire, Species.Spell);
    Card unknown = new Card("Unknown", CardType.SpellCard, 99, ElementType.Normal, Species.Spell);
    Card SchwarzerRitter = new Card("Black Knight", CardType.MonsterCard, 65, ElementType.Normal, Species.Knight);


    Console.WriteLine(Mage.Name + " " + Mage.ElementType + Mage.Species+ " " + Mage.Damage);
    Console.WriteLine(Viking.Name + " " + Viking.ElementType + Viking.Species + " " + Viking.Damage);


Deck Player1 = new Deck();
Player1.AddCardToDeck(Mage);
Player1.AddCardToDeck(Mage);

Deck Player2 = new Deck();
Player2.AddCardToDeck(Viking);

Console.WriteLine("Card in Deck PL1:");

foreach (var card in Player1.GetCardDeck())
{
    Console.WriteLine(card.Name + " " + card.ElementType + card.Species + " " + card.Damage);
}

Console.WriteLine("Card in Deck PL2:");

foreach (var card in Player2.GetCardDeck())
{
    Console.WriteLine(card.Name + " " + card.ElementType + card.Species + " " + card.Damage);
}

Player1.RemoveCardFromDeck(Mage);

Console.WriteLine("Nach Entfernen einer Karte:");

foreach (var card in Player1.GetCardDeck())
{
    Console.WriteLine(card.Name + " " + card.ElementType + card.Species + " " + card.Damage);
}

User Tobias = new User("Tobias", "TobiasPW");
Tobias.CardCollection.AddCard(Viking);
Tobias.CardCollection.AddCard(Fireball);
Tobias.CardCollection.AddCard(WaterDrake);
Tobias.CardCollection.AddCard(Viking);
Tobias.CardCollection.AddCard(Fireball);
Tobias.CardCollection.AddCard(WaterDrake);
Tobias.CardCollection.AddCard(unknown);
Tobias.CardCollection.AddCard(SchwarzerRitter);
Console.WriteLine("Ausgabe des Gesamten cardstacks von user Tobias:");
foreach(var card in Tobias.CardCollection.GetCards())
{
    Console.WriteLine(card.Name + " " + card.ElementType + card.Species + " " + card.Damage + " " + card.CardType);
}
Console.WriteLine("Ausgabe des Gesamten Decks von user Tobias:");
Tobias.Deck.AddCardToDeck(WaterDrake);
Tobias.Deck.AddCardToDeck(Viking);
Tobias.Deck.AddCardToDeck(Fireball);
foreach (var Card in Tobias.Deck.GetCardDeck())
{
    Console.WriteLine(Card.Name + " " + Card.CardType + " " + Card.Damage + " " + Card.ElementType);
}
 */
 