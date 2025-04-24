using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib.Tests
{
   
    public class PrimePartitionPopcount
    {
        private static readonly List<int> Primes = GeneratePrimes(50033); // 5136 primes
        private static readonly Random Rng = new();

        private const int GroupSize = 4;
        private const int GroupCount = 5136 / GroupSize;
        private const int PopulationSize = 100;
        private const int Generations = 500;

        public static void Run()
        {
            var population = Enumerable.Range(0, PopulationSize)
                                       .Select(_ => GenerateRandomGenome())
                                       .ToList();

            var best = population.OrderBy(GetFitness).First();
            Console.WriteLine($"Initial fitness: {GetFitness(best)}");

            for (int gen = 0; gen < Generations; gen++)
            {
                population = population.OrderBy(GetFitness).ToList();
                var elite = population.Take(10).ToList();

                var nextGen = new List<List<int>>(elite);
                while (nextGen.Count < PopulationSize)
                {
                    var p1 = population[Rng.Next(elite.Count)];
                    var p2 = population[Rng.Next(elite.Count)];
                    var child = Crossover(p1, p2);
                    Mutate(child);
                    nextGen.Add(child);
                }

                var candidate = nextGen.OrderBy(GetFitness).First();
                if (GetFitness(candidate) < GetFitness(best))
                {
                    best = candidate;
                    Console.WriteLine($"[Gen {gen}] Fitness: {GetFitness(best)}");
                }

                population = nextGen;
            }

            DisplayBestGrouping(best);
        }

        private static int GetFitness(List<int> genome)
        {
            int totalPop = 0;
            for (int i = 0; i < genome.Count; i += GroupSize)
            {
                long product = 1;
                for (int j = 0; j < GroupSize; j++)
                    product *= genome[i + j];

                totalPop += BitOperations.PopCount((ulong)product);
            }
            return totalPop;
        }

        private static List<int> GenerateRandomGenome()
        {
            return Primes.OrderBy(_ => Rng.Next()).ToList();
        }

        private static List<int> Crossover(List<int> p1, List<int> p2)
        {
            var child = new List<int>(p1);
            int swaps = Rng.Next(5, 15);
            for (int i = 0; i < swaps; i++)
            {
                int a = Rng.Next(child.Count);
                int b = Rng.Next(child.Count);
                (child[a], child[b]) = (child[b], child[a]);
            }
            return child;
        }

        private static void Mutate(List<int> genome)
        {
            if (Rng.NextDouble() < 0.2)
            {
                int i = Rng.Next(genome.Count);
                int j = Rng.Next(genome.Count);
                (genome[i], genome[j]) = (genome[j], genome[i]);
            }
        }

        private static void DisplayBestGrouping(List<int> best)
        {
            Console.WriteLine("\nBest grouping:");
            int totalPop = 0;
            for (int i = 0; i < best.Count; i += GroupSize)
            {
                var group = best.Skip(i).Take(GroupSize).ToList();
                long product = group.Aggregate(1L, (a, b) => a * b);
                int pop = BitOperations.PopCount((ulong)product);
                Console.WriteLine($"{string.Join(",", group)} → {product} (popcount = {pop})");
                totalPop += pop;
            }
            Console.WriteLine($"\nTotal popcount: {totalPop}");
        }

        private static List<int> GeneratePrimes(int max)
        {
            var sieve = new bool[max + 1];
            for (int i = 2; i <= max; i++) sieve[i] = true;
            for (int i = 2; i * i <= max; i++)
            {
                if (!sieve[i]) continue;
                for (int j = i * i; j <= max; j += i)
                    sieve[j] = false;
            }
            return Enumerable.Range(2, max - 1).Where(i => sieve[i]).ToList();
        }
    }

    [TestClass]
    public class GeneticPrimePopulation
    {
        [TestMethod]
        public void RunGeneticPrimePopulation()
        {
            PrimePartitionPopcount.Run();

        }
    }
}
