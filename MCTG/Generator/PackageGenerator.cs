using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Generator
{
    internal class PackageGenerator
    {
        public static Package CreatePackage()
        {
            CardGenerator cardGen = new();
            Package package = new();

            for (int i = 0; i < 5; i++)
            {
                Card newCard = cardGen.CreateCard();
                package.cards.Add(newCard);
            }
            if (package.cards.Count == 5)
            {
                return package;
            }
            else
            {
                throw new NotFoundEx("Package konnte nicht erstellt werden");
            }
        }
    }
}
