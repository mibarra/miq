using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
    public class BowlingScorer
    {
        public const int MaxRollsPerGame = 21;

        public void Roll(int pins)
        {
            rolls[nextRoll++] = pins;
        }

        public int Score()
        {
            int score = 0;
            int rollIndex = 0;
            for (int frame = 0; frame < 10; frame++)
            {
                if (IsStrike(rollIndex))
                {
                    score += 10 + StrikeBonus(rollIndex);
                    rollIndex += 1;
                }
                else if (IsSpare(rollIndex))
                {
                    score += 10 + SpareBonus(rollIndex);
                    rollIndex += 2;
                }
                else
                {
                    score += SimpleFrameScore(rollIndex);
                    rollIndex += 2;
                }
            }

            return score;
        }

        private int SimpleFrameScore(int rollIndex)
        {
            return rolls[rollIndex] + rolls[rollIndex + 1];
        }

        private bool IsStrike(int rollIndex)
        {
            return rolls[rollIndex] == 10;
        }

        private int StrikeBonus(int rollIndex)
        {
            return rolls[rollIndex + 1] + rolls[rollIndex + 2];
        }

        private int SpareBonus(int rollIndex)
        {
            return rolls[rollIndex + 2];
        }

        private bool IsSpare(int rollIndex)
        {
            return rolls[rollIndex] + rolls[rollIndex + 1] == 10;
        }

        private int[] rolls = new int[MaxRollsPerGame];
        private int nextRoll = 0;
    }

    [TestClass]
    public class BowlingScorerKata
    {
        BowlingScorer Sut;

        [TestInitialize]
        public void Initialize()
        {
            Sut = new BowlingScorer();
        }

        [TestMethod]
        public void BowlingScorerExists()
        {
            Assert.IsTrue(Sut is BowlingScorer);
        }

        [TestMethod]
        public void TestGutterGame()
        {
            int expectedResult = 0;

            RollMany(20, 0);
            int result = Sut.Score();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestAllOnesGame()
        {
            int expectedResult = 20;

            RollMany(20, 1);
            int result = Sut.Score();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestOneSpare()
        {
            int expectedResult = 22;

            RollSpare();
            Sut.Roll(6);
            RollMany(17, 0);
            int result = Sut.Score();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestOneStrike()
        {
            int expectedResult = 14;

            RollStrike();
            Sut.Roll(1);
            Sut.Roll(1);
            int result = Sut.Score();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestPerfectGame()
        {
            int expectedResult = 300;

            RollMany(12, 10);
            int result = Sut.Score();

            Assert.AreEqual(expectedResult, result);
        }

        private void RollStrike()
        {
            Sut.Roll(10);
        }

        private void RollSpare()
        {
            Sut.Roll(5);
            Sut.Roll(5);
        }

        private void RollMany(int times, int pins)
        {
            for (int i = 0; i < times; i++)
            {
                Sut.Roll(pins);
            }
        }

    }
}
