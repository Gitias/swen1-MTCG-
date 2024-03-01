using MCTG.Models;
using MCTG.Generator;
namespace MCTG_Test
{
    public class GeneratorTests
    {
        //Arrange
        //Act
        //Assert
        [Test] // #1
        public void PackageGenerator_CreatePackage_ReturnsValidPackageObjet()
        {
            //Arrange
            var package = new Package();
            //ACT
            package = PackageGenerator.CreatePackage();
            //Assert
            Assert.IsNotNull(package);
            Assert.AreEqual(5, package.cards.Count);
            foreach(Card card in package.cards)
            {
                Assert.IsNotNull(card);
                Assert.IsNotNull(card.Id);
                Assert.IsNotEmpty(card.Id);
                Assert.IsNotNull(card.Name);
                Assert.IsNotEmpty(card.Name);
                Assert.IsNotNull(card.Type);
                Assert.IsNotNull(card.Element);
                Assert.IsNotNull(card.Damage);

                if(card.Type == CardType.MonsterCard)
                {
                    Assert.IsNotNull(card.Monster);
                }
                else
                {
                    Assert.IsNull(card.Monster);
                }
            }

        }

        [Test] //#2
        public void CardGenerator_CreateCard_ReturnsValidCardObject()
        {
            //Arrange
            var cardGen = new CardGenerator();
            var card = new Card();
            //Act
            card = cardGen.CreateCard();
            //Assert
            Assert.IsNotNull(card);
            Assert.IsNotNull(card.Id);
            Assert.IsNotEmpty(card.Id);
            Assert.IsNotNull(card.Name);
            Assert.IsNotEmpty(card.Name);
            Assert.IsNotNull(card.Type);
            Assert.IsNotNull(card.Element);
            Assert.IsNotNull(card.Damage);

            if (card.Type == CardType.MonsterCard)
            {
                Assert.IsNotNull(card.Monster);
            }
            else
            {
                Assert.IsNull(card.Monster);
            }
        }
    }
}