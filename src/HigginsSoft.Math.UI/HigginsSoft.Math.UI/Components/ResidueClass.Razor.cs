
using Microsoft.AspNetCore.Components;
using System.Numerics;
using static HigginsSoft.Math.Lib.MathLib;
using static HigginsSoft.Math.Lib.s;

namespace HigginsSoft.Math.UI.Components
{
    public partial class Class
    {

        [Parameter]
        public Models.Class Value { get; set; } = null!;

        [Parameter]
        public bool ShowModTable { get; set; } = false;


        [Parameter]
        public Pages.NTables Parent { get; set; } = null!;

        public List<(int p, int q)> solutions = new();
        public void AddSolution(int p, int q)
        {
            //solutions.Add(new(p, q));

        }

        protected override async Task OnInitializedAsync()
        {
            ShowSolutions();
            Parent.Components.Add(this);
            await base.OnInitializedAsync();
        }

        public void ShowSolutions()
        {
            solutions.Clear();
            
            var N = Value.Class;
            if (N == 7)
            {
                string bp = "";
            }
            var  = Value.;
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

        public void ShowModTableChanged(bool toggled)
        {
            // Because variable is not two-way bound, we need to update it ourself
            ShowModTable = toggled;

        }

        [Parameter]
        public EventCallback OnRemoveClick { get; set; }



        private async Task HandleClick()
        {
            // Invoke the event callback to notify the parent component
            await OnRemoveClick.InvokeAsync();
        }


        [Parameter]
        public EventCallback OnFilterClick { get; set; }



        private async Task HandleFilterClick()
        {
            // Invoke the event callback to notify the parent component
            await OnFilterClick.InvokeAsync();
        }
    }
}
