using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
    [TestClass]
    // XXX RENAME
    public class UnitTest1
    {
        int SumSquares(IEnumerable<int> xs)
        {
            return xs.Sum(i => i * i);
        }

        class Hand
        {
            public Hand(string handSpec)
            {
                Cards = handSpec.Split(' ');
            }

            string[] Cards;
        }

        Hand Poker(IEnumerable<Hand> hands)
        {
            return hands.OrderByDescending(hand => HandRank(hand)).First();
        }

        int HandRank(Hand hand)
        {
            return 0;
        }

        [TestMethod]
        public void Poker_ReturnsTheHighestRankingHand()
        {
            var straightFlush = new Hand("6C 7C 8C 9C TC");
            var fourOfAKind = new Hand("9D 9H 9S 9C 7D");
            var fullHouse = new Hand("TD TC TH 7C 7D");
            Assert.AreSame(straightFlush, Poker(new List<Hand>() { straightFlush, fourOfAKind, fullHouse }));
            Assert.AreSame(fourOfAKind, Poker(new List<Hand>() { fourOfAKind, fullHouse }));
            Assert.AreSame(fullHouse, Poker(new List<Hand>() { fullHouse, fullHouse }));
            Assert.AreSame(fullHouse, Poker(new List<Hand>() { fullHouse }));
        }

        [TestMethod]
        public void PythonLikeMax()
        {
            Assert.AreEqual(5, new int[] { 3, 4, 5, 0 }.Max());
            Assert.AreEqual(-5, new int[] { 3, 4, -5, 0 }.OrderByDescending(i => System.Math.Abs(i)).First());
        }

        [TestMethod]
        public void MaxUsingTuples()
        {
            var a = new HandRank(8, 4);
            var b = new HandRank(8, 1);
            bool x = a > b;
        }
    }

    public class HandRank
    {
        public HandRank(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        int a;
        int b;

        public static bool operator >(HandRank a, HandRank b)
        {
            return true;
        }

        public static bool operator <(HandRank a, HandRank b)
        {
            return true;
        }
    }
}
