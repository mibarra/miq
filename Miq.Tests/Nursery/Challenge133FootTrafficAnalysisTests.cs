//using System;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;

//namespace Miq.Tests.Nursery
//{
//    [TestClass]
//    public class Challenge133FootTrafficAnalysisTests
//    {
//        [TestMethod]
//        public void FirstExample()
//        {
//            var events = new List<FootTrafficEvent> {
//                new FootTrafficEvent("0 0 I 540"),
//                new FootTrafficEvent("1 0 I 540"),
//                new FootTrafficEvent("0 0 O 56"),
//                new FootTrafficEvent("1 0 O 560"),
//            };
//            var expected = new List<FootTrafficAnalysis>
//            {
//                new FootTrafficAnalysis {Room=0, TotalVisitsTime=40, TotalVisits=2}
//            };

//            var actual = events.Analyze();

//            CollectionAssert.AreEquivalent(expected, actual.ToList());
//            Assert.AreEqual(20, actual.First().AverageVisitTime);
//        }

//        [TestMethod]
//        public void SecondExample()
//        {
//            var events = new List<FootTrafficEvent> {
//                //new FootTrafficEvent { Visitor = 0, Room=11, Direction=TrafficDirection.In, Time=347},
//                //new FootTrafficEvent { Visitor = 1, Room=13, Direction=TrafficDirection.In, Time=307},
//                //new FootTrafficEvent { Visitor = 2, Room=15, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 3, Room=6, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 4, Room=9, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 5, Room=2, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 6, Room=2, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 7, Room=11, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 8, Room=1, Direction=TrafficDirection.In, Time=334},
//                //new FootTrafficEvent { Visitor = 0, Room=11, Direction=TrafficDirection.Out, Time=376},
//                //new FootTrafficEvent { Visitor = 1, Room=13, Direction=TrafficDirection.Out, Time=321},
//                //new FootTrafficEvent { Visitor = 2, Room=15, Direction=TrafficDirection.Out, Time=389},
//                //new FootTrafficEvent { Visitor = 3, Room=6, Direction=TrafficDirection.Out, Time=412},
//                //new FootTrafficEvent { Visitor = 4, Room=9, Direction=TrafficDirection.Out, Time=418},
//                //new FootTrafficEvent { Visitor = 5, Room=2, Direction=TrafficDirection.Out, Time=414},
//                //new FootTrafficEvent { Visitor = 6, Room=2, Direction=TrafficDirection.Out, Time=349},
//                //new FootTrafficEvent { Visitor = 7, Room=11, Direction=TrafficDirection.Out, Time=418},
//                //new FootTrafficEvent { Visitor = 8, Room=1, Direction=TrafficDirection.Out, Time=418},
//                //new FootTrafficEvent { Visitor = 0, Room=12, Direction=TrafficDirection.In, Time=437},
//                //new FootTrafficEvent { Visitor = 1, Room=28, Direction=TrafficDirection.In, Time=343},
//                //new FootTrafficEvent { Visitor = 2, Room=32, Direction=TrafficDirection.In, Time=408},
//                //new FootTrafficEvent { Visitor = 3, Room=15, Direction=TrafficDirection.In, Time=458},
//                //new FootTrafficEvent { Visitor = 4, Room=18, Direction=TrafficDirection.In, Time=424},
//                //new FootTrafficEvent { Visitor = 5, Room=26, Direction=TrafficDirection.In, Time=442},
//                //new FootTrafficEvent { Visitor = 6, Room=7,  Direction=TrafficDirection.In, Time=435},
//                //new FootTrafficEvent { Visitor = 7, Room=19, Direction=TrafficDirection.In, Time=456},
//                //new FootTrafficEvent { Visitor = 8, Room=19, Direction=TrafficDirection.In, Time=450},
//                //new FootTrafficEvent { Visitor = 0, Room=12, Direction=TrafficDirection.Out, Time=455},
//                //new FootTrafficEvent { Visitor = 1, Room=28, Direction=TrafficDirection.Out, Time=374},
//                //new FootTrafficEvent { Visitor = 2, Room=32, Direction=TrafficDirection.Out, Time=495},
//                //new FootTrafficEvent { Visitor = 3, Room=15, Direction=TrafficDirection.Out, Time=462},
//                //new FootTrafficEvent { Visitor = 4, Room=18, Direction=TrafficDirection.Out, Time=500},
//                //new FootTrafficEvent { Visitor = 5, Room=26, Direction=TrafficDirection.Out, Time=479},
//                //new FootTrafficEvent { Visitor = 6, Room=7, Direction=TrafficDirection.Out, Time=493},
//                //new FootTrafficEvent { Visitor = 7, Room=19, Direction=TrafficDirection.Out, Time=471},
//                //new FootTrafficEvent { Visitor = 8, Room=19, Direction=TrafficDirection.Out, Time=458},
//            };
//            var expected = new List<FootTrafficAnalysis>
//            {
//                new FootTrafficAnalysis {Room=1, TotalVisitsTime=84, TotalVisits=1},
//                new FootTrafficAnalysis {Room=2, TotalVisitsTime=95, TotalVisits=2},
//                new FootTrafficAnalysis {Room=6, TotalVisitsTime=78, TotalVisits=1},
//                new FootTrafficAnalysis {Room=7, TotalVisitsTime=58, TotalVisits=1},
//                new FootTrafficAnalysis {Room=9, TotalVisitsTime=84, TotalVisits=1},
//                new FootTrafficAnalysis {Room=11, TotalVisitsTime=113, TotalVisits=2},
//                new FootTrafficAnalysis {Room=12, TotalVisitsTime=18, TotalVisits=1},
//                new FootTrafficAnalysis {Room=13, TotalVisitsTime=14, TotalVisits=1},
//                new FootTrafficAnalysis {Room=15, TotalVisitsTime=59, TotalVisits=2},
//                new FootTrafficAnalysis {Room=18, TotalVisitsTime=76, TotalVisits=1},
//                new FootTrafficAnalysis {Room=19, TotalVisitsTime=23, TotalVisits=2},
//                new FootTrafficAnalysis {Room=26, TotalVisitsTime=37, TotalVisits=1},
//                new FootTrafficAnalysis {Room=28, TotalVisitsTime=31, TotalVisits=1},
//                new FootTrafficAnalysis {Room=32, TotalVisitsTime=87, TotalVisits=1},
//            };

