using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery.RT
{
    [TestClass]
    public class RTTests
    {
        class Point
        {
            public Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public readonly double X, Y, Z;

            public double Magnitude
            {
                get
                {
                    return Math.Sqrt(X * X + Y * Y + Z * Z);
                }
            }

            public Point Normal
            {
                get
                {
                    return new Point(X / Magnitude, Y / Magnitude, Z / Magnitude);
                }
            }

            public static Point operator -(Point a, Point b)
            {
                return new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }
        }

        class Ray
        {
            public Ray(Point origin, Point direction)
            {
                Origin = origin;
                Direction = direction.Normal;
            }

            public readonly Point Origin;
            public readonly Point Direction;

            public Point PointAt(double distance)
            {
                return new Point(Origin.X + Direction.X * distance,
                              Origin.Y + Direction.Y * distance,
                              Origin.Z + Direction.Z * distance);
            }
        }

        class Sphere
        {
            public Sphere(Point center, double radius)
            {
                Center = center;
                Radius = radius;
            }

            public readonly Point Center;
            public readonly Double Radius;

            public Point Intersection(Ray ray)
            {
                return AlgebraicIntersection(ray);
            }

            public Point Normal(Point point)
            {
                return new Point((point.X - Center.X) / Radius,
                              (point.Y - Center.Y) / Radius,
                              (point.Z - Center.Z) / Radius);
            }

            Point AlgebraicIntersection(Ray ray)
            {
                double b = 2 * (ray.Direction.X * (ray.Origin.X - Center.X) +
                               ray.Direction.Y * (ray.Origin.Y - Center.Y) +
                               ray.Direction.Z * (ray.Origin.Z - Center.Z));
                double c = Math.Pow(ray.Origin.X - Center.X, 2) +
                          Math.Pow(ray.Origin.Y - Center.Y, 2) +
                          Math.Pow(ray.Origin.Z - Center.Z, 2) -
                          Math.Pow(Radius, 2);
                double discriminant = Math.Pow(b, 2) - 4 * c;
                if (discriminant <= 0)
                {
                    return null;
                }
                double distance = (-b - Math.Sqrt(discriminant)) / 2;
                if (distance <= 0)
                {
                    distance = (-b + Math.Sqrt(discriminant)) / 2;
                }

                return ray.PointAt(distance);
            }
        }

        [TestMethod]
        public void SphereIntersection()
        {
            var ray = new Ray(new Point(1, -2, -1), new Point(1, 2, 4));
            var sphere = new Sphere(new Point(3, 0, 5), 3);
            var expectedIntersection = new Point(1.816, -0.368, 2.269);
            var expectedNormal = new Point(-0.395, -0.123, -0.910);

            var actualIntersection = sphere.Intersection(ray);
            var actualNormal = sphere.Normal(actualIntersection);

            AssertPointIsNear(expectedIntersection, actualIntersection);
            AssertPointIsNear(expectedNormal, actualNormal);
        }

        private void AssertPointIsNear(Point expected, Point actual)
        {
            Assert.IsTrue((expected - actual).Magnitude < 0.01);
        }
    }
}
