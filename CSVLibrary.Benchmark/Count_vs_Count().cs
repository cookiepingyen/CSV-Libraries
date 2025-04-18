using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class Count_vs_Count__
    {
        static StringBuilder stringBuilder = new StringBuilder("22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60", 90);
        char[] chars = new char[90];
        [Benchmark]
        public void Count()
        {
            List<CSVModel> list = new List<CSVModel>();
            int count = list.Count;
        }

        // 藍方選手
        [Benchmark]
        public void Count_LINQ()
        {
            List<CSVModel> list = new List<CSVModel>();
            int count = list.Count();
        }

        // 藍方選手
        [Benchmark]
        public void StringBuilderToArray()
        {
            stringBuilder.CopyTo(0, chars, 0, stringBuilder.Length - 1);
        }

    }
}
