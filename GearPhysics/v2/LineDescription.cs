using Microsoft.Xna.Framework;

namespace GearPhysics.v2
{
    public sealed class LineDescription
    {
        public LineDescription(Vector2 a, Vector2 b)
        {
            this.A = a;
            this.B = b;
        }

        public Vector2 A { get; }
        public Vector2 B { get; }
    }
}
