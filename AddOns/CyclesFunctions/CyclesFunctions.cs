using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Modulartistic.Core;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml.Serialization;
using Melanchall.DryWetMidi.Interaction;

namespace CyclesFunctions
{
    public static class CyclesFunctions
    {
        #region Memory fields
        private static ConcurrentDictionary<string, List<List<Point>>> CycleDicts = new ConcurrentDictionary<string, List<List<Point>>>();
        private static ConcurrentDictionary<string, bool> CycleDictsInitFinished = new ConcurrentDictionary<string, bool>();

        private static ConcurrentDictionary<string, ConcurrentDictionary<Point, double>> ValueDicts = new ConcurrentDictionary<string, ConcurrentDictionary<Point, double>>();
        private static ConcurrentDictionary<string, bool> ValueDictsInitFinished = new ConcurrentDictionary<string, bool>();
        #endregion

        #region Functions for usage in Function parser
        public static double FibonacciCycles(double x, double y, double mod_num)
        {
            
            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int imod_num = (int)mod_num;
            
            Func<Point, int, Point> func = (p, n) => new Point(p.y, (int)Helper.mod(p.x+p.y, imod_num));

            double n = AssignValueSimple(ix, iy, imod_num, func, $"fibonacci{mod_num}");
            return n;
        }

        public static double FibonacciCycles2(double x, double y, double mod_num)
        {

            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int imod_num = (int)mod_num;

            Func<Point, int, Point> func = (p, n) => new Point(p.y, (int)Helper.mod(p.x + p.y, imod_num));

            double n = AssignValueComplex(ix, iy, imod_num, func, $"fibonacci{mod_num}");
            return n;
        }

        public static double LinearCycles(double x, double y, double factor, double offset, double mod_num)
        {

            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int imod_num = (int)mod_num;
            int ifactor = (int)factor;
            int ioffset = (int)offset;

            Func<Point, int, Point> func = (p, n) => new Point(p.y, (int)Helper.mod(ifactor*p.x + ioffset, imod_num));

            double n = AssignValueSimple(ix, iy, imod_num, func, $"fibonacci{mod_num}");
            return n;
        }

        public static double LinearCycles2(double x, double y, double factor, double offset, double mod_num)
        {

            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int imod_num = (int)mod_num;
            int ifactor = (int)factor;
            int ioffset = (int)offset;

            Func<Point, int, Point> func = (p, n) => new Point(p.y, (int)Helper.mod(ifactor * p.x + ioffset, imod_num));

            double n = AssignValueComplex(ix, iy, imod_num, func, $"fibonacci{mod_num}");
            return n;
        }
        #endregion

        #region private helper functions
        private static void AddCycles(string key, int mod_num, Func<Point, int, Point> func)
        {
            if (CycleDictsInitFinished.ContainsKey(key) || !CycleDictsInitFinished.TryAdd(key, false)) { return; }
            
            Console.WriteLine("1");
            List<List<Point>> result = new List<List<Point>>();
            for (int x = 0; x < mod_num; x++)
            {
                for (int y = 0; y < mod_num; y++)
                {
                    Point p = new Point(x, y);
                    if (result.Any(cycle => cycle.Contains(p))) { continue; }
                    
                    if (p == func(p, mod_num))
                    {
                        result.Add(new List<Point>() { p });
                        continue;
                    }

                    List<Point> cycle = new List<Point>();
                    Point current = p;
                    while (true) 
                    {
                        if (cycle.Contains(current))
                        {
                            cycle.Add(current);
                            break;
                        }

                        cycle.Add(current);
                        if (result.Any(c => c.Contains(current))) { break; }

                        current = func(current, mod_num);
                    }
                    result.Add(cycle);
                }
            }

            Console.WriteLine("2");
            CycleDicts.TryAdd(key, result);
            CycleDictsInitFinished.TryAdd(key, true);
            CycleDictsInitFinished[key] = true;
            Console.WriteLine("3");
        }

        private static double AssignValueSimple(int x, int y, int mod_num, Func<Point, int, Point> func, string key)
        {
            AddCycles(key, mod_num, func);
            while (!CycleDictsInitFinished[key]) { Thread.Sleep(1000); }
            
            int idx = 0;
            foreach (List<Point> cycle in CycleDicts[key])
            {
                if (cycle.Contains(new Point((int)Helper.mod(x, mod_num), (int)Helper.mod(y, mod_num)))) { break; }

                idx++;
            }

            return (double)idx / CycleDicts[key].Count;
        }

        private static void FillValueDict(string key)
        {
            if (ValueDictsInitFinished.ContainsKey(key) || !ValueDictsInitFinished.TryAdd(key, false)) { return; }

            List<List<Point>> cycles = CycleDicts[key];
            ConcurrentDictionary<Point, double> valueDict = new ConcurrentDictionary<Point, double>();

            foreach (List<Point> cycle in cycles)
            {
                if (IsCycleReal(cycle))
                {
                    int count = cycle.Count;
                    for (int i = 0; i < count; i++) 
                    {
                        valueDict.TryAdd(cycle[i], (double)i/count);
                    }
                }
                else
                {
                    int count = cycle.Count;
                    double endvalue = valueDict[cycle[^1]];
                    double startvalue = 0; // maybe change startvalue later
                    for (int i = 0; i < count-1; i++)
                    {
                        valueDict.TryAdd(cycle[i], startvalue - (startvalue - endvalue) * ((double)i / count));
                    }
                }
            }

            ValueDicts.TryAdd(key, valueDict);
            ValueDictsInitFinished.TryAdd(key, true);
            ValueDictsInitFinished[key] = true;
        }

        private static bool IsCycleReal(List<Point> cycle)
        {
            Point final = cycle[^1];
            for (int i = cycle.Count - 1; i >= 0; i--)
            {
                if (cycle[i] == final) { return true; }
            }
            return false;

            return (cycle.Count(p => p == final) == 2); // gotta test if this is faster, ifso make it inline, maybe not even a function
        }

        private static double AssignValueComplex(int x, int y, int mod_num, Func<Point, int, Point> func, string key)
        {
            AddCycles(key, mod_num, func);
            while (!CycleDictsInitFinished[key]) { Thread.Sleep(1000); }

            FillValueDict(key);
            while (!ValueDictsInitFinished[key]) { Thread.Sleep(1000); }

            ConcurrentDictionary<Point, double> valueDict = ValueDicts[key];
            return valueDict[new Point((int)Helper.mod(x, mod_num), (int)Helper.mod(y, mod_num))];
        }
        #endregion


        private struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static bool operator ==(Point p1, Point p2)
            {
                return p1.x == p2.x && p1.y == p2.y;
            }

            public static bool operator !=(Point p1, Point p2)
            {
                return !(p1 == p2);
            }
        }
    }
}
