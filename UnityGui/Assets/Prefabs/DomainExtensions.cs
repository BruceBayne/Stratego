using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Prefabs
{
  public static  class DomainExtensions
    {

        public static (int, int) ToXandY(this StrategoTypes.FigurePosition position)
        {
            var x = position.Get.Item1;
            var y = position.Get.Item2;
            return (x, y);
        }

    }
}
