using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.GearMath;
using GearPhysics.v2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.v3
{
    public sealed class Gear
    {
        private readonly List<Line> Lines;
        private readonly GraphicsDevice Device;

        public Gear(GraphicsDevice device, GearDescription description, Vector3 position, float direction)
        {
            this.Teeth = description.Teeth;
            this.Lines = new List<Line>();

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


            this.Device = device;
            this.Direction = direction;
        }

        public float Rotation { get; private set; }
        public float Direction { get; }
        public int Teeth { get; }

        public void Spin(float step)
        {
            this.Rotation = (this.Rotation + (step * this.Direction)) % MathHelper.TwoPi;
            for (var i = 0; i < this.Lines.Count; i++)
            {
                var line = this.Lines[i];
                line.Angle = this.Rotation;
                line.Update();
            }
        }

        public void Transfer(Gear to, float step)
        {
            var intersects = false;
            var intersectionPoint = Vector2.Zero;
            for(var f = 0; f < this.Lines.Count; f++)
            {
                for (var t = 0; t < to.Lines.Count; t++)
                {
                    var fromLine = this.Lines[f];
                    var toLine = to.Lines[t];

                    if (LineMath.Intersection(fromLine.OutlineTransformed, toLine.OutlineTransformed, out intersectionPoint))
                    {
                        intersects = true;
                        goto outside;
                    }
                }
            }

            outside: 
            if(intersects)
            {
                var ratio = this.Teeth / (float)to.Teeth;
                to.Spin(step * ratio);
            }
        }

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
