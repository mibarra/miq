using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Endo
{
    class Dna
    {
        public Dna (char b)
	    {
            dna = new String(b, 1);
	    }

        public char Consume()
        {
            char b = dna[0];
            dna = dna.Substring(1);
            return b;
        }

        string dna;

        public void Prepend(char prefix)
        {
            dna = prefix + dna;
        }
    }

    class Endo
    {
        Dna Dna;

        int Nat()
        {
            int n;
            int result = 0;
            char b = Dna.Consume();

            if (b == 'I' || b == 'F')
            {
                n = Nat();
                result = 2 * n;
            }
            else if (b == 'C')
            {
                n = Nat();
                result = 2 * n + 1;
            }

            return result;
        }

        Dna AsNat(int n)
        {
            if (n == 0)
            {
                return new Dna('P');
            }

            int half = n / 2;
            Dna dna = AsNat(half);
            char prefix = n % 2 == 0 ? 'I' : 'C';
            dna.Prepend(prefix);

            return dna; 
        }
    }
}
