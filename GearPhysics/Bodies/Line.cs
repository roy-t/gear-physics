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

        private double angle; // radians
        private double angularVelocity; // radians per second

        public Line(GraphicsDevice device, Color color, Vector3 position, float length)
        {
            this.Device = device;
            this.Position = position;
            this.World = Matrix.CreateTranslation(position);

            var flatPosition = new Vector2(position.X, position.Z);
            
            Outline = new List<Vector2>()
            {
                flatPosition - (Vector2.UnitX * (length / 2)),
                flatPosition + (Vector2.UnitX * (length / 2))
            };

            this.LineShape = new Shape(device, color, Outline, false);
        }

        public List<Vector2> Outline { get; private set; }
        public List<Vector2> OutlineTransformed { get; private set; }

        public Vector3 Position { get; }
        public Matrix World { get; set; }
        public Shape LineShape { get; }

        public void Spin(double angularVelocity) => this.angularVelocity = angularVelocity;

        public void Update(GameTime gameTime)
        {
            var instantAngularVelocity = angularVelocity * gameTime.ElapsedGameTime.TotalSeconds;
            angle = (angle + instantAngularVelocity) % (PI * 2);

            World = Matrix.CreateRotationY((float)angle) * Matrix.CreateTranslation(Position);


            var outlineTransformed = new List<Vector2>(Outline.Count);
            for (var i = 0; i < Outline.Count; i++)
            {
                var v = Outline[i];
                var v3 = new Vector3(v.X, 0, v.Y);

                var v3Transformed = Vector3.Transform(v3, World);
                // TODO: what if I use Vector2.Transform?)

                outlineTransformed.Add(new Vector2(v3Transformed.X, v3Transformed.Z));                
            }

            OutlineTransformed = outlineTransformed;
        }
    }
}
