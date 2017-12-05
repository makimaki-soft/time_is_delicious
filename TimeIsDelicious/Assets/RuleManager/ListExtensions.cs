using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleManager
{
    static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var rng = new Random();
            int n = list.Count();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var tmp = list[k];
                list[k] = list[n];
                list[n] = tmp;
            }
        }

        public static List<T> Chunks<T>(this List<T> list, int size)
        {
            List<T> ret = list.Take(size).ToList();
            list = list.Skip(size).ToList();
            return ret;
        }
    }
}
