/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Numerics;

namespace HigginsSoft.Math.Lib
{
    public class Fermat
    {
        public void TestFermat()
        {

        }
        public static bool FermatFactorization(int n, out int p, out int q, int maxIterations = 1000)
        {
            p = 0;
            q = 0;
            if (n < 2)
            {
                p = n;
                return false;
            }
            if ((n & 1) == 0)
            {
                // If the number is even, one factor is 2 and the other factor is n/2
                p = 2;
                q = n >> 1;
                return n == 2;
            }

            int a = (int)MathLib.Ceiling(MathLib.Sqrt(n));

            int b2 = a * a - n;
            int b = (int)MathLib.Sqrt(b2);
            bool noFactor = b * b != b2;
            for (var i = 0; noFactor && i < maxIterations; i++)
            {
                a++;
                b2 = a * a - n;
                b = (int)MathLib.Sqrt(b2);
                noFactor = b * b != b2;
            }

            if (!noFactor)
            {
                q = a + b;
                p = a - b;
                return n == q || n == q;
            }


            return false;
        }

        public static FactorizationState StartResumable(int n, int maxIterations = 1000)
        {
            var state = new FactorizationState(n: n);

            if (n < 2)
            {
                state.P = n;
                state.HasFactor = false;
                return state;
            }
            if ((n & 1) == 0)
            {
                // If the number is even, one factor is 2 and the other factor is n/2
                state.P = 2;
                state.Q = n >> 1;
                state.HasFactor = n == 2;
                return state;
            }

            state.A = (int)MathLib.Ceiling(MathLib.Sqrt(n));

            state.B2 = state.A * state.A - n;
            state.B = (int)MathLib.Sqrt(state.B2);
            bool noFactor = state.B * state.B != state.B2;

            if (!noFactor)
            {
                state.Q = state.A + state.B;
                state.P = state.A - state.B;
                state.HasFactor = n == state.Q || n == state.P;
            }
            else
            {
                Resume(state);
            }
            return state;
        }

        public static bool Resume(FactorizationState state, int maxIterations = 1000)
        {
            if (state.HasFactor) { return state.HasFactor; }


            //ref int p = ref state.P;
            //ref int q = ref state.Q;
            //ref int a = ref state.A;

            //ref int b2 = ref state.B2;
            //ref int b = ref state.B;


            int p = state.P;
            int q = state.Q;
            int a = state.A;

            int b2 = state.B2;
            int b = state.B;

            int n = state.N;
            bool noFactor = true;
            int i;
            for (i = 0; noFactor && i < maxIterations; i++)
            {
                a++;
                b2 = a * a - n;
                b = (int)MathLib.Sqrt(b2);
                noFactor = b * b != b2;
            }
            state.Iterations += i;
            if (!noFactor)
            {
                q = a + b;
                p = a - b;
                state.HasFactor = n == p || n == q;
            }

            state.P = p;
            state.Q = q;
            state.A = a;

            state.B2 = b2;
            state.B = b;


            return state.HasFactor;
        }

        public class FactorizationState
        {
            public int N;
            public int Sqrt;
            public int A;
            public int B2;
            public int B;
            public int P;
            public int Q;
            public long Iterations;
            public bool HasFactor;

            public FactorizationState(int n)
            {
                N = n;

            }
        }
    }
}