
using Microsoft.AspNetCore.Components;
using System.Numerics;
using static HigginsSoft.Math.Lib.MathLib;
using static HigginsSoft.Math.Lib.s;

namespace HigginsSoft.Math.UI.Pages
{
    public partial class ModTables
    {
        private string nString = "10";
        private string String = "0";
        private long n;
        private long ;
        public long N { get => n; set => UpdateTable(value); }
        public long  { get => ; set => Update(value); }
        public Table Table { get; private set; } = null!;

        public string NString
        {
            get => nString;
            set
            {
                if (long.TryParse(value, out long n))
                {
                    nString = value;
                    //N = n;

                }
            }

        }

        public string String
        {
            get => String;
            set
            {
                if (long.TryParse(value, out long ))
                {
                    String = value;
                    // = ;

                }
            }

        }

        private void UpdateTable(long value)
        {
            n = value;
        
        }
        private void Update(long value)
        {
             = value;

        }

        List<(int p, int q)> solutions = new();
        public void AddSolution(int p, int q)
        {
            //solutions.Add(new(p, q));

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

       void ShowSolutions()
        {
            solutions.Clear();
            N = long.Parse(NString);
             = long.Parse(String);
            long t;
            var C = N;
            for (var p = 1; p < C; p++)
            {
                t = 0;
                for (var q = 1; q < C; q++)
                {
                    t += p;
                    if (t > C) t -= C;
                    if (t == )
                    {
                        solutions.Add(new(p, q));
                    }
                }
            }
            StateHasChanged();
            //await Task.FromResult(true);
        }
    }
}
