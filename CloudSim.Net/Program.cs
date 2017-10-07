using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CloudSim.Sharp.ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.WriteLine("Test");
            Log.WriteLine("Another Test");

            using (var reader = new StreamReader(Log.GetOutput(), Encoding.Default, false, 4128, true))
            {
                reader.BaseStream.Position = 0;
                var str = reader.ReadToEnd();
                Console.Write(str);
                Debugger.Break();
            }

            Console.ReadLine();
        }
    }
}
