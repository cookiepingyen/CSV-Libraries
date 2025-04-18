using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Runtime.DacInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class Write_VS_OptimazeWrite
    {
        static StringBuilder stringBuilder = new StringBuilder();
        static PropertyInfo[] props = typeof(CSVModel).GetProperties();

        static int PropsCount = props.Length;
        delegate object GetterDelegate(object target);
        private static readonly GetterDelegate[] _getterDelegates =
        props.Select(p => CreateGetter(p)).ToArray();
        private static GetterDelegate CreateGetter(PropertyInfo property)
        {

            //var targetParam = Expression.Parameter(typeof(object), "target");
            //var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            //var propertyGetter = Expression.Property(castTarget, property);
            //var castResult = Expression.Convert(propertyGetter, typeof(object));

            //var lambda = Expression.Lambda<GetterDelegate>(castResult, targetParam);
            //return lambda.Compile();

            var targetType = typeof(object);
            var targetParam = Expression.Parameter(targetType, "target");
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var propertyGetter = Expression.Call(castTarget, property.GetGetMethod());
            var lambda = Expression.Lambda<GetterDelegate>(propertyGetter, targetParam);
            return lambda.Compile();
        }
        [Benchmark]
        public void Write()
        {
            CSVModel model = new CSVModel("1", "Ping", "Wu", "example@gamil.com", "male", "11.125.13");

            PropertyInfo[] props = typeof(CSVModel).GetProperties();
            string dataStr = "";

            for (int i = 0; i < props.Length; i++)
            {
                dataStr += props[i].GetValue(model).ToString() + ", ";
            }
            dataStr = dataStr.TrimEnd(',');

        }

        // 藍方選手
        [Benchmark]
        public void OptimazeWrite()
        {
            CSVModel model = new CSVModel("1", "Ping", "Wu", "example@gamil.com", "male", "11.125.13");

            for (int i = 0; i < props.Length; i++)
            {
                stringBuilder.Append(_getterDelegates[i](model));
                stringBuilder.Append(',');
            }

            List<char> charList = new List<char>();
            for (int i = 0; i < stringBuilder.Length; i++)
            {
                charList.Add(stringBuilder[i]);
            }

            string dataStr = new string(charList.ToArray());


            string dataStr2 = stringBuilder.ToString(0, stringBuilder.Length - 1);
            stringBuilder.Clear();

        }

    }
}
