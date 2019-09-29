using System;
using System.Collections.Generic;
using GearPhysics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.Bodies
{
    public sealed class Line
    {
        private const double PI = 3.141592653589793238462643383279502884;
        private readonly GraphicsDevice Device;
        
        private const double angularFriction = 0.033f;
        private const double MinAngularVelocity = 0.02;

        public Line(GraphicsDevice device, Color color, Vector3 position, float length)
        {
            this.Device = device;
            this.Position = position;
            this.Length = length;
            this.World = Matrix.CreateTranslation(position);

            Outline = new List<Vector2>()
            {
                -Vector2.UnitX * (length / 2),
                Vector2.UnitX * (length / 2)
            };

            this.LineShape = new Shape(device, color, Outline, false);

            Update(new GameTime());
        }

        public Line(GraphicsDevice device, Color color, Vector2 a, Vector2 b)
        {
            this.Device = device;
            this.Position = Vector3.Zero;
            this.Length = Vector2.Distance(a, b);
            this.World = Matrix.CreateTranslation(this.Position);

            Outline = new List<Vector2>() { a, b };

            this.LineShape = new Shape(device, color, Outline, false);

            Update(new GameTime());
        }

        public List<Vector2> Outline { get; private set; }
        public List<Vector2> OutlineTransformed { get; private set; }

        public Vector3 Position { get; set; }
        public float Length { get; }
        public double Angle { get; set; }
        // radians per second
        public double AngularVelocity { get; private set; }

        public Matrix World { get; set; }
        public Shape LineShape { get; }

        public void Spin(double angularVelocity) => this.AngularVelocity = angularVelocity;

        public void Update(GameTime gameTime)
        {
            var elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            var instantAngularVelocity = AngularVelocity * elapsed;
            AngularVelocity *= (1.0 - angularFriction);

            AngularVelocity = Math.Abs(AngularVelocity) < MinAngularVelocity ? 0 : AngularVelocity;

            Angle = (Angle + instantAngularVelocity) % (PI * 2);

            Update();           
        }

        public void Update()
        {
            World = Matrix.CreateRotationY((float)Angle) * Matrix.CreateTranslation(Position);


            var outlineTransformed = new List<Vector2>(Outline.Count);
            for (var i = 0; i < Outline.Count; i++)
            {
                var v = Outline[i];
                var v3 = new Vector3(v.X, 0, v.Y);
                var v3Transformed = Vector3.Transform(v3, World);
                outlineTransformed.Add(new Vector2(v3Transformed.X, v3Transformed.Z));
            }

            OutlineTransformed = outlineTransformed;
        }
    }
}
