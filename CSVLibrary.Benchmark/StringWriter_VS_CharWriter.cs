using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class StringWriter_VS_CharWriter
    {
        char[] chars = "22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60".ToCharArray();

        [Benchmark]
        public void Write()
        {
            using (StreamWriter writer = new StreamWriter("data1.csv"))
            {
                for (int i = 0; i < 1000000; i++)
                {
                    writer.WriteLine("22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60");
                    writer.Flush();
                }
            }

        }

        // 藍方選手
        [Benchmark]
        public void OptimazeWrite()
        {
            using (StreamWriter writer = new StreamWriter("data2.csv"))
            {
                for (int i = 0; i < 1000000; i++)
                {
                    writer.WriteLine(chars);
                }
                writer.Flush();
            }
        }


    }
}