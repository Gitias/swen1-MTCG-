using MCTG.Models;
using MCTG.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTG.Logic;

namespace MCTG_Test
{
    public class GameLogictests
    {
        [Test] //# 17 
        public void SpellFight_TestEffectMultiplier()
        {
            // Arrange
            SpellFight spellFight = new();

            //Act - Effective Element first
            (float WaterVsFire, float FireVsWater) = spellFight.CalculateEffect(ElementType.Water, ElementType.Fire);
            (float FireVsNormal, float NormalVsFire) = spellFight.CalculateEffect(ElementType.Fire, ElementType.Normal);
            (float WaterVsNormal, float NormalVsWater) = spellFight.CalculateEffect(ElementType.Water, ElementType.Normal);

            //Assert
            Assert.That(WaterVsFire, Is.EqualTo(2.0f));
            Assert.That(FireVsWater, Is.EqualTo(0.5f));
            Assert.That(WaterVsNormal, Is.EqualTo(0.5f));
            Assert.That(NormalVsWater, Is.EqualTo(2.0f));
            Assert.That(NormalVsFire, Is.EqualTo(0.5f));
            Assert.That(FireVsNormal, Is.EqualTo(2.0f));

            //Act - Ineffective First
            (FireVsWater, WaterVsFire) = spellFight.CalculateEffect(ElementType.Fire, ElementType.Water);
            (NormalVsWater, WaterVsNormal) = spellFight.CalculateEffect(ElementType.Normal, ElementType.Water);
            (FireVsNormal, NormalVsFire) = spellFight.CalculateEffect(ElementType.Fire, ElementType.Normal);

            //Assert - Should remain unchanged
            Assert.That(WaterVsFire, Is.EqualTo(2.0f));
            Assert.That(FireVsWater, Is.EqualTo(0.5f));
            Assert.That(WaterVsNormal, Is.EqualTo(0.5f));
            Assert.That(NormalVsWater, Is.EqualTo(2.0f));
            Assert.That(NormalVsFire, Is.EqualTo(0.5f));
            Assert.That(FireVsNormal, Is.EqualTo(2.0f));
        }
        [Test] //# 18 
        public void MixedFight_TestEffectMultiplier()
        {
            // Arrange
            MixedFight mixedFight = new();

            //Act - Effective Element first
            (float WaterVsFire, float FireVsWater) = mixedFight.CalculateEffect(ElementType.Water, ElementType.Fire);
            (float FireVsNormal, float NormalVsFire) = mixedFight.CalculateEffect(ElementType.Fire, ElementType.Normal);
            (float WaterVsNormal, float NormalVsWater) = mixedFight.CalculateEffect(ElementType.Water, ElementType.Normal);

            //Assert
            Assert.That(WaterVsFire, Is.EqualTo(2.0f));
            Assert.That(FireVsWater, Is.EqualTo(0.5f));
            Assert.That(WaterVsNormal, Is.EqualTo(0.5f));
            Assert.That(NormalVsWater, Is.EqualTo(2.0f));
            Assert.That(NormalVsFire, Is.EqualTo(0.5f));
            Assert.That(FireVsNormal, Is.EqualTo(2.0f));

            //Act - Ineffective First
            (FireVsWater, WaterVsFire) = mixedFight.CalculateEffect(ElementType.Fire, ElementType.Water);
            (NormalVsWater, WaterVsNormal) = mixedFight.CalculateEffect(ElementType.Normal, ElementType.Water);
            (FireVsNormal, NormalVsFire) = mixedFight.CalculateEffect(ElementType.Fire, ElementType.Normal);

            //Assert - Should remain unchanged
            Assert.That(WaterVsFire, Is.EqualTo(2.0f));
            Assert.That(FireVsWater, Is.EqualTo(0.5f));
            Assert.That(WaterVsNormal, Is.EqualTo(0.5f));
            Assert.That(NormalVsWater, Is.EqualTo(2.0f));
            Assert.That(NormalVsFire, Is.EqualTo(0.5f));
            Assert.That(FireVsNormal, Is.EqualTo(2.0f));
        }

