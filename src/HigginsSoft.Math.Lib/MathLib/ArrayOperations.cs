/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Some routines inspired by the Stanford Bit Twiddling Hacks by Sean Eron Anderson:
// http://graphics.stanford.edu/~seander/bithacks.html

#define TARGET_64BIT


using System.Runtime.InteropServices;

namespace HigginsSoft.Math.Lib
{
    public partial class MathLib
    {
        /// <summary>
        /// Aligns the reference array on a 32 bit boundary in memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Align<T>(ref T[] array)
            => ArrayOperations.AlignArray(ref array);


        /// <summary>
        /// Aligns the reference array on a 32 bit boundary in memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static GCHandle AlignPinnedArray<T>(ref T[] array)
            => ArrayOperations.AlignPinnedArray(ref array);

        public static unsafe int GetAlignment<T>(T* ptr)
        {
            return (int)(uint)ptr & 31;
        }
        public static unsafe int GetAlignment<T>(ref T[] array)
        {
            fixed (T* ptr = array)
            {
                return (int)(uint)ptr & 31;
            }
        }
        public class ArrayOperations
        {
            public static unsafe void AlignArray<T>(ref T[] array)
            {
                for (; ; )
                {
                    array = array.ToArray();

                    fixed (T* ptr = array)
                    {
                        //if ((uint)ptr % 32 == 0)
                        if (((uint)ptr & 31) == 0u)
                        {
                            break;
                        }
                    }
                }
            }

            public static unsafe GCHandle AlignPinnedArray<T>(ref T[] array)
            {
                for (; ; )
                {
                    array = array.ToArray();

                    fixed (T* ptr = array)
                    {
                        //if ((uint)ptr % 32 == 0)
                        if (((uint)ptr & 31) == 0u)
                        {
                            break;
                        }
                    }
                }
                return GCHandle.Alloc(array, GCHandleType.Pinned);
            }
        }
    }
}