using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.GearMath;
using GearPhysics.v2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.v4
{
    public sealed class Gear
    {
        private readonly List<Line> Lines;
        private readonly List<Tooth> Teeth;
        private readonly GraphicsDevice Device;
        private readonly float ToothSideLength;
        private readonly float ToothHalfSideLength;

        private readonly Line TempLine;

        public Gear(GraphicsDevice device, float toothSideLength, int teeth, Vector3 position, float direction)
        {
            this.Lines = new List<Line>();
            this.Teeth = new List<Tooth>();

            this.Device = device;
            this.ToothSideLength = toothSideLength;
            this.ToothHalfSideLength = toothSideLength / 2.0f;
            this.NumberOfTeeth = teeth;
            this.Position = position;
            this.Direction = direction;

            var description = GearDescription.DescribeGear(toothSideLength, teeth);
            this.OutsideDiameter = description.OutsideDiameter;
            this.RootDiameter = description.RootDiameter;

            for (var i = 0; i < teeth; i++)
            {
                var cw = CreateLine(device, position, description.ClockwiseEdges[i], Color.Blue);
                this.Lines.Add(cw);

                var ccw = CreateLine(device, position, description.CounterClockwiseEdges[i], Color.Red);
                this.Lines.Add(ccw);


                this.Teeth.Add(new Tooth(cw, ccw));
            }

            var indicatorLine = new Line(device, Color.White, Vector2.Zero, new Vector2(description.OutsideDiameter * 0.5f, 0))
            {
                Position = position
            };
            indicatorLine.Update();
            this.Lines.Add(indicatorLine);

            this.TempLine = new Line(device, Color.Yellow, Vector2.Zero, Vector2.One * 0.05f)
            {
                Position = position
            };
            this.TempLine.Update();
            this.Lines.Add(this.TempLine);
        }

        private static Line CreateLine(GraphicsDevice device, Vector3 position, LineDescription edge, Color color)
        {
            var line = new Line(device, color, edge.A, edge.B)
            {
                Position = position
            };

            line.Update();

            return line;
        }

        public float Rotation { get; private set; }

        public int NumberOfTeeth { get; }
        public Vector3 Position { get; }
        public float Direction { get; }
        public float OutsideDiameter { get; }
        public float RootDiameter { get; }

        public void Spin(float radians)
        {
            this.Rotation += radians;
            this.Rotation %= MathHelper.TwoPi;
            for (var i = 0; i < this.Lines.Count; i++)
            {
                var line = this.Lines[i];
                line.Angle = this.Rotation;
                line.Update();
            }
        }

        public void Transfer(float radians, Gear target)
        {
            var ratio = this.NumberOfTeeth / (float)target.NumberOfTeeth;
            var targetRadians = radians * ratio;

            target.Spin(targetRadians * target.Direction);

            // Solve colisions due to small inaccuracies

            // 1. Find tooth closest to target gear
            var targetPosition2D = new Vector2(target.Position.X, target.Position.Z);

            var myClosestTooth = GetToothClosestToPoint(targetPosition2D);


            // 2. Find tooth from target gear to us
            var myClosestToothCom = myClosestTooth.CenterOfMassTransformed();
            var targetClosestTooth = target.GetToothClosestToPoint(myClosestToothCom);

            // 3. Check if they overlap
            if (Direction == 1)
            {
                if (LineMath.PenetrationOfAIntoB(myClosestTooth.CounterClockwiseLine.OutlineTransformed, targetClosestTooth.ClockwiseLine.OutlineTransformed, out var point))
                {
                    var adjustment = (MathHelper.TwoPi / target.NumberOfTeeth) / 300.0f;
                    target.Spin(adjustment * target.Direction);
                }
                else if (LineMath.PenetrationOfAIntoB(myClosestTooth.ClockwiseLine.OutlineTransformed, targetClosestTooth.CounterClockwiseLine.OutlineTransformed, out var point2))
                {
                    var adjustment = -(MathHelper.TwoPi / target.NumberOfTeeth) / 300.0f;
                    target.Spin(adjustment * target.Direction);
                }
            }
            // TODO: code path for when we rotate in the other direction
        }

        public Tooth GetToothClosestToPoint(Vector2 point)
        {
            var bestTooth = this.Teeth[0];
            var bestDistance = float.MaxValue;
            for (var i = 0; i < this.Teeth.Count; i++)
            {
                var tooth = this.Teeth[i];
                var centerOfMassPosition = tooth.CenterOfMassTransformed();
                var distance = Vector2.Distance(centerOfMassPosition, point);

                if (distance < bestDistance)
                {
                    bestTooth = tooth;
                    bestDistance = distance;
                }
            }

            // TODO: temp
            var com = bestTooth.CenterOfMassTransformed();
            this.TempLine.Position = new Vector3(com.X, 0, com.Y);
            this.TempLine.Update();

            return bestTooth;
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
