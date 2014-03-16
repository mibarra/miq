using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq
{
    public static partial class MoreMath
    {
        public static string ToRoman(this uint arabic)
        {
            StringBuilder result = new StringBuilder();

            uint[] digits = arabic.DigitsOf();
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                result.Append(Convert(digits[i], (uint)i));
            }
            return result.ToString();
        }

        // LATER Implement uint.ParseRoman - Parses a string containing roman numerals into an uint. 

        private static string Convert(uint arabic, uint exponent)
        {
            if (arabic == 0)
            {
                return string.Empty;
            }

            KeyValuePair<uint, string> convertion = Convertion(arabic, exponent);
            return convertion.Value + Convert(arabic - convertion.Key, exponent);
        }

        private static KeyValuePair<uint, string> Convertion(uint arabic, uint exponent)
        {
            if (exponent > 6 || (exponent == 6 && arabic > 3))
            {
                throw new ArgumentException("value is to big to be converted to roman numerals");
            }

            Dictionary<uint, string[]> primitiveNumbers = new Dictionary<uint, string[]>()
            {
                {0, new string[] {"I","V","X"}},
                {1, new string[] {"X","L","C"}},
                {2, new string[] {"C","D","M"}},
                {3, new string[] {"M","V̄","X̄"}},
                {4, new string[] {"X̄","L̄","C̄"}},
                {5, new string[] {"C̄","D̄","M̄"}},
                {6, new string[] {"M̄","?","?"}},
            };

            // kinda ugly
            List<KeyValuePair<uint, string>> convertions = new List<KeyValuePair<uint, string>>()
            {
                new KeyValuePair<uint, string>(9, primitiveNumbers[exponent][0] + primitiveNumbers[exponent][2]),
                new KeyValuePair<uint, string>(5, primitiveNumbers[exponent][1]),
                new KeyValuePair<uint, string>(4, primitiveNumbers[exponent][0] + primitiveNumbers[exponent][1]),
                new KeyValuePair<uint, string>(1, primitiveNumbers[exponent][0]),
            };

            KeyValuePair<uint, string> convertion = convertions.First(c => c.Key <= arabic);
            return convertion;
        }

    }
}
