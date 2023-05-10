using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HigginsSoft.Math.Lib
{
    public class ConvertFactory
    {
        static Dictionary<Type, IConverter> Converters = new();
        static Dictionary<Type, IConverter> RegisterConverters = new();
        public static IConverter Convert(INumeric value)
        {
            if (!Converters.ContainsKey(value.Type))
            {
                var t = typeof(IConverter<>);
                Type[] typeArgs = { value.Type };
                Type converterType = t.MakeGenericType(typeArgs);
                Converters.Add(value.Type, RegisterConverters[converterType]);

            }
            return (IConverter)Converters[value.Type];
        }

        static ConvertFactory()
        {
            Converters.Add(typeof(int), new IntConverter());
        }
    }
    public interface IConverter<T>
    {

    }
    public interface IConverter
    {
        public int ToInt();
    }
    public interface IIntConverter:IConverter { }

    public class IntConverter: IIntConverter
    {

        public int ToInt()
        {
            return 0;
        }
    }
}