        [Test] //#19
        public void TestBattle_PlayerWin()
        {
            //Arrange
            GameLogic game = new GameLogic();
            Deck Player_Deck = new Deck()
            {
                UserId = 999999,
                Name = "Player"
            };
            Deck Opponent_Deck = new Deck()
            {
                UserId = 999991,
                Name = "Opponent"
            };

            for (int i = 0; i < 4; i++) 
            {
                Player_Deck.deck.Add(
                    new Card()
                    {
                        Name = "BetterCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 49f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999999,
                        InDeck = true
                    }
                    );
            }
            for (int i = 0; i < 4; i++)
            {
                Opponent_Deck.deck.Add(
                    new Card()
                    {
                        Name = "WortCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 1f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999991,
                        InDeck = true
                    }
                    );
            }
            (bool draw, int winner, int looser) = game.Battle(Player_Deck, Opponent_Deck);
            Assert.That(draw, Is.EqualTo(false));
            Assert.That(winner, Is.EqualTo(999999));
            Assert.That(looser, Is.EqualTo(999991));
        }
        [Test] //#20
        public void TestBattle_OpponentWin()
        {
            //Arrange
            GameLogic game = new GameLogic();
            Deck Player_Deck = new Deck()
            {
                UserId = 999999,
                Name = "Player"
            };
            Deck Opponent_Deck = new Deck()
            {
                UserId = 999991,
                Name = "Opponent"
            };

            for (int i = 0; i < 4; i++)
            {
                Opponent_Deck.deck.Add(
                    new Card()
                    {
                        Name = "BetterCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 49f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999999,
                        InDeck = true
                    }
                    );
            }
            for (int i = 0; i < 4; i++)
            {
                Player_Deck.deck.Add(
                    new Card()
                    {
                        Name = "WortCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 1f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999991,
                        InDeck = true
                    }
                    );
            }
            (bool draw, int winner, int looser) = game.Battle(Player_Deck, Opponent_Deck);
            Assert.That(draw, Is.EqualTo(false));
            Assert.That(winner, Is.EqualTo(999991));
            Assert.That(looser, Is.EqualTo(999999));
        }

        [Test] //#21
        public void TestBattle_DrawReturn()
        {
            //Arrange
            GameLogic game = new GameLogic();
            Deck Player_Deck = new Deck()
            {
                UserId = 999999,
                Name = "Player"
            };
            Deck Opponent_Deck = new Deck()
            {
                UserId = 999991,
                Name = "Opponent"
            };

            for (int i = 0; i < 4; i++)
            {
                Opponent_Deck.deck.Add(
                    new Card()
                    {
                        Name = "BetterCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 25f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999999,
                        InDeck = true
                    }
                    );
            }
            for (int i = 0; i < 4; i++)
            {
                Player_Deck.deck.Add(
                    new Card()
                    {
                        Name = "WortCard",
                        Type = CardType.MonsterCard,
                        Monster = Monsters.Wizzard,
                        Damage = 25f,
                        Id = Guid.NewGuid().ToString(),
                        UserId = 999991,
                        InDeck = true
                    }
                    );
            }
            (bool draw, int winner, int looser) = game.Battle(Player_Deck, Opponent_Deck);
            Assert.That(draw, Is.EqualTo(true));
        }

        [Test] //#22
        public void TestCanAttach_Monster_MixedFight()
        {
            //Arrange
            MonsterFight monsterFight = new MonsterFight();
            MixedFight mixedFight = new MixedFight();

            Card Player_one = new()
            {
                Name = "TestCard_PlayerOne",
                Type = CardType.MonsterCard,
                Monster = Monsters.Wizzard,
                Damage = 25f,
                Id = Guid.NewGuid().ToString(),
                UserId = 333333333,
                InDeck = true
            };
            Card Player_two = new()
            {
                Name = "TestCard_PlayerTwo",
                Type = CardType.MonsterCard,
                Monster = Monsters.Wizzard,
                Damage = 25f,
                Id = Guid.NewGuid().ToString(),
                UserId = 66666666,
                InDeck = true
            };
            Assert.That(monsterFight.CanAttack(Player_one, Player_two), Is.True);
            Player_two.Monster = Monsters.Ork;
            Assert.That(monsterFight.CanAttack(Player_one, Player_two), Is.True);
            Assert.That(monsterFight.CanAttack(Player_two, Player_one), Is.False);
            Player_one.Monster = Monsters.Goblin;
            Player_two.Monster = Monsters.Dragon;
            Assert.That(monsterFight.CanAttack(Player_one, Player_two), Is.False);
            Assert.That(monsterFight.CanAttack(Player_two, Player_one), Is.True);
            Player_one.Monster = Monsters.Knight;
            Player_two.Type = CardType.SpellCard;
            Player_two.Element = ElementType.Water;
            Assert.That(mixedFight.CanAttack(Player_one, Player_two), Is.False);
            Assert.That(mixedFight.CanAttack(Player_two, Player_one), Is.True);
            Player_one.Monster = Monsters.Kraken;
            Assert.That(mixedFight.CanAttack(Player_one, Player_two), Is.True);
            Assert.That(mixedFight.CanAttack(Player_two, Player_one), Is.False);
            Player_one.Monster = Monsters.Elve;
            Player_one.Element = ElementType.Fire;
            Player_two.Type = CardType.MonsterCard;
            Player_two.Monster = Monsters.Dragon;
            Assert.That(monsterFight.CanAttack(Player_one, Player_two), Is.True);
            Assert.That(monsterFight.CanAttack(Player_two, Player_one), Is.False);

        }

    }
}
