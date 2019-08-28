using Microsoft.Xna.Framework;

namespace GearPhysics.GearMath
{
    public static class LineMath
    {
        public static Vector2[] ShortestDistance(Vector2[] lineA, Vector2[] lineB)
        {



            return null;
        }


        public static Vector2 ShortestDistance(Vector2[] line, Vector2 point)
        {
            var p1 = line[0];
            var p2 = line[1];
            var p3 = point;

            var x1 = p1.X;
            var y1 = p1.Y;
            var x2 = p2.X;
            var y2 = p2.Y;
            var x3 = p3.X;
            var y3 = p3.Y;

            var above = (x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1);
            var below = Vector2.DistanceSquared(p2, p1);

            var u = MathHelper.Clamp(above / below, 0, 1);

            var x = x1 + (u * (x2 - x1));
            var y = y1 + (u * (y2 - y1));

            return new Vector2(x, y);
        }
    }
}
