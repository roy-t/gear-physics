using System;
using System.Collections.Generic;
using System.Linq;
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

        // Calculates a gear with perfect equilateral triangles as teeth teeth
        // TODO: position it properly!
        public static GearDescription DescribeGear(float toothSideLength, int teeth)
        {
            Assert(toothSideLength > float.Epsilon);
            Assert(teeth > 0);

            var halfToothSideLength = toothSideLength / 2.0f;

            //var diameter = Diameter(toothSideLength, teeth);
            var diameter = (toothSideLength * teeth) / MathHelper.Pi;
            var diameterDifference = (float)(halfToothSideLength * Math.Sqrt(3.0f));

            
            var angleStep = MathHelper.TwoPi / teeth;

            var description = new GearDescription(diameter + diameterDifference, diameter, teeth);            

            var center = new Vector2(0, 0);

            var lastPosition = new Vector2(diameter / 2, -halfToothSideLength);
            var lastAngle = 0.0f;
            for(var i = 0; i < teeth; i++)
            {
                var min = lastPosition;
                var forward = Vector2.TransformNormal(Vector2.UnitY, Matrix.CreateRotationZ(lastAngle));
                var max = min + (forward * toothSideLength);

                var tipBase = (min + max) / 2.0f;
                var normal = Vector2.Normalize(tipBase - center);
                var tipTop = tipBase + (normal * diameterDifference);


                description.ClockwiseEdges.Add(new LineDescription(tipTop, min));
                description.CounterClockwiseEdges.Add(new LineDescription(tipTop, max));
                description.Edges.Add(new LineDescription(min, max));

                //description.ClockwiseEdges.Add(new LineDescription(tipTop, min));
                //description.CounterClockwiseEdges.Add(new LineDescription(tipTop, max));


                //description.ClockwiseEdges.Add(new LineDescription(center, tipBase));
                //description.CounterClockwiseEdges.Add(new LineDescription(min, max));

                lastPosition = max;
                lastAngle += angleStep;                
            }


            return description;
            
            //var rootDiameter = (toothSideLength * teeth) / MathHelper.Pi;

            //// The height of an equilateral triangle is the length of a side divided by 2 time sqrt(3)
            
            //var diameterDifference = (float)(toothSideLength * Math.Sqrt(3.0f));
            //var outsideDiameter = rootDiameter + diameterDifference;

            //return DescribeGear(outsideDiameter, rootDiameter, teeth, 0.0f);
        }

        private static float Diameter(float toothSideLength, int teeth)
        {
            var points = new List<Vector2>();

            var halfToothSideLength = toothSideLength / 2.0f;

            var angleStep = MathHelper.TwoPi / teeth;

            var lastPosition = new Vector2(0, -halfToothSideLength);
            var lastAngle = 0.0f;
            for (var i = 0; i < teeth; i++)
            {
                var min = lastPosition;
                var forward = Vector2.TransformNormal(Vector2.UnitY, Matrix.CreateRotationZ(lastAngle));
                var max = min + (forward * toothSideLength);

                var tipBase = (min + max) / 2.0f;

                points.Add(max);

                lastPosition = max;
                lastAngle += angleStep;
            }



            var xMax = points.Select(v => v.X).Max();
            var xMin = points.Select(v => v.X).Min();


            return xMax - xMin;
        }


        public static GearDescription DescribeGear(float outsideDiameter, float rootDiameter, int teeth, float profileRatio)
        {
            Assert(outsideDiameter > rootDiameter);
            Assert(teeth > 0);
            Assert(profileRatio >= 0.0f && profileRatio < 1.0f);

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

                if (profileRatio > 0.0f)
                {
                    var tip = new LineDescription(riseEnd, fallStart);
                    description.Edges.Add(tip);
                }

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
