﻿/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Runtime.CompilerServices;

namespace HigginsSoft.Math.Lib
{
    public static partial class MathLib
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tanh(double d) => System.Math.Tanh(d);
    }
}