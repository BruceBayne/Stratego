using System;
using Stratego.Logic;

namespace Stratego.CSharp.Console.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           


            var x=StrategoLogic.StartNewGame();
           

            var fso = StrategoTypes.FigurePosition.Create(0, 0);
           var moves= StrategoLogic.CalculateAvailableMoves(x.GameField.Field, fso.Value);





        }
    }
}