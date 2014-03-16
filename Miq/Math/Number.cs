using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq
{
    public static partial class MoreMath
    {
        public static uint NumberOfDigits(this uint n)
        {
            // LATER look for a bit twiddling hack to make this faster. Do a speed profilling first!
            return (uint)System.Math.Floor(System.Math.Log10((double)n));
        }

        public static uint[] DigitsOf(this uint n)
        {
            // LATER Profile this, can be made to run faster.
            return n.ToString().ToCharArray().Reverse().Select(d => uint.Parse(d.ToString())).ToArray();
        }
    }
}
