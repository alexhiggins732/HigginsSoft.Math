
using HigginsSoft.Math.Lib;
using HigginsSoft.Math.UI.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Numerics;
using static HigginsSoft.Math.Lib.MathLib;
using static HigginsSoft.Math.Lib.s;
using static MudBlazor.Defaults;

namespace HigginsSoft.Math.UI.Models
{
    public class Class
    {
        public int Class;
        public int Res;

        public Class(int @class, int res)
        {
            Class = @class;
            Res = res;
        }
    }
}
namespace HigginsSoft.Math.UI.Pages
{

    public partial class NTables
    {
        private string nString = "10";
        private string String = "0";
        private string ClassString = "0";
        public BigInteger N { get; set; }

        public List<Class> Classes = new();
        List<(long p, long q)> solutions = new();

        public List<Components.Class> Components = new();

        public string NString
        {
            get => nString;
            set
            {
                if (BigInteger.TryParse(value, out BigInteger n))
                {
                    nString = value;
                    foreach (var c in Classes)
                    {
                        c. = (int)(n % c.Class);

                    }
                    //var res = N % n;
                    //this.ClassString = res.ToString();
                    //N = n;

                }
            }

        }

        public string ClassString
        {
            get => ClassString;
            set
            {
                if (int.TryParse(value, out int))
                {
                    ClassString = value;
                    if (BigInteger.TryParse(nString, out BigInteger n))
                    {
                        var res = n % ;
                        String = res.ToString();
                    }
                    // = ;

                }
            }
        }

        public string String
        {
            get => String;
            set
            {
                if (int.TryParse(value, out int))
                {
                    String = value;
                }
            }
        }
        void Remove(Class Class)
        {
            // Method to be executed when the child component's button is clicked
            Classes.Remove(Class);
            Classes = Classes.OrderBy(x => x.Class).ToList();
            var comp = Components.First(x => x.Value == Class);
            Components.Remove(comp);
            StateHasChanged();
        }


        void FilterSolutions(Class Class)
        {
            // Method to be executed when the child component's button is clicked
            var comp = Components.First(x => x.Value == Class);
            var otherComponents = Components.Where(x => x.Value != Class).ToList();
            var solutions = comp.solutions;

            bool removed = true;
            int removedCount = 0;
            while (removed)
            {
                removed = false;
                foreach (var sol in solutions)
                {
                    bool found = false;
                    foreach (var other in otherComponents)
                    {
                        var pmod = sol.p % other.Value.Class;
                        var qMod = sol.q % other.Value.Class;

                        found = false;
                        foreach (var othersol in other.solutions)
                        {
                            if (pmod == othersol.p && qMod == othersol.q)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            Console.WriteLine($"Removed solution: {sol.p} - {sol.q} mod {comp.Value.Class} - not found in {other.Value.Class}");
                            break;
                        }

                    }
                    if (!found)
                    {
                        removed = true;
                        solutions.Remove(sol);
                        removedCount++;
                        break;
                    }
                    if (removed)
                        break;
                }
            }

            Console.WriteLine($"Filtered {removedCount} s classes");
            StateHasChanged();
        }

        void ClearClasses()
        {
            Classes.Clear();
            Components.Clear();
        }
        protected override async Task OnInitializedAsync()
        {
            //rsa(260)
            nString = "1804494070555133551597086232012760938085832447464768169982721327473606041236239";
            /*
             P39 = 395949542754668322591943478289006315011
             P40 = 4557383897960968698166467601610769663749
             */
            var n = BigInteger.Parse(nString);
            var classes = new[] { 5, 6, 7, 8, 9, 10, 11, 20, 30, 36, 40, 50, 60, 70, 80, 90, 100 };//, 210 * 3 * 5 * 7 * 11 * 17, 360, 290 };
            LoadClasses(classes);
            await base.OnInitializedAsync();
        }

        void AddClass()
        {
            int @class = int.Parse(ClassString);
            int  = int.Parse(String);
            Classes.Add(new(@class, ));
            Classes = Classes.OrderBy(x => x.Class).ToList();
        }


        public long SolutionSteps { get; set; } = 0;
        public long SolutionValue { get; set; } = 0;

        async Task ShowSolutionsAsync()
        {
            var t = Task.Run(() => ShowSolutions());
            while (!t.IsCompleted)
            {
                await Task.Delay(100);
                StateHasChanged();
            }
        }

        public int SmallPrimeCount { get; set; }

        void LoadSmallPrimes()
        {
            var idx = Array.IndexOf(Primes.IntFactorPrimes, SmallPrimeCount);
            if (idx > 0)
            {
                var primes = Primes.IntFactorPrimes.Take(idx + 1);
                LoadClasses(primes);
            }
        }
        void LoadClasses(IEnumerable<int> classes)
        {

            if (BigInteger.TryParse(NString, out BigInteger n))
            {
                ClearClasses();
                foreach (var c in classes)
                {
                    var res = n % c;
                    Classes.Add(new(c, (int)res));
                }
                StateHasChanged();
            }

        }

        bool stop = false;
        void ShowSolutions()
        {

            if (string.IsNullOrEmpty(nString))
                return;
            solutions.Clear();
            stop = false;
            N = BigInteger.Parse(nString);
            var steps = Classes.OrderByDescending(x => x.Class).Select(x => (long)x.Class).ToArray();
            var s = Classes.OrderByDescending(x => x.Class).Select(x => (long)x.).ToArray();

            var start = 0;
            var found = false;
            ref long first = ref s[0];
            var firstStep = steps[0];
            SolutionSteps = 0;
            int updateCounter = 0;

            while (!stop && solutions.Count < 2)
            {
                var previous = s[0];
                found = true;
                for (var j = 1; found && j < steps.Length; j++)
                {
                    ref long  = ref s[j];
                    ref long step = ref steps[j];
                    var diff = previous - ;
                    var stepCount = (long)MathLib.Ceiling((double)diff / step);
                    var bigStep = stepCount * step;
                     += bigStep;
                    while ( < previous)
                    {
                         += step;
                    }
                    found =  == previous;
                }

                if (found)
                {
                    solutions.Add(new(first, SolutionSteps));
                    if (solutions.Count < 2)
                        first += firstStep;
                }
                else
                    first += firstStep;

                SolutionSteps++;
                SolutionValue = first;
                updateCounter++;
                if (found || updateCounter >= 10000)
                {
                    updateCounter = 0;
                    //StateHasChanged();
                }
            }

            // = long.Parse(String);
            //long t;
            //var C = N;
            //for (var p = 1; p < C; p++)
            //{
            //    t = 0;
            //    for (var q = 1; q < C; q++)
            //    {
            //        t += p;
            //        if (t > C) t -= C;
            //        if (t == )
            //        {
            //            solutions.Add(new(p, q));
            //        }
            //    }
            //}
            StateHasChanged();
            //await Task.FromResult(true);
        }
    }
}
