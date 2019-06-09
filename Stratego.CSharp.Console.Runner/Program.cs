using System;
using System.Linq;
using System.Threading;
using Stratego.Logic;

namespace Stratego.CSharp.Console.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = StrategoLogic.StartNewGame();
            
            var fso = StrategoTypes.FigurePosition.Create(0, 0);



            var fso2 = StrategoTypes.FigurePosition.Create(1, 3);
            

            var moves = StrategoLogic.CalculateAvailableMoves(x.GameField.Field, fso.Value).ResultValue.ToList();



            var tsi=StrategoLogic.MakeMove(x,
                new StrategoTypes.MoveIntent(StrategoTypes.Player.Blue, fso.Value, moves.First()));

            
            




            var invalid = StrategoLogic.CalculateAvailableMoves(x.GameField.Field, fso2.Value).ErrorValue;


            
            
        }
    }
}

