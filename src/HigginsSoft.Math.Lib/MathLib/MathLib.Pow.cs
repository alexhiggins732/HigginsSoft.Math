using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Numerics;
using MathGmp.Native;

namespace HigginsSoft.Math.Lib
{

	public static partial class MathLib
	{
		public static GmpInt Pow(GmpInt value, int exponent)
			=> GmpInt.Power(value, exponent);

		public static mpz_t Pow(mpz_t value, int exponent)
			=> GmpInt.Power(value, exponent);

		public static double Pow(double value, int exponent)
			=> System.Math.Pow(value, exponent);

    }
}
