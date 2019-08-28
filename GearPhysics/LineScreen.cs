using System;
using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.GearMath;
using GearPhysics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    public sealed class LineScreen : IScreen
    {
        private readonly List<Line> LineList;                
        private readonly GraphicsDevice Device;
        private readonly List<Shape> Indicators;


        public LineScreen(GraphicsDevice device)
        {
            var lineA = new Line(device, Color.Blue, Vector3.Zero, 10);
            var lineB = new Line(device, Color.Red, Vector3.Right * 8, 10) { Angle = -0.2f };
            LineList = new List<Line>()
            {
                lineA,
                lineB
            };

            Indicators = new List<Shape>();

            this.Device = device;
        }

        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Space))
            {
                LineList[0].Spin(0.075);
                //LineList[1].Spin(-0.75);

            }
            else
            {
                LineList[0].Spin(0);
                //LineList[1].Spin(0);
            }

            for (var i = 0; i < LineList.Count; i++)
            {
                LineList[i].Update(gameTime);
            }

            var shortest = LineMath.ShortestDistance(LineList[0].OutlineTransformed[0], LineList[0].OutlineTransformed[1], LineList[1].OutlineTransformed[0]);
            var shortestLine = LineMath.ShortestDistance(LineList[0].OutlineTransformed.ToArray(), LineList[1].OutlineTransformed.ToArray());

            Indicators.Clear();

            //Indicators.Add(new Shape(Device, Color.Green, PointToCross(LineList[1].OutlineTransformed[0], 0.2f), false));
            //Indicators.Add(new Shape(Device, Color.Pink, PointToCross(LineList[0].OutlineTransformed[0], 0.2f), false));
            //Indicators.Add(new Shape(Device, Color.Purple, PointToCross(LineList[0].OutlineTransformed[1], 0.2f), false));
            //Indicators.Add(new Shape(Device, Color.Orange, PointToCross(shortest, 0.4f), false));


            //Indicators.Add(new Shape(Device, Color.Yellow, PointToCross(shortestLine[0], 0.04f), false));
            //Indicators.Add(new Shape(Device, Color.Yellow, PointToCross(shortestLine[1], 0.04f), false));

            if(LineMath.Intersection(LineList[0].OutlineTransformed.ToArray(), LineList[1].OutlineTransformed.ToArray(), out var intersectionPoint))
            {
                Indicators.Add(new Shape(Device, Color.Yellow, PointToCross(intersectionPoint, 0.1f), false));

                // Proportional increase in angular velocity so that things get unstuck
                var d = LineMath.PenetrationOfAIntoB(LineList[0].OutlineTransformed.ToArray(), intersectionPoint);                

                var dRatio = d / LineList[0].Length;

                LineList[1].Spin(-LineList[0].AngularVelocity * (1.0 + dRatio));

                Console.WriteLine($"Penetration {d:F4}, L0: {LineList[0].AngularVelocity:F4}, L1: {LineList[1].AngularVelocity:F4}");

                // TODO: what if things intersect and the velocity of line 0 is 0, or quickly decreases?
            }



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

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < LineList.Count; i++)
            {
                var line = LineList[i];
                effect.World = line.World;
                effect.CurrentTechnique.Passes[0].Apply();
                line.LineShape.Draw(Device);
            }

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