//            var actual = events.Analyze();
//            var result = String.Join("\n", actual.OrderBy(e => e.Room).Select(e => e.ToString()));

//            CollectionAssert.AreEquivalent(expected, actual.ToList());
//            Assert.AreEqual(84, actual.First(r => r.Room == 1).AverageVisitTime);
//            Assert.AreEqual(48, actual.First(r => r.Room == 2).AverageVisitTime);
//            Assert.AreEqual(78, actual.First(r => r.Room == 6).AverageVisitTime);
//            Assert.AreEqual(58, actual.First(r => r.Room == 7).AverageVisitTime);
//            Assert.AreEqual(84, actual.First(r => r.Room == 9).AverageVisitTime);
//            Assert.AreEqual(57, actual.First(r => r.Room == 11).AverageVisitTime);
//            Assert.AreEqual(18, actual.First(r => r.Room == 12).AverageVisitTime);
//            Assert.AreEqual(14, actual.First(r => r.Room == 13).AverageVisitTime);
//            Assert.AreEqual(30, actual.First(r => r.Room == 15).AverageVisitTime);
//            Assert.AreEqual(76, actual.First(r => r.Room == 18).AverageVisitTime);
//            Assert.AreEqual(12, actual.First(r => r.Room == 19).AverageVisitTime);
//            Assert.AreEqual(37, actual.First(r => r.Room == 26).AverageVisitTime);
//            Assert.AreEqual(31, actual.First(r => r.Room == 28).AverageVisitTime);
//            Assert.AreEqual(87, actual.First(r => r.Room == 32).AverageVisitTime);
//        }
//    }
//}
