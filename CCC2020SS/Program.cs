using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CCC2020SS
{
    #region Helpers

    public static class Logger
    {
        public static void Log(string message)
        {
            System.Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }

    public static class StringExtension
    {
        public static int AsInt(this String str)
        {
            return Convert.ToInt32(str);
        }
        public static double AsDouble(this String str)
        {
            return double.Parse(str.Replace(".", ","));
        }
    }

    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }

    public class Helpers
    {
        public static Random rand = new Random();

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            var filenames = Enumerable.Range(1, 5).Select(p => "..\\..\\data\\level1_" + p + ".in").ToList();
            List<String> outputText = new List<String>();
            foreach (var filename in filenames)
            {
                Console.WriteLine(filename);
                string[] lines = System.IO.File.ReadAllLines(filename);
                int[] props = lines[0].Split(' ').Select(p => p.AsInt()).ToArray();
                int[] props2 = lines[1].Split(' ').Select(p => p.AsInt()).ToArray();

                var data = lines[2].Split(' ');
                for (int i = 0; i < data.Length; i += 2)
                {
                    
                }

                IList<String> s = new List<String>();
                System.IO.File.WriteAllLines(filename + ".out", s);
            }
            Console.ReadKey();
        }
    }
}
