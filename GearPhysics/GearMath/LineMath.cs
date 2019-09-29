using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GearPhysics.GearMath
{
    public static class LineMath
    {
        public static Vector2[] ShortestDistance(Vector2[] lineA, Vector2[] lineB)
        {
            // [A0-A1] -> B0
            var v0 = ShortestDistance(lineA[0], lineA[1], lineB[0]);
            var d0 = Vector2.Distance(v0, lineB[0]);

            // [A0-A1] -> B1
            var v1 = ShortestDistance(lineA[0], lineA[1], lineB[1]);
            var d1 = Vector2.Distance(v1, lineB[1]);

            // [B0-B1] -> A0
            var v2 = ShortestDistance(lineB[0], lineB[1], lineA[0]);
            var d2 = Vector2.Distance(v2, lineA[0]);

            // [B0-B1] -> A2
            var v3 = ShortestDistance(lineB[0], lineB[1], lineA[1]);
            var d3 = Vector2.Distance(v3, lineA[1]);


            if (IsMin(d0, d0, d1, d2, d3))
            {
                return new Vector2[] { v0, lineB[0] };
            }

            if (IsMin(d1, d0, d1, d2, d3))
            {
                return new Vector2[] { v1, lineB[1] };
            }

            if (IsMin(d2, d0, d1, d2, d3))
            {
                return new Vector2[] { v2, lineA[0] };
            }

            // d3
            return new Vector2[] { v3, lineA[1] };
        }

        /// <summary>
        /// Shortest distance between a line segment defined by vectors a and b and point p
        /// </summary>        
        public static Vector2 ShortestDistance(Vector2 a, Vector2 b, Vector2 p)
        {
            var p1 = a;
            var p2 = b;
            var p3 = p;

            var x1 = p1.X;
            var y1 = p1.Y;
            var x2 = p2.X;
            var y2 = p2.Y;
            var x3 = p3.X;
            var y3 = p3.Y;

            var above = (x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1);

            // Prevent a division by zero if A and B are the same point
            var below = Math.Max(Vector2.DistanceSquared(p2, p1), float.Epsilon);

            var u = MathHelper.Clamp(above / below, 0, 1);

            var x = x1 + (u * (x2 - x1));
            var y = y1 + (u * (y2 - y1));

            return new Vector2(x, y);
        }

        public static bool PenetrationOfAIntoB(IList<Vector2> lineA, IList<Vector2> lineB, out float penetration)
        {
            penetration = 0.0f;

            if(Intersection(lineA, lineB, out var intersectionPoint))
            {
                penetration = PenetrationOfAIntoB(lineA, intersectionPoint);
                return true;
            }

            return false;
        }

        public static float PenetrationOfAIntoB(IList<Vector2> lineA, Vector2 intersectionPoint)
        {
            return Math.Min(Vector2.Distance(lineA[0], intersectionPoint), Vector2.Distance(lineA[1], intersectionPoint));
        }

        public static bool Intersection(IList<Vector2> lineA, IList<Vector2> lineB, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;

            var p1 = lineA[0];
            var p2 = lineA[1];
            var p3 = lineB[0];
            var p4 = lineB[1];

            var x1 = p1.X;
            var y1 = p1.Y;
            var x2 = p2.X;
            var y2 = p2.Y;
            var x3 = p3.X;
            var y3 = p3.Y;
            var x4 = p4.X;
            var y4 = p4.Y;

            var denominator = ((y4 - y3) * (x2 - x1)) - ((x4 - x3) * (y2 - y1));

            if (denominator == 0)
            {
                // Lines are parallel
                return false;
            }

            var a = ((x4 - x3) * (y1 - y3)) - ((y4 - y3) * (x1 - x3));            
            var b = ((x2 - x1) * (y1 - y3)) - ((y2 - y1) * (x1 - x3));
            

            var ua = a / denominator;
            var ub = b / denominator;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                var x = x1 + (ua * (x2 - x1));
                var y = y1 + (ua * (y2 - y1));

                intersectionPoint = new Vector2(x, y);

                return true;
            }

            return false;
        }

        private static bool IsMin(float target, params float[] others) => others.All(f => f >= target);
    }
}
