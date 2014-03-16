using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Miq.Tests.Nursery
{
    public class Individual
    {
        public Individual(string genome)
        {
            Genotype = genome;
            Fitness = genome.Where((c, i) => c == Goal[i]).Count();
            Fitness *= Fitness;
        }

        public static string Goal = "to be or not to be that is the question";
        static string Abecedary = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ,.?\":'0123456789\n\r-";
        static Random Rng = new Random();

        public static Individual Random()
        {
            var chars = new Char[Goal.Length];
            for (int i = 0; i < Goal.Length; i++)
            {
                chars[i] = Abecedary[Rng.Next(Abecedary.Length)];
            }
            return new Individual(new string(chars));
        }

        public readonly string Genotype;
        public readonly double Fitness;

        internal static char RandomChar()
        {
            return Abecedary[Rng.Next(Abecedary.Length)];
        }
    }

    public class Population
    {
        public double TotalFitness { get; private set; }
        public double AverageFitness { get; private set; }
        public Individual BestIndividual { get; private set; }

        public int Capacity { get; private set; }

        public Population(int capacity)
        {
            Capacity = capacity;
            Individuals = new Dictionary<string, Individual>(capacity);
        }

        public void Add(Individual individual)
        {
            if (Individuals.ContainsKey(individual.Genotype))
                return;

            Individuals.Add(individual.Genotype, individual);

            if (BestIndividual == null || individual.Fitness > BestIndividual.Fitness)
                BestIndividual = individual;

            TotalFitness += individual.Fitness;
            AverageFitness = TotalFitness / Individuals.Count();

            CheckPopulationSize();
        }

        Random Rng = new Random();

        public Individual PickFromTop()
        {
            double p = Rng.NextDouble();
            double cumulativeProbability = 0.0;
            foreach (var i in Individuals)
            {
                cumulativeProbability += i.Value.Fitness / TotalFitness;
                if (p <= cumulativeProbability)
                    return i.Value;
            }

            return Individuals.First().Value;
        }

        public Individual PickFromBottom()
        {
            Individual picked = Individuals.Values.First();
            foreach (var i in Individuals.Values)
            {
                if (i.Fitness < picked.Fitness)
                    picked = i;
            }
            return picked;
        }

        void CheckPopulationSize()
        {
            if (Individuals.Count <= Capacity)
                return;

            Remove(PickFromBottom());
        }

        void Remove(Individual individual)
        {
            TotalFitness -= individual.Fitness;
            AverageFitness = TotalFitness / Individuals.Count;
            Individuals.Remove(individual.Genotype);
            if (individual == BestIndividual)
                FindBestIndividual();
        }

        private void FindBestIndividual()
        {
            BestIndividual = Individuals.Values.First();
            foreach (var item in Individuals.Values)
            {
                if (item.Fitness > BestIndividual.Fitness)
                {
                    BestIndividual = item;
                }
            }
        }

        Dictionary<string, Individual> Individuals;
    }

    public class GeneticAlgorithm
    {
        public Population Population { get; set; }

        public GeneticAlgorithm(int populationSize)
        {
            Population = new Population(populationSize);
            for (int i = 0; i < populationSize; i++)
                Population.Add(Individual.Random());
        }

        Random Rng = new Random();

        public void Step()
        {
            Generation++;
            Individual newIndividual;
            if (Rng.NextDouble() < 0.1)
            {
                var parent = Population.PickFromTop();
                var chars = parent.Genotype.ToArray();
                var charToMutate = Rng.Next(chars.Length);
                chars[charToMutate] = Individual.RandomChar();
                newIndividual = new Individual(new string(chars));
            }
            else
            {
                newIndividual = Crossover(Population.PickFromTop(), Population.PickFromTop());
            }

            Population.Add(newIndividual);
        }

        private Individual Crossover(Individual a, Individual b)
        {
            var chars = new char[a.Genotype.Length];
            for (int i = 0; i < a.Genotype.Length; i++)
            {
                chars[i] = Rng.NextDouble() < 0.5 ? a.Genotype[i] : b.Genotype[i];
            }
            return new Individual(new String(chars));
        }

        int Generation = 0;

        public void PrintStatistics()
        {
            Console.Clear();
            Console.WriteLine("{0} {1:0.000} {2} {3}", Generation, Population.AverageFitness, Population.BestIndividual.Fitness, Population.BestIndividual.Genotype);
        }
    }

    [TestClass]
    public class GeneticAlgorithmTests
    {
   
    }
}
