using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Miq.Game.ZombieDice
{
    public class Dice
    {
        public Dice(int numberOfBrainFaces)
        {
            NumberOfBrainFaces = numberOfBrainFaces;
        }

        public static readonly Dice Red = new Dice(1);
        public static readonly Dice Yellow = new Dice(2);
        public static readonly Dice Green = new Dice(3);
        public Color Color
        {
            get
            {
                switch (NumberOfBrainFaces)
                {
                    case 1: return Color.Red;
                    case 2: return Color.Yellow;
                    case 3: return Color.Green;
                }

                throw new NotSupportedException("Invalid number of brain faces in a dice.");
            }
        }

        public Face Roll()
        {
            int roll = Random.Next(6);
            if (roll < 2)
            {
                return Face.Footprints;
            }
            else if (roll < 2 + NumberOfBrainFaces)
            {
                return Face.Brain;
            }
            else
            {
                return Face.Shotgun;
            }
        }

        private Random Random = new Random();
        private int NumberOfBrainFaces;
    }

    public class Cup
    {
        public int DicesLeft { get { return Dices.Count; } }

        public Dice NextDice()
        {
            int n = Random.Next(Dices.Count);
            Dice nextDice = Dices[n];
            Dices.RemoveAt(n);
            return nextDice;
        }

        public Cup()
        {
            Dices = new List<Dice>(new Dice[] {
                Dice.Green, Dice.Green, Dice.Green, Dice.Green, Dice.Green, Dice.Green,
                Dice.Yellow, Dice.Yellow, Dice.Yellow, Dice.Yellow, 
                Dice.Red, Dice.Red, Dice.Red
            });
        }

        private List<Dice> Dices;
        private Random Random = new Random();
    }

    public class Turn
    {
        
    }

    public enum Face
    {
        Brain,
        Shotgun,
        Footprints
    }

    public enum Color
    {
        Red,
        Green, 
        Yellow
    }
}

namespace Miq.Tests.Nursery
{
    using Miq.Game.ZombieDice;
    using System.Collections.Generic;
    using Math = System.Math;

