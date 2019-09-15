using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using static System.Diagnostics.Debug;

namespace GearPhysics.v2
{
    public sealed class GearDescription
    {
        private GearDescription(float outsideDiameter, float rootDiameter, int teeth)
        {
            this.OutsideDiameter = outsideDiameter;
            this.RootDiameter = rootDiameter;
            this.Teeth = teeth;

            this.ClockwiseEdges = new List<LineDescription>();
            this.CounterClockwiseEdges = new List<LineDescription>();
            this.Edges = new List<LineDescription>();
        }

        public List<LineDescription> ClockwiseEdges { get; private set; }
        public List<LineDescription> CounterClockwiseEdges { get; set; }

        public List<LineDescription> Edges { get; private set; }

        public float OutsideDiameter { get; }
        public float RootDiameter { get; }
        public int Teeth { get; }


        public static GearDescription DescribeGear(float outsideDiameter, float rootDiameter, int teeth, float profileRatio)
        {
            Assert(outsideDiameter > rootDiameter);
            Assert(teeth > 0);
            Assert(profileRatio > 0.0f && profileRatio < 1.0f);

            var outsideRadius = outsideDiameter / 2.0f;
            var rootRadius = rootDiameter / 2.0f;

            var description = new GearDescription(outsideDiameter, rootDiameter, teeth);
            var stepSize = MathHelper.TwoPi / teeth;
            var tipStepSize = stepSize / 2.0f * profileRatio;
            var riseStepSize = (stepSize - (tipStepSize * 2.0f)) / 2.0f;
            var fallStepSize = riseStepSize;

            // We're traversing the outline of the gear in a counter clockwise fashion
            // starting at the three on the clock
            for(var i = 0; i < teeth; i++)
            {
                var position = stepSize * i;
                
                var riseStart = Vertex(position, rootRadius);
                position += riseStepSize;

                var riseEnd = Vertex(position, outsideRadius);
                position += tipStepSize;

                var fallStart = Vertex(position, outsideRadius);
                position += fallStepSize;

                var fallEnd = Vertex(position, rootRadius);
                position = stepSize * (i + 1);

                var next = Vertex(position, rootRadius);

                var rise = new LineDescription(riseStart, riseEnd);
                description.ClockwiseEdges.Add(rise);
                description.Edges.Add(rise);


                var tip = new LineDescription(riseEnd, fallStart);
                description.Edges.Add(tip);


                var fall = new LineDescription(fallStart, fallEnd);
                description.CounterClockwiseEdges.Add(fall);
                description.Edges.Add(fall);

                var connection = new LineDescription(fallEnd, next);
                description.Edges.Add(connection);
            }

            return description;
        }

        private static Vector2 Vertex(float step, float radius) => new Vector2(Cos(step) * radius,Sin(step) * radius);
        private static float Cos(float f) => (float)Math.Cos(f);
        private static float Sin(float f) => (float)Math.Sin(f);
    }
}
