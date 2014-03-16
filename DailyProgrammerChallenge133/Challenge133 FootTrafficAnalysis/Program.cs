using System;
using System.Collections.Generic;
using System.Linq;

namespace Miq.DailyProgrammerChallenges
{
    static class Challenge133FootTrafficAnalysis
    {
        static void Main()
        {
            Console.WriteLine(
                String.Join("\n",
                    Input().Analyze()
                          .OrderBy(e => e.Room)
                          .Select(e => e.ToString())));
            Console.ReadLine();
        }

        static IEnumerable<FootTrafficEvent> Input()
        {
            int nrLines = Int32.Parse(Console.ReadLine());
            while (nrLines > 0)
            {
                yield return new FootTrafficEvent(Console.ReadLine());
                nrLines--;
            }
        }

        static IEnumerable<FootTrafficAnalysis> Analyze(this IEnumerable<FootTrafficEvent> events)
        {
            return events.GroupBy(e => e.Room)
                        .Select(group => new FootTrafficAnalysis(group.Key, group.Count() / 2, group.Sum(e => e.SignedTime)));
        }
    }

    class FootTrafficEvent
    {
        public FootTrafficEvent(string line)
        {
            var field = line.Split();
            Visitor = Int32.Parse(field[0]);
            Room = Int32.Parse(field[1]);
            Direction = field[2];
            Time = Int32.Parse(field[3]);
        }

        public int Room { get; set; }
        public int Visitor { get; set; }
        public string Direction { get; set; }
        public int Time { get; set; }
        public int SignedTime { get { return Direction == "I" ? -Time : Time; } }
    }

    class FootTrafficAnalysis
    {
        public FootTrafficAnalysis(int room, int totalVisits, int totalVisitsTime)
        {
            Room = room;
            TotalVisits = totalVisits;
            TotalVisitsTime = totalVisitsTime;
        }

        public int Room { get; set; }
        public int TotalVisits { get; set; }
        public int TotalVisitsTime { get; set; }
        public int AverageVisitTime { get { return TotalVisitsTime / TotalVisits; } }

        public override string ToString()
        {
            return String.Format("Room {0}, {1} minute average visit, {2} visitor total", Room, AverageVisitTime, TotalVisits);
        }
    }
}