using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
            //return double.Parse(str.Replace(".", ","));
            return double.Parse(str);
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

    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

    }

    public class Airport
    {
        public String Name { get; set; }
    }

    public class LLAT
    {
        public int TimestampOffset { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double Altitude { get; set; }

        public Position xyz { get; set; }
    }

    public class Contact
    {
        public int ContactToFlightId { get; set; }
        public int TimeStamp { get; set; }
        public int Delay { get; set; }
    }

    public class Details
    {
        public int Id { get; set; }

        public Airport Start { get; set; }
        public Airport Destination { get; set; }
        public int TakeoffTimestamp { get; set; }

        public List<LLAT> LLATs { get; set; }
        public List<LLAT> LLATExact { get; set; }

        public List<Contact> Contacts { get; set; }

        public Details()
        {
            this.LLATs = new List<LLAT>();
            this.LLATExact = new List<LLAT>();
            this.Contacts = new List<Contact>();
        }
    }

    public class Flight
    {
        public int Id { get; set; }

        public int Timestamp { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double Altitude { get; set; }

        public Airport Start { get; set; }
        public Airport Destination { get; set; }

        public int Takeoff { get; set; }
    }
    
    class Program
    {
        public static double DegreeToRad(double deg)
        {
            return Math.PI * deg / 180.0;
        }

        public static double Interpolate(double x, double f0, double x0, double f1, double x1)
        {
            return f0 + (f1 - f0) / (x1 - x0) * (x - x0);
        }

        public static Position GetXYZ(LLAT llat)
        {
            var xyz = new Position()
                {
                    x = (r + llat.Altitude) * Math.Cos(DegreeToRad(llat.Lat)) * Math.Cos(DegreeToRad(llat.Long)),
                    y = (r + llat.Altitude) * Math.Cos(DegreeToRad(llat.Lat)) * Math.Sin(DegreeToRad(llat.Long)),
                    z = (r + llat.Altitude) * Math.Sin(DegreeToRad(llat.Lat))
                };
            return xyz;
        }

        public static double Distance(Position p1, Position p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2) + Math.Pow(p1.z - p2.z, 2));
        }

        public static bool CanContact(LLAT llat1, LLAT llat2, int delay)
        {
            bool can = false;
            if (llat1.TimestampOffset != llat2.TimestampOffset + delay)
            {
                return false;
            }
            double distance = Distance(llat1.xyz, llat2.xyz);
            if (distance >= minRequiredDistance && distance <= transferRange)
            {
                can = true;
            }
            return can;
        }

        static int r = 6371000;
        static int maxDelay = 3600;
        static int requiredAltitude = 6000;
        static int minRequiredDistance = 1000;
        static double transferRange = 0;

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            //var filenames = Enumerable.Range(1, 5).Select(p => "..\\..\\..\\data\\level5_" + p + ".in").ToList();
            var filenames = new List<String>() { "..\\..\\..\\data\\level5_example.in" };
            List<String> outputText = new List<String>();
            foreach (var filename in filenames)
            {
                Console.WriteLine(filename);
                string[] lines = System.IO.File.ReadAllLines(filename);
                //int[] props = lines[0].Split(',').Select(p => p.AsInt()).ToArray();

                transferRange = lines[0].AsDouble();
                int N = lines[1].AsInt();

                List<Flight> flights = new List<Flight>();

                for (int ii = 0; ii < N; ii++)
                {
                    //string[] d = lines[i+2].Split(' ');
                    flights.Add(new Flight() { //Timestamp = d[0].AsInt(),
                                               //Lat = d[1].AsDouble(), Long = d[2].AsDouble(), Altitude = d[3].AsDouble(),
                                               //Start = new Airport() { Name = d[4] },
                                               //Destination= new Airport() { Name = d[5] },
                                               ///Takeoff = d[6].AsInt()
                                               //Lat = d[0].AsDouble(), Long = d[1].AsDouble(), Altitude = d[2].AsDouble()
                                               //Id = d[0].AsInt(), Timestamp = d[1].AsInt()
                                               Id = lines[ii+2].AsInt()
                    });
                }

                //Level 5

                List<Details> flightDetails = new List<Details>();

                foreach(var f in flights)
                {
                    string dFileName = "..\\..\\..\\data\\" + f.Id + ".csv";
                    string[] dLines = System.IO.File.ReadAllLines(dFileName);
                    Details details = new Details();
                    details.Start = new Airport() { Name = dLines[0] };
                    details.Destination = new Airport() { Name = dLines[1] };
                    details.TakeoffTimestamp = dLines[2].AsInt();

                    for (int i = 0; i < dLines[3].AsInt(); i++)
                    {
                        string[] dd = dLines[i + 4].Split(',');
                        details.LLATs.Add(new LLAT() { TimestampOffset = dd[0].AsInt(), Lat = dd[1].AsDouble(), Long = dd[2].AsDouble(), Altitude = dd[3].AsDouble() });
                    }
                    details.Id = f.Id;
                    flightDetails.Add(details);
                }

                flightDetails = flightDetails.OrderBy(p => p.Id).ToList();
                foreach (var fd in flightDetails)
                {
                    bool above = false;
                    int n = 0;
                    foreach(var llat in fd.LLATs)
                    {
                        if (llat.Altitude > requiredAltitude || above && llat.Altitude <= requiredAltitude)
                        {                            
                            above = llat.Altitude > requiredAltitude;
                            var beforeLLAT = fd.LLATs[n - 1];
                            for (int t = beforeLLAT.TimestampOffset; t < llat.TimestampOffset; t++)
                            {
                                var newLLAT = new LLAT()
                                {
                                    TimestampOffset = t + fd.TakeoffTimestamp, //TODO: check this!
                                    Lat = Interpolate(t, beforeLLAT.Lat, beforeLLAT.TimestampOffset, llat.Lat, llat.TimestampOffset),
                                    Long = Interpolate(t, beforeLLAT.Long, beforeLLAT.TimestampOffset, llat.Long, llat.TimestampOffset),
                                    Altitude = Interpolate(t, beforeLLAT.Altitude, beforeLLAT.TimestampOffset, llat.Altitude, llat.TimestampOffset)
                                };
                                if (newLLAT.Altitude > requiredAltitude)
                                {
                                    newLLAT.xyz = GetXYZ(newLLAT);
                                    fd.LLATExact.Add(newLLAT);
                                }
                            }
                        }
                        n++;
                    }
                }

                flightDetails = flightDetails.Where(p => p.LLATExact.Count() > 0).ToList();

                //Parallel.ForEach(flightDetails, (f1) =>
                foreach (var f1 in flightDetails)
                {
                    foreach (var f2 in flightDetails)
                    {
                        if (f1.Id == f2.Id || f1.Destination.Name == f2.Destination.Name)
                        {
                            continue;
                        }
                        if (f1.LLATExact.Last().TimestampOffset < f2.LLATExact.First().TimestampOffset)
                        {
                            continue;
                        }
                        if (f1.LLATExact.First().TimestampOffset > f2.LLATExact.Last().TimestampOffset + maxDelay)
                        {
                            continue;
                        }

                        for (int delay = 999; delay <= 1100; delay++)
                        {
                            System.Console.WriteLine(delay);
                            foreach (var llat1 in f1.LLATExact)
                            {
                                var llat2 = f2.LLATExact.Where(p => p.TimestampOffset + delay == llat1.TimestampOffset).SingleOrDefault();
                                if (llat2 != null)
                                //foreach(var llat2 in f2.LLATExact)
                                {
                                    if (CanContact(llat1, llat2, delay))
                                    {
                                        System.Console.WriteLine("yeah!");
                                        f1.Contacts.Add(new Contact()
                                        {
                                            ContactToFlightId = f2.Id,
                                            Delay = delay,
                                            TimeStamp = llat1.TimestampOffset
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                //);

                var s = flightDetails.Where(p => p.Contacts.Count > 0).Select(p => p.Contacts.Select(m => p.Id + " " + m.ContactToFlightId + " " + m.Delay + " " + m.TimeStamp)).ToArray();
                //var s = result.Select(p => String.Format("{0:0.0000000000000} {1:0.0000000000000} {2:0.0000000000000}", p.Lat, p.Long, p.Altitude)).ToArray();
                //System.IO.File.WriteAllLines(filename + ".out", s);
                int tmp = 0;

                //Level 4
                //var result = new List<LLAT>();
                //foreach(var f in flights)
                //{
                //    string dFileName = "..\\..\\..\\data\\" + f.Id + ".csv";
                //    string[] dLines = System.IO.File.ReadAllLines(dFileName);
                //    Details details = new Details();
                //    details.Start = new Airport() { Name = dLines[0] };
                //    details.Destination = new Airport() { Name = dLines[1] };
                //    details.TakeoffTimestamp = dLines[2].AsInt();
                //
                //    for (int i = 0; i < dLines[3].AsInt(); i++)
                //    {
                //        string[] dd = dLines[i + 4].Split(',');
                //        details.LLATs.Add(new LLAT() { TimestampOffset = dd[0].AsInt(), Lat = dd[1].AsDouble(), Long = dd[2].AsDouble(), Altitude = dd[3].AsDouble() });
                //    }
                //
                //    LLAT llat = null;
                //    int offsetTime = f.Timestamp - details.TakeoffTimestamp;
                //    var exact = details.LLATs.Where(p => p.TimestampOffset == offsetTime).SingleOrDefault();
                //    if (exact != null)
                //    {
                //        llat = exact;
                //    }
                //    else
                //    {
                //        int detN = 0;
                //        foreach(var det in details.LLATs)
                //        {
                //            if (offsetTime < det.TimestampOffset)
                //            {
                //                break;
                //            }
                //            detN++;
                //        }
                //        if (detN == 0)
                //        {
                //            System.Console.WriteLine("ERROR");
                //        }
                //        var first = details.LLATs[detN - 1];
                //        var second = details.LLATs[detN];
                //        
                //        llat = new LLAT() { TimestampOffset = offsetTime,
                //                            Lat = Interpolate(offsetTime, first.Lat, first.TimestampOffset, second.Lat, second.TimestampOffset),
                //                            Long = Interpolate(offsetTime, first.Long, first.TimestampOffset, second.Long, second.TimestampOffset),
                //                            Altitude = Interpolate(offsetTime, first.Altitude, first.TimestampOffset, second.Altitude, second.TimestampOffset)
                //        };
                //    }
                //    result.Add(llat);
                //}
                //
                //var s = result.Select(p => String.Format("{0:0.0000000000000} {1:0.0000000000000} {2:0.0000000000000}", p.Lat, p.Long, p.Altitude)).ToArray();
                //System.IO.File.WriteAllLines(filename + ".out", s);

                //Level 1
                //var minTime = flights.Min(p => p.Timestamp);
                //var maxTime = flights.Max(p => p.Timestamp);
                //var minLat = flights.Min(p => p.Lat);
                //var maxLat = flights.Max(p => p.Lat);
                //var minLong = flights.Min(p => p.Long);
                //var maxLong = flights.Max(p => p.Long);
                //var maxAlt = flights.Max(p => p.Altitude);

                //IList<String> s = new List<String>() {
                //    minTime + " " + maxTime,
                //    minLat + " " + maxLat,
                //    minLong + " " + maxLong,
                //    maxAlt.ToString(),
                //    };
                //System.IO.File.WriteAllLines(filename + ".out", s);

                //Level 2
                //var ff = flights.Select(p => new { port = p.Start.Name + " " + p.Destination.Name, time = p.Takeoff }).Distinct();
                //var f = ff.GroupBy(p => p.port).OrderBy(p => p.Key).Select(p => new { sd = p.Key, count = p.Count() }).ToArray();
                //var s = f.Select(p => p.sd + " " + p.count).ToArray();
                //var j = f.Sum(p => p.count);
                //System.IO.File.WriteAllLines(filename + ".out", s);


                //Level 3
                //int r = 6371000;
                //var xyz = flights.Select(p => new Position()
                //{
                //    x = (r + p.Altitude) * Math.Cos(DegreeToRad(p.Lat)) * Math.Cos(DegreeToRad(p.Long)),
                //    y = (r + p.Altitude) * Math.Cos(DegreeToRad(p.Lat)) * Math.Sin(DegreeToRad(p.Long)),
                //    z = (r + p.Altitude) * Math.Sin(DegreeToRad(p.Lat))
                //});
                //var s = xyz.Select(p => p.x + " " + p.y + " " + p.z).ToArray();
                //System.IO.File.WriteAllLines(filename + ".out", s);
            }
            Console.ReadKey();
        }
    }
}
