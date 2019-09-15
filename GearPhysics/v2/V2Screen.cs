using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.GearMath;
using GearPhysics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v2
{
    public sealed class V2Screen : IScreen
    {
        private readonly GraphicsDevice Device;
        private readonly List<Line> GearA;
        private readonly List<Line> GearB;
        private readonly List<Line> GearC;
        private readonly List<Shape> Indicators;

        public V2Screen(GraphicsDevice device)
        {
            this.Indicators = new List<Shape>();

            this.Device = device;

            var start = -5.0f;
            var step = 9.3f;

            this.GearA = CreateGear(device, Vector3.Right * start, 10, 8, 24, 0.02f);
            this.GearB = CreateGear(device, Vector3.Right * (start + step), 10, 8, 24, 0.02f);
            this.GearC = CreateGear(device, Vector3.Right * (start + step + step), 10, 8, 24, 0.02f);
        }

        private List<Line> CreateGear(GraphicsDevice device, Vector3 position, float outsideDiameter, float rootDiameter, int teeth, float profileRatio)
        {
            var gear = new List<Line>();
            var description = GearDescription.DescribeGear(outsideDiameter, rootDiameter, teeth, profileRatio);


            for (var i = 0; i < description.ClockwiseEdges.Count; i++)
            {
                var edge = description.ClockwiseEdges[i];
                var line = new Line(device, Color.Blue, edge.A, edge.B);
                line.Position = position;
                gear.Add(line);
            }

            for (var i = 0; i < description.CounterClockwiseEdges.Count; i++)
            {
                var edge = description.CounterClockwiseEdges[i];
                var line = new Line(device, Color.Red, edge.A, edge.B);
                line.Position = position;
                gear.Add(line);
            }

            return gear;
        }

        private void UpdateGear(GameTime gameTime, List<Line> gear, double? spin, float shift)
        {
            for (var i = 0; i < gear.Count; i++)
            {
                var line = gear[i];
                if (spin.HasValue)
                {
                    line.Spin(spin.Value);                    
                }
                line.Position += Vector3.Right * shift;
                line.Update(gameTime);
            }
        }

        private void DrawGear(BasicEffect effect, List<Line> gear)
        {
            for (var i = 0; i < gear.Count; i++)
            {
                var line = gear[i];
                effect.World = line.World;
                effect.CurrentTechnique.Passes[0].Apply();
                line.LineShape.Draw(Device);
            }
        }

        private double? Gear(Line a, Line b)
        {
            // TODO leading on leading should add speed, while trailing on trailing should reduce speed
            if (LineMath.Intersection(a.OutlineTransformed.ToArray(), b.OutlineTransformed.ToArray(), out var intersectionPoint))
            {
                Indicators.Add(new Shape(Device, Color.Yellow, PointToCross(intersectionPoint, 0.1f), false));

                // Proportional increase in angular velocity so that things get unstuck
                var d = LineMath.PenetrationOfAIntoB(a.OutlineTransformed.ToArray(), intersectionPoint);
                var dRatio = d / a.Length;

                var spin = -a.AngularVelocity * (1.0 + dRatio);

                return spin;
            }

            return null;
        }

        private double? Spin(List<Line> a, List<Line>b)
        {
            double? spin = null;
            for (var i = 0; i < a.Count; i++)
            {
                var lineA = a[i];
                for (var y = 0; y < b.Count; y++)
                {
                    var lineB = b[y];

                    spin = Gear(lineA, lineB);
                    if (spin != null)
                    {
                        return spin;
                    }
                }
            }

            return spin;
        }

        private List<Vector2> PointToCross(Vector2 point, float radius)
        {
            var outline = new List<Vector2>(8);
            outline.Add(point);
            outline.Add(point + (new Vector2(-1, 1) * radius));

            outline.Add(point);
            outline.Add(point + (new Vector2(1, 1) * radius));

            outline.Add(point);
            outline.Add(point + (new Vector2(1, -1) * radius));

            outline.Add(point);
            outline.Add(point + (new Vector2(-1, -1) * radius));

            return outline;
        }

        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            Indicators.Clear();

            var spinA = keyboard.IsKeyDown(Keys.Space) ?  new double?(.75) : null;
            var spinB = Spin(GearA, GearB);
            var spinC = Spin(GearB, GearC);

            var shift = 0.0f;
            if(keyboard.IsKeyDown(Keys.Left))
            {
                shift = (float)(gameTime.ElapsedGameTime.TotalSeconds * 0.5);
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                shift = (float)(gameTime.ElapsedGameTime.TotalSeconds * -0.5);
            }
            UpdateGear(gameTime, GearA, spinA, -shift);
            UpdateGear(gameTime, GearB, spinB, shift);
            UpdateGear(gameTime, GearC, spinC, -shift);
        }

        public void Draw(BasicEffect effect)
        {
            DrawGear(effect, GearA);
            DrawGear(effect, GearB);
            DrawGear(effect, GearC);

            effect.World = Matrix.Identity;
            for (var i = 0; i < Indicators.Count; i++)
            {
                var shape = Indicators[i];
                effect.CurrentTechnique.Passes[0].Apply();
                shape.Draw(Device);
            }            
        }
    }
}
