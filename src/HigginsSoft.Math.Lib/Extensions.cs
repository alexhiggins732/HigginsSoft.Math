/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using System.Collections;

namespace HigginsSoft.Math
{
    public static class Extensions
    {

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (var t in array)
            {
                action(t);
            }
        }

        public static byte[] HexToByteArray(this string hex)
        {
            if (hex.Length % 2 == 1)
                hex = "0" + hex;

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static ulong[] HexToUlongArray(this string hex)
        {
            var rem = 16 - (hex.Length & 15);
            hex = hex.PadLeft(hex.Length + rem, '0');

            var words = Enumerable.Range(0, hex.Length >> 4)
                .Select(i => new string(hex.Skip(i << 4).Take(16).ToArray()))
                .ToArray();
            var result = words
                .Select(i => Convert.ToUInt64(i, 16))
                .ToArray();
            return result;
            //var bytes = hex.HexToByteArray();
            //var result = Enumerable.Range(0, bytes.Length / sizeof(ulong))
            //    .Select(i=> bytes.Skip(i << 3).Take(sizeof(ulong)).ToArray())
            //    .Select(i => BitConverter.ToUInt64(i.Reverse().ToArray(), 0))
            //    .ToArray();
            //return result;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }

    public static class TypeExtensions
    {
        public static string Alias(this Type type)
        {
            return TypeAliases.ContainsKey(type) ?
                TypeAliases[type] : type.Name;
        }




        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
    {
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(object), "object" },
        { typeof(bool), "bool" },
        { typeof(char), "char" },
        { typeof(string), "string" },
        { typeof(void), "void" },
        { typeof(byte?), "byte?" },
        { typeof(sbyte?), "sbyte?" },
        { typeof(short?), "short?" },
        { typeof(ushort?), "ushort?" },
        { typeof(int?), "int?" },
        { typeof(uint?), "uint?" },
        { typeof(long?), "long?" },
        { typeof(ulong?), "ulong?" },
        { typeof(float?), "float?" },
        { typeof(double?), "double?" },
        { typeof(decimal?), "decimal?" },
        { typeof(bool?), "bool?" },
        { typeof(char?), "char?" }
    };
    }
}
