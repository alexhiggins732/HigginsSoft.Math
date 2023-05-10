/*
 Copyright (c) 2023 HigginsSoft
 Written by Alexander Higgins https://github.com/alexhiggins732/ 
 
 Source code for this software can be found at https://github.com/alexhiggins732/HigginsSoft.Math
 
 This software is licensce under GNU General Public License version 3 as described in the LICENSE
 file at https://github.com/alexhiggins732/HigginsSoft.Math/LICENSE
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

*/

using HigginsSoft.Math.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib
{
    internal class Generator
    {

        private static string[] IntegerTypeNames = {
            typeof(int).Alias(),
            typeof(uint).Alias(),
            typeof(long).Alias(),
            typeof(ulong).Alias(),
            typeof(BigInteger).Alias(),
            typeof(GmpInt).Alias()
        };

        private static string[] DecimalTypeNames = {
            typeof(float).Alias(),
            typeof(double).Alias(),
            typeof(decimal).Alias(),
            typeof(GmpFloat).Alias()
        };

        private static string MathUtilClassTemplate = @"using System;



namespace HigginsSoft.Math.Lib
{
    public partial class MathUtil
    {
        [body]
    }
}
";
        public static void GenerateGcdClass()
        {
            var template = @"

        public static [type] GCD([type] a, [type] b)
        {
            if (b == 0)
            {
                return a;
            }
            return GCD(b, a % b);
        }
";
            var sb = new StringBuilder();

            IntegerTypeNames.ForEach(x => sb.Append(template.Replace("[type]", x)));
            var code = sb.ToString();
            var classDefinition = MathUtilClassTemplate.Replace("[body]", code);
            var projectDir = GetMathLibPath();
            var destPath = Path.Combine(projectDir.FullName, nameof(MathUtil), "Gcd.cs");
            var fi = new FileInfo(destPath);
            File.WriteAllText(fi.FullName, classDefinition);
        }

        static DirectoryInfo GetMathLibPath()
        {
            var dir = AppContext.BaseDirectory;
            if (dir == null)
            {
                throw new Exception("Failed to get AppContext.BaseDirectory");
            }
            var di = new DirectoryInfo(dir);
            if (di is not null)
            {
                while (di.GetDirectories(nameof(HigginsSoft.Math.Lib)).Count() == 0)
                {
                    var parent = di.Parent;
                    if (parent is not null)
                    {
                        di = parent;
                    }
                    else
                    {
                        throw new Exception($"Failed to retrieve parent directory for {di.FullName}");
                    }
                }
                return di.GetDirectories(nameof(HigginsSoft.Math.Lib)).First();
            }
            else
            {

                throw new Exception($"Failed to retrieve directory info for {dir}");
            }
        }
    }
}

