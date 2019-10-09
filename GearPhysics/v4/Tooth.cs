using GearPhysics.Bodies;
using Microsoft.Xna.Framework;

namespace GearPhysics.v4
{
    public sealed class Tooth
    {        
        public Tooth(Line clockwiseLine, Line counterClockwiseLine)
        {
            this.ClockwiseLine = clockwiseLine;
            this.CounterClockwiseLine = counterClockwiseLine;
        }

        public Line ClockwiseLine { get; }
        public Line CounterClockwiseLine { get; }

        public Vector2 CenterOfMassTransformed()
        {
            var sum = Vector2.Zero;

            sum += this.ClockwiseLine.OutlineTransformed[0];
            sum += this.ClockwiseLine.OutlineTransformed[1];
            sum += this.CounterClockwiseLine.OutlineTransformed[0];
            sum += this.CounterClockwiseLine.OutlineTransformed[1];

            return sum / 4;
        }
    }
}
