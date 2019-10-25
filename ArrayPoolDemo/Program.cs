using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArrayPoolDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using var file = new StreamReader(@"D:\Projects\Data\Firstnames.txt");

            var m1 = 0;

            var t1 = new Stopwatch();
            t1.Start();

            await foreach (var line in ReadLines(file))
            {
                char[] data = new char[line.Length];
                Parse(line, data);

                m1++;
            }
            t1.Stop();

            Console.WriteLine("M1 : " + t1.ElapsedMilliseconds + " counter:" + m1);

            using var file2 = new StreamReader(@"D:\Projects\Data\Firstnames.txt");
            var t2 = new Stopwatch();
            var m2 = 0;
            t2.Start();

            var pool = ArrayPool<char>.Shared;

            await foreach (var line in ReadLines(file2))
            {
                var dataArray = pool.Rent(line.Length);
                Parse(line, dataArray);
                pool.Return(dataArray);

                m2++;
            }

            t2.Stop();

            Console.WriteLine("M2 : " + t2.ElapsedMilliseconds + " counter:" + m2);

        }

        private static async IAsyncEnumerable<string> ReadLines(StreamReader reader)
        {
            //using StreamReader reader = new StreamReader(file);

            while (reader.Peek() >= 0)
            {
                yield return await reader.ReadLineAsync();
            }

        }
        private static void Parse(string lineData, char[] chars)
        {
            Array.Copy(lineData.ToCharArray(), chars, lineData.Length);
        }
        private static int CountNumberOfRepeatedCharacters(char[] chars)
        {
            return chars.GroupBy(c => c)
                 .Where(c => c.Count() > 1)
                 .Sum(c => c.Count());

        }
    }
}
