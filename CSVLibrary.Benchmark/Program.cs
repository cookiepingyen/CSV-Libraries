using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var summary = BenchmarkRunner.Run<Count_vs_Count__>();
            //var summary = BenchmarkRunner.Run<StringWriter_VS_CharWriter>();

            //Write_VS_OptimazeWrite write_VS_OptimazeWrite = new Write_VS_OptimazeWrite();
            //write_VS_OptimazeWrite.OptimazeWrite();
            //chuck

            //string input = "22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60";

            //char[] chars = input.ToCharArray();

            //StreamWriter writer = new StreamWriter("data.csv");
            //writer.WriteLine(chars);
            //writer.Flush();
            //writer.Close();

            //StringBuilder stringBuilder = new StringBuilder("22,Layney,Vasyutkin,lvasyutkinl@cocolog-nifty.com,Female,173.255.164.60", 90);
            //char[] chars = new char[90];
            //stringBuilder.CopyTo(0, chars, 0, stringBuilder.Length - 1);

            Console.ReadKey();
        }
    }
}
