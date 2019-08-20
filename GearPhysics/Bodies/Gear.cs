using GearPhysics.Shapes;
using Microsoft.Xna.Framework;

namespace GearPhysics.Bodies
{
    public sealed class Gear
    {
        private const double PI = 3.141592653589793238462643383279502884;

        private double angularVelocity; // radians per second

        private double angle;

        public Gear(Shape shape, Vector3 position)
        {
            Shape = shape;
            Position = position;

            World = Matrix.CreateTranslation(position);
        }

        public Shape Shape { get; }
        public Vector3 Position { get; }
        public Matrix World { get; set; }


        public void Spin(double angularVelocity)
        {
            this.angularVelocity = angularVelocity;
        }

        public void Update(GameTime gameTime)
        {
            var instantAngularVelocity = angularVelocity * gameTime.ElapsedGameTime.TotalSeconds;
            angle = (angle + instantAngularVelocity) % (PI * 2);

            World = Matrix.CreateRotationY((float)angle) * Matrix.CreateTranslation(Position);
        }

        public void Transfer(Gear target)
        {
            var targetPosition = target.Position;

            var distance = Vector3.Distance(this.Position, targetPosition);

            var radii = Shape.Radius + target.Shape.Radius;

            if(distance < radii)
            {
                if (GotGrip(target))
                {
                    target.Spin(-angularVelocity);
                }
            }
        }

        private bool GotGrip(Gear target)
        {
            return true;
        }
    }
}
