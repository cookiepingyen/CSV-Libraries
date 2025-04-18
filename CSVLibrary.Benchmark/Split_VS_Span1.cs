using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Runtime.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class Split_VS_Span1
    {
        static PropertyInfo[] props = typeof(CSVModel).GetProperties();
        static List<CSVModel> list = new List<CSVModel>();


        static int PropsCount = props.Length;

        delegate void SetterDelegate(object target, object value);

        private static readonly SetterDelegate[] _setterDelegates =
        props.Select(p => CreateSetter(p)).ToArray();

        private static SetterDelegate CreateSetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);

            var propertySetter = Expression.Call(castTarget, property.GetSetMethod(), castValue);

            var lambda = Expression.Lambda<SetterDelegate>(propertySetter, targetParam, valueParam);
            return lambda.Compile();
        }


        [Benchmark]
        public void Split()
        {

            string input = "22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60";
            string[] result = input.Split(',');

            List<CSVModel> list = new List<CSVModel>();
            CSVModel t = new CSVModel();
            PropertyInfo[] props = t.GetType().GetProperties();


            for (int i = 0; i < result.Length; i++)
            {
                props[i].SetValue(t, result[i]);
            }
            list.Add(t);
        }

        // 藍方選手
        [Benchmark]
        public void Span()
        {
            //List<Action<object, object>> temp = props.Select(x=> x.SetValue).ToList();  

            string input = "22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60";
            ReadOnlySpan<char> datas = input.AsSpan();
            CSVModel dataModel = new CSVModel();

            int start = 0;
            for (int i = 0; i < PropsCount; i++)
            {
                // 找逗號位置
                int commaIndex = datas.Slice(start).IndexOf(',');

                if (commaIndex == -1)
                {
                    // 最後一欄
                    _setterDelegates[i](dataModel, datas.Slice(start).ToString());
                    break;
                }
                else
                {
                    _setterDelegates[i](dataModel, datas.Slice(start, commaIndex).ToString());
                    start += commaIndex + 1;
                }
            }

            // 這裡就已經是完整填好的 DataModel
            list.Add(dataModel);
            list.Clear();
        }
    }
}
