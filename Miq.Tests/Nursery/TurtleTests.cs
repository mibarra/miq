using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Miq.Tests.Nursery
{
    class Turtle
    {
        public Turtle(IPoint origin, double stepSize, double angleIncrement)
        {
            StepSize = stepSize;
            AngleIncrement = angleIncrement;
            Position = origin;
            Heading = 0.0;
        }

        public IPoint Position { get; set; }
        public double Heading { get; set; }
        public double StepSize { get; set; }
        public double AngleIncrement { get; set; }

        public void TurnLeft()
        {
            Heading += AngleIncrement;
        }

        public void TurnRight()
        {
            Heading -= AngleIncrement;
        }

        public Line DrawForward()
        {
            var prevPosition = Position;
            Position = Position.Forward(StepSize, Heading);
            return new Line(prevPosition, Position);
        }

        public void JumpForward()
        {
            Position = Position.Forward(StepSize, Heading);
        }

        public IEnumerable<Line> Interpret(string v)
        {
            foreach (char command in v)
            {
                switch (command)
                {
                    case 'F': yield return DrawForward(); break;
                    case 'f': JumpForward(); break;
                    case '+': TurnLeft(); break;
                    case '-': TurnRight(); break;
                    default: break;
                }
            }

            yield break;
        }
    }

    public interface IPoint
    {
        IPoint Forward(double distance, double angle);
    }

    class Point : IPoint
    {
        private double x;
        private double y;

        public static IPoint Origin = new Point(0, 0);

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public IPoint Forward(double distance, double angle)
        {
            return new Point(
                x + distance * System.Math.Cos(angle * System.Math.PI / 180.0),
                y + distance * System.Math.Sin(angle * System.Math.PI / 180.0));
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point;
            var equal = other != null &&
                System.Math.Abs(x - other.x) <= 0.00000001 &&
                System.Math.Abs(y - other.y) <= 0.00000001;
            return equal;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }

    class Line
    {
        private IPoint point1;
        private IPoint point2;

        public Line(IPoint point1, IPoint point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Line;
            bool equal = other != null && point1.Equals(other.point1) && point2.Equals(other.point2);
            return equal;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + point1.GetHashCode();
            hash = hash * 23 + point2.GetHashCode();
            return hash;
        }
    }

    [TestClass]
    public class TurtleTests
    {
        [TestMethod]
        public void PointEquals()
        {
            Assert.AreEqual(new Point(0, 0), new Point(0, 0));
            Assert.AreNotEqual(new Point(0, 0), new Point(1, 0));
            Assert.AreNotEqual(new Point(0, 0), new Point(0, 1));
        }

        [TestMethod]
        public void PointFullyImplementsEqualsOverride()
        {
            Assert.AreEqual(new Point(0, 0).GetHashCode(), new Point(0, 0).GetHashCode());
            Assert.AreNotEqual(new Point(0, 0).GetHashCode(), new Point(0, 1).GetHashCode());
        }

        [TestMethod]
        public void TurtleCanBeIstantiated()
        {
            var sut = new Turtle(new Point(0, 0), stepSize: 10, angleIncrement: 90);

            Assert.AreEqual(new Point(0, 0), sut.Position);
            Assert.AreEqual(0.0, sut.Heading);
            Assert.AreEqual(10, sut.StepSize);
            Assert.AreEqual(90, sut.AngleIncrement);
        }

        [TestMethod]
        public void PointForward()
        {
            Assert.AreEqual(new Point(1, 0), new Point(0, 0).Forward(1.0, 0));
            Assert.AreEqual(new Point(0, 1), new Point(0, 0).Forward(1.0, 90));
            Assert.AreEqual(new Point(-1, 0), new Point(0, 0).Forward(1.0, 180));
            Assert.AreEqual(new Point(0, -1), new Point(0, 0).Forward(1.0, -90));
        }

        [TestMethod]
        public void TurtleTurningLeft()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            sut.TurnLeft();
            Assert.AreEqual(90, sut.Heading);
        }

        [TestMethod]
        public void TurtleTurningRight()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            sut.TurnRight();
            Assert.AreEqual(-90, sut.Heading);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void TurtleUsesPointToCalculateMoveForwardOnDraws()
        {
            // Why is this test so slow? 120ms
            var mockPoint = new Mock<IPoint>();
            var sut = new Turtle(mockPoint.Object, 1.0, 90);

            sut.DrawForward();

            mockPoint.Verify(
                point => point.Forward(sut.StepSize, sut.Heading),
                Times.Exactly(1));
        }

        [TestMethod]
        public void TurtleUsesPointToCalculateMoveForwardOnJumps()
        {
            var mockPoint = new Mock<IPoint>();
            var sut = new Turtle(mockPoint.Object, 1.0, 90);

            sut.JumpForward();

            mockPoint.Verify(
                point => point.Forward(sut.StepSize, sut.Heading),
                Times.Exactly(1));
        }

        [TestMethod]
        public void TurtleDrawForwardReturnsALine()
        {
            var sut = new Turtle(new Point(0, 0), 1.0, 90.0);
            var expectedLine = new Line(new Point(0, 0), new Point(1, 0));

            var actualLine = sut.DrawForward();

            Assert.AreEqual(expectedLine, actualLine);
        }

        [TestMethod]
        public void TurtleInterpretsCommands_F()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            var expectedLines = new List<Line>() { new Line(new Point(0, 0), new Point(1, 0)) };

            IEnumerable<Line> lines = sut.Interpret("F");
            var actualLines = lines.ToList();

            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        [TestMethod]
        public void TurtleInterpretsCommands_PlusF()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            var expectedLines = new List<Line>() { new Line(new Point(0, 0), new Point(0, 1)) };

            IEnumerable<Line> lines = sut.Interpret("+F");
            var actualLines = lines.ToList();

            CollectionAssert.AreEqual(expectedLines, actualLines);
        }

        [TestMethod]
        public void TurtleInterpretsCommands_MinusF()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            var expectedLines = new List<Line>() { new Line(new Point(0, 0), new Point(0, -1)) };

            IEnumerable<Line> lines = sut.Interpret("-F");
            var actualLines = lines.ToList();

            CollectionAssert.AreEqual(expectedLines, actualLines);
        }

        [TestMethod]
        public void TurtleInterpretsCommands_fF()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            var expectedLines = new List<Line>() { new Line(new Point(1, 0), new Point(2, 0)) };

            IEnumerable<Line> lines = sut.Interpret("fF");
            var actualLines = lines.ToList();

            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        [TestMethod]
        public void TurtleInterpretsCommands_FPlusF()
        {
            var sut = new Turtle(Point.Origin, 1.0, 90);
            var expectedLines = new List<Line>() { 
                new Line(new Point(0, 0), new Point(1, 0)),
                new Line(new Point(1, 0), new Point(1, 1))
            };

            IEnumerable<Line> lines = sut.Interpret("F+F");
            var actualLines = lines.ToList();

            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }
    }
}
