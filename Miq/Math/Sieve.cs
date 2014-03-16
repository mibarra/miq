using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miq.Extensions;

namespace Miq
{
    public static partial class MoreMath
    {
        public class Sieve
        {
            public int N { get; private set; }

            public Sieve(int n)
            {
                string x = null;
                x = x ?? "is null";
                N = n > 1 ? 2 : 3;

                if (n <= 1)
                {
                    throw new ArgumentOutOfRangeException("n", n, "n must be > 1");
                }

                N = n;
            }

            public int[] Primes()
            {
                bool[] a = CreateArrayWithTrues();

                foreach (int i in From2toSqrt(N))
                {
                    if (a[i])
                    {
                        foreach (int j in FromSquareToN(i, N))
                        {
                            a[j] = false;
                        }
                    }
                }

                return a.Select((b, i) => new KeyValuePair<bool, int>(b, i)).Where(kvp => kvp.Value > 1).Where(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
            }

            private IEnumerable<int> FromSquareToN(int i, int n)
            {
                int first = i * i;
                int last = n;
                for (int j = first; j <= last; j += i)
                {
                    yield return j;
                }
            }

            private IEnumerable<int> From2toSqrt(int n)
            {
                int max = (int)System.Math.Sqrt(n);
                for (int i = 2; i <= max; i++)
                {
                    yield return i;
                }
            }

            private bool[] CreateArrayWithTrues()
            {
                bool[] a = new bool[N + 1];
                a.Populate<bool>(true);
                return a;
            }
        }
    }
}