    [TestClass]
    public class ZombieDiceTests
    {
        [TestMethod]
        public void RollDiceReturnsAFace()
        {
            Dice Sut = Dice.Yellow;

            Face result = Sut.Roll();

            Assert.IsTrue(result == Face.Brain || result == Face.Shotgun || result == Face.Footprints);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void YellowDiceRollsEverythingWithTheSameProbability()
        {
            int nRolls = 3000000;
            Dictionary<Face, int> counts = new Dictionary<Face, int>() {
                {Face.Brain, 0},
                {Face.Footprints, 0},
                {Face.Shotgun, 0}};
            Dice Sut = Dice.Yellow;

            for (int i = 0; i < nRolls; i++)
            {
                counts[Sut.Roll()]++;
            }

            double average = counts.Values.Average();
            double diff =
                Math.Abs(counts[Face.Brain] - average) +
                Math.Abs(counts[Face.Footprints] - average) +
                Math.Abs(counts[Face.Shotgun] - average);
            Assert.IsTrue(diff / nRolls < 0.002, string.Format("Big differentce from averages: {0}", diff / nRolls));
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void GreenDiceRollsBrainsHalfTheTime()
        {
            int nRolls = 3000000;
            Dictionary<Face, int> counts = new Dictionary<Face, int>() {
                {Face.Brain, 0},
                {Face.Footprints, 0},
                {Face.Shotgun, 0}};
            Dice Sut = Dice.Green;

            for (int i = 0; i < nRolls; i++)
            {
                counts[Sut.Roll()]++;
            }

            double brainsExpected = nRolls / 2.0;
            double footprintsExpected = nRolls / 3.0;
            double shotgunsExpected = nRolls / 6.0;
            double diff =
                Math.Abs(counts[Face.Brain] - brainsExpected) +
                Math.Abs(counts[Face.Footprints] - footprintsExpected) +
                Math.Abs(counts[Face.Shotgun] - shotgunsExpected);
            Assert.IsTrue(
                diff / nRolls < 0.002,
                string.Format("Big differentce from averages: {0}", diff / nRolls));
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void RedDiceRollsShotgunHalfTheTime()
        {
            int nRolls = 3000000;
            Dictionary<Face, int> counts = new Dictionary<Face, int>() {
                {Face.Brain, 0},
                {Face.Footprints, 0},
                {Face.Shotgun, 0}};
            Dice Sut = Dice.Red;

            for (int i = 0; i < nRolls; i++)
            {
                counts[Sut.Roll()]++;
            }

            double brainsExpected = nRolls / 6.0;
            double footprintsExpected = nRolls / 3.0;
            double shotgunsExpected = nRolls / 2.0;
            double diff =
                Math.Abs(counts[Face.Brain] - brainsExpected) +
                Math.Abs(counts[Face.Footprints] - footprintsExpected) +
                Math.Abs(counts[Face.Shotgun] - shotgunsExpected);
            Assert.IsTrue(
                diff / nRolls < 0.002,
                string.Format("Big differentce from averages: {0}", diff / nRolls));
        }

        [TestMethod]
        public void DiceKnowsItsColor()
        {
            Assert.AreEqual(Color.Green, Dice.Green.Color);
            Assert.AreEqual(Color.Yellow, Dice.Yellow.Color);
            Assert.AreEqual(Color.Red, Dice.Red.Color);
        }

        [TestMethod]
        public void CupStartsWith13Dice()
        {
            Cup sut = new Cup();

            Assert.AreEqual(13, sut.DicesLeft);
        }

        [TestMethod]
        public void CupCanRemoveARandomDice()
        {
            Cup sut = new Cup();

            Dice dice = sut.NextDice();

            Assert.IsNotNull(dice);
        }

        [TestMethod]
        public void Cup_AfterRemovingADice_CountShouldDecrese()
        {
            Cup sut = new Cup();
            
            sut.NextDice();

            Assert.AreEqual(12, sut.DicesLeft);
        }

        [TestMethod]
        public void Cup_StartsWith_6Green4Yellow3Red()
        {
            int greens = 0, yellows = 0, reds = 0;
            Cup sut = new Cup();

            while (sut.DicesLeft > 0)
            {
                Dice dice = sut.NextDice();
                if (dice == Dice.Green) greens++;
                if (dice == Dice.Yellow) yellows++;
                if (dice == Dice.Red) reds++;
            }

            Assert.AreEqual(6, greens);
            Assert.AreEqual(4, yellows);
            Assert.AreEqual(3, reds);
        }

        [TestMethod]
        public void Turn_AClassForTurnExists()
        {
            Turn sut = new Turn();
            Assert.IsNotNull(sut);
        }

        // Turn
        //   shotguns dices so far
        //   brains dices so far
        //   number of brains so far
        //   dices to roll (runners from last rull + dices taken from the cup to make a total of 3)
        //   status of the turn (Lost, Won, Playing)
        //   cup to take dices from
        //   Roll
        //      take dices from cup (3 - dices to roll), roll them. We need to see the results of the rolls.
        //              if cup doesn't have enough dices, accumulate brain dices to total, and return them to the cup.
        //      accumulate the last roll to the totals. remove shotguns from dices to roll and put them in shotguns,
        //      etc...
        //      turn is lost if shotguns == 3
        //   Done
        //      (user selects done if they don't want to roll again)
        //      turn is won

        // Turn
        // select 3 random dice from the cup
        // roll them
        //   Acummulated 3 shotguns? turn lost
        //   set aside brains and shotguns
        //   choose dices from the cup to complete 3 along with the runners
        //   not enough? keep a note of brains, add them to the cup, 
        //  plaeyer can score and end turn after rolling
        // turn ends when player rolls three shotguns, brains accomulated are lost

        // goal: roll 13 brains
    }
}
