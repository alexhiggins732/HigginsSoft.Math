using MathGmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HigginsSoft.Math.Lib.Tests
{
    internal interface IOpProvider
    {

    }
    public interface IOpProvider<T>
    {
        T Add(T a, T b);
    }

    public interface IIntOpProvider: IOpProvider<int> { }
    public interface IMpzOpProvider : IOpProvider<mpz_t> { }
    public class IntOpProvider : IIntOpProvider
    {
        public int Add(int a, int b) => a + b;

    }
    public class MPZOpProvider : IMpzOpProvider
    {
        public mpz_t Add(mpz_t a, mpz_t b)
        {
            mpz_t z = new mpz_t();
            gmp_lib.mpz_init(z);
            gmp_lib.mpz_add(z, a, b);
            return z;
        }

        public mpz_t Add(mpz_t a, int b)
        {
            mpz_t x = new mpz_t();
            mpz_t z = new mpz_t();
       
            gmp_lib.mpz_init_set_si(x, b);
            gmp_lib.mpz_init(z);
       
            gmp_lib.mpz_add(z, a, x);
            gmp_lib.mpz_clears(x, null);
            return z;
        }

        public mpz_t Add(mpz_t a, uint b)
        {
            mpz_t x = new mpz_t();
            mpz_t z = new mpz_t();

            gmp_lib.mpz_init_set_ui(x, b);
            gmp_lib.mpz_init(z);

            gmp_lib.mpz_add(z, a, x);
            gmp_lib.mpz_clears(x, null);
            return z;
        }

        public mpz_t Add(mpz_t a, double b)
        {
            mpz_t x = new mpz_t();
            mpz_t z = new mpz_t();

            gmp_lib.mpz_init_set_d(x, b);
            gmp_lib.mpz_init(z);

            gmp_lib.mpz_add(z, a, x);
            gmp_lib.mpz_clears(x, null);
            return z;
        }

        public mpz_t Add(mpz_t a, BigInteger b)
        {
            mpz_t x = new mpz_t();
            mpz_t z = new mpz_t();
            char_ptr value = new char_ptr(b.ToString("x").TrimStart('0'));
            gmp_lib.mpz_init_set_str(x, value, 16);
            gmp_lib.mpz_init(z);

            gmp_lib.mpz_add(z, a, x);

            gmp_lib.free(value);
            gmp_lib.mpz_clears(x, null);
            return z;
        }
    }
}
