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

            public static Point operator -(Point a)
            {
                return new Point(-a.X, -a.Y, -a.Z);
            }

            public static double DotProduct(Point a, Point b)
            {
                return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            }

            public static Point CrossProduct(Point a, Point b)
            {
                return new Point(a.Y * b.Z - a.Z * b.Y,
                                 a.Z * b.X - a.X * b.Z,
                                 a.X * b.Y - a.Y * b.X);
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
            public Point PoleAxis; // XXX make this readonly
            public Point EcuatorAxis; // XXX make this readonly

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

            public MappingParameter InverseMapping(Point normal)
            {
                double φ = Math.Acos(Point.DotProduct(-PoleAxis, normal));
                double v = φ / Math.PI;
                double u = 0;
                // XXX refactor, comparison between doubles
                //v != 0 && v != 1
                if (v > 0.001 || (Math.Abs(v - 1) > 0.001))
                {
                    double θ = Math.Acos(Point.DotProduct(EcuatorAxis, normal) / Math.Sin(φ)) / (2 * Math.PI);
                    // XXX refactor the cross product below is constant for the sphere, precalculate
                    if (Point.DotProduct(Point.CrossProduct(PoleAxis, EcuatorAxis), normal) > 0)
                    {
                        u = θ;
                    }
                    else
                    {
                        u = 1 - θ;
                    }
                }

                return new MappingParameter(u, v);
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

        // XXX create interface with members Intersection/Normal/InverseMapping, for Sphere and Plane
        class Plane
        {
            public Plane(Point normal, double distance)
            {
                NormalToOrigin = normal.Normal;
                Distance = distance;
            }

            public readonly Point NormalToOrigin;
            public readonly double Distance;

            public Point Intersection(Ray ray)
            {
                double vd = Point.DotProduct(NormalToOrigin, ray.Direction);
                if (Math.Abs(vd) < 0.0001)
                {
                    return null;
                }
                double v0 = -(Point.DotProduct(NormalToOrigin, ray.Origin) + Distance);
                double distance = v0 / vd;
                if (distance < 0)
                {
                    return null;
                }
                return ray.PointAt(distance);
            }

            public Point Normal(Point point, Ray ray)
            {
                var vd = Point.DotProduct(NormalToOrigin, ray.Direction);
                if (vd < 0)
                {
                    return NormalToOrigin;
                }
                else
                {
                    return -NormalToOrigin;
                }

            }
        }

        class OrthogonalBox
        {
            public OrthogonalBox(Point minimumExtent, Point maximumExtent)
            {
                MinimumExtent = minimumExtent;
                MaximumExtent = maximumExtent;
            }

            public readonly Point MinimumExtent;
            public readonly Point MaximumExtent;

            public Boolean Hit(Ray ray)
            {
                return false;
            }
        }

        // XXX refactor, extremly similar to Point
        struct MappingParameter
        {
            public MappingParameter(double u, double v)
            {
                // XXX u should range from 0 to 1, at the poles u is 0 (ie. v is 0 or 1)
                // XXX u == 1 should convert it to 0
                // XXX v should range from 0 (south pole) to 1 (north pole)
                U = u;
                V = v;
            }

            public static MappingParameter operator -(MappingParameter a, MappingParameter b)
            {
                return new MappingParameter(a.U - b.U, a.V - b.V);
            }

            public double Magnitude
            {
                get
                {
                    return Math.Sqrt(U * U + V * V);
                }
            }

            public readonly Double U;
            public readonly Double V;
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

        [TestMethod]
        public void SphereInverseMapping()
        {
            var normal = new Point(0.577, -0.577, 0.577);
            // XXX Refactor: make these the default parameter for the sphere
            var sphere = new Sphere(new Point(0, 0, 0), 1);
            sphere.PoleAxis = new Point(0, 0, 1); // XXX refactor, create a Point.Z
            sphere.EcuatorAxis = new Point(1, 0, 0); // XXX refactor, create a Point.X

            MappingParameter parm = sphere.InverseMapping(normal);
            Assert2dPointIsNear(new MappingParameter(0.875, 0.696), parm);
        }

        [TestMethod]
        public void PlaneIntersection()
        {
            var plane = new Plane(new Point(1, 0, 0), -7);
            var ray = new Ray(new Point(2, 3, 4), new Point(0.577, 0.577, 0.577));

            var actualIntersection = plane.Intersection(ray);
            var actualNormal = plane.Normal(actualIntersection, ray);

            AssertPointIsNear(new Point(7, 8, 9), actualIntersection);
            AssertPointIsNear(new Point(-1, 0, 0), actualNormal);
        }

        [TestMethod]
        public void OrthogonalBoxHit()
        {
            var ray = new Ray(new Point(0, 4, 2), new Point(0.218, -0.436, 0.873));
            var box = new OrthogonalBox(new Point(-1, 2, 1), new Point(3, 3, 3));

            Assert.IsFalse(box.Hit(ray));
        }

        // XXX refactor, this is the same than AssertPointIsNear
        private void Assert2dPointIsNear(MappingParameter expected, MappingParameter actual)
        {
            Assert.IsTrue((expected - actual).Magnitude < 0.01);
        }

        private void AssertPointIsNear(Point expected, Point actual)
        {
            Assert.IsTrue((expected - actual).Magnitude < 0.01);
        }
    }
}
