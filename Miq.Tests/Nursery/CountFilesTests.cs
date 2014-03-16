using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class CountFilesTests
    {
        [TestMethod]
        [Ignore]
        public void TestMethod1()
        {
            var counts = new Dictionary<string, int>();
            var dir = @"C:\Users\Miguel\Documents\Books\Unsorted";
            var x = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories);
            foreach (var item in x)
            {
                var ext = Path.GetExtension(item).ToLower();
                if (!counts.ContainsKey(ext))
                {
                    counts.Add(ext, 0);
                }
                counts[ext]++;
                if (counts[ext] % 100 == 0)
                {
                    Debug.WriteLine(ext + " " + counts[ext]);
                }
            }

            Debug.WriteLine("===");
            foreach (var item in counts.OrderByDescending(k => k.Value))
            {
                Debug.WriteLine(item.Key + "\t" + item.Value);
            }
        }

        private static void DoDirectory(Dictionary<string, int> counts, string dir, int limit)
        {
            var types = CountDirectory(dir);
            AccumulateDirectory(counts, types);
            var subdirs = Directory.GetDirectories(dir);
            if (!subdirs.Any())
                Debug.WriteLine(dir);
            foreach (var subdir in subdirs)
            {
                DoDirectory(counts, subdir, limit + 1);
            }
        }

        private static void AccumulateDirectory(Dictionary<string, int> counts, IEnumerable<KeyValuePair<string, int>> types)
        {
            foreach (var item in types)
            {
                if (!counts.ContainsKey(item.Key))
                {
                    counts.Add(item.Key, 0);
                }
                counts[item.Key] += item.Value;
            }
        }

        private static IEnumerable<KeyValuePair<string, int>> CountDirectory(string dir)
        {
            var files = Directory.GetFiles(dir);
            var types = files.Select(f => Path.GetExtension(f).ToLower())
                           .GroupBy(m => m)
                           .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()));
            return types;
        }
    }
}
