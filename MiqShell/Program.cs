using Miq.Tests.Nursery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            Individual.Goal = @"
To be, or not to be, that is the question:
Whether 'tis Nobler in the mind to suffer
The Slings and Arrows of outrageous Fortune,
Or to take Arms against a Sea of troubles,
And by opposing end them: to die, to sleep
No more; and by a sleep, to say we end
The Heart-ache, and the thousand Natural shocks
That Flesh is heir to? 'Tis a consummation
";
            var ga = new GeneticAlgorithm(5000);
            for (int i = 0; i < 100000000; i++)
            {
                ga.Step();
                ga.PrintStatistics();
                if (ga.Population.BestIndividual.Fitness == Individual.Goal.Length)
                    break;
            }

            Console.ReadLine();
        }
    }
}
