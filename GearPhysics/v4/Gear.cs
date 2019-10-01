using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.v2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.v4
{
    public sealed class Gear
    {
        private readonly List<Line> Lines;
        private readonly GraphicsDevice Device;
        private readonly float ToothSideLength;
        private readonly float ToothHalfSideLength;

        public Gear(GraphicsDevice device, float toothSideLength, int teeth, Vector3 position, float direction)
        {
            this.Lines = new List<Line>();
            
            this.Device = device;
            this.ToothSideLength = toothSideLength;
            this.ToothHalfSideLength = toothSideLength / 2.0f;
            this.NumberOfTeeth = teeth;
            this.Position = position;
            this.Direction = direction;

            var description = GearDescription.DescribeGear(toothSideLength, teeth);
            for (var i = 0; i < description.ClockwiseEdges.Count; i++)
            {
                var edge = description.ClockwiseEdges[i];
                var line = new Line(device, Color.Blue, edge.A, edge.B);
                line.Position = position;
                line.Update();
                this.Lines.Add(line);
            }

            for (var i = 0; i < description.CounterClockwiseEdges.Count; i++)
            {
                var edge = description.CounterClockwiseEdges[i];
                var line = new Line(device, Color.Red, edge.A, edge.B)
                {
                    Position = position
                };
                line.Update();
                this.Lines.Add(line);
            }

            var indicatorLine = new Line(device, Color.White, Vector2.Zero, new Vector2(description.RootDiameter * 0.5f, 0))
            {
                Position = position
            };
            indicatorLine.Update();
            this.Lines.Add(indicatorLine);
        }

        public float Rotation { get; }

        public int NumberOfTeeth { get; }
        public Vector3 Position { get; }
        public float Direction { get; }

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < this.Lines.Count; i++)
            {
                var line = this.Lines[i];
                effect.World = line.World;
                effect.CurrentTechnique.Passes[0].Apply();
                line.LineShape.Draw(this.Device);
            }
        }
    }
}
