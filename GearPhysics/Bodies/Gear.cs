using System;
using System.Collections.Generic;
using GearPhysics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.Bodies
{
    public sealed class Gear
    {
        private const double PI = 3.141592653589793238462643383279502884;
        private readonly GraphicsDevice device;
        private readonly Color color;
        private readonly float radius;
        private readonly int numberOfTeeth;
        private readonly float tipRatio;
        private readonly float toothHeight;
        private double angularVelocity; // radians per second

        private double angle;

        public Gear(GraphicsDevice device, Color color, Vector3 position, float radius, int numberOfTeeth, float tipRatio, float toothHeight)
        {
            this.device = device;
            this.color = color;
            this.Position = position;
            this.radius = radius;
            this.numberOfTeeth = numberOfTeeth;
            this.tipRatio = tipRatio;
            this.toothHeight = toothHeight;
            this.World = Matrix.CreateTranslation(position);

            CreateShape();

        }        

        public Shape GearShape { get; private set; }
        public Shape LeadingEdgesShape { get; private set; }
        public Shape TrailingEdgesShape { get; private set; }
        public Vector3 Position { get; }
        public Matrix World { get; set; }

        private void CreateShape()
        {
            var gearVertices = new List<Vector2>();
            var leadingVertices = new List<Vector2>();
            var trailingVertices = new List<Vector2>();

            var stepSize = MathHelper.TwoPi / numberOfTeeth;
            MathHelper.Clamp(tipRatio, 0f, 1f);
            var toothTipStepSize = (stepSize / 2f) * tipRatio;

            var toothAngleStepSize = (stepSize - (toothTipStepSize * 2f)) / 2f;

            for (var i = numberOfTeeth - 1; i >= 0; --i)
            {
                var vertex = Vector2.Zero;

                if (toothTipStepSize > 0f)
                {
                    vertex = new Vector2(radius *
                                    (float)Math.Cos(stepSize * i + toothAngleStepSize * 2f + toothTipStepSize),
                                    -radius *
                                    (float)Math.Sin(stepSize * i + toothAngleStepSize * 2f + toothTipStepSize));
                    gearVertices.Add(vertex);
                    leadingVertices.Add(vertex);

                    vertex = new Vector2((radius + toothHeight) *
                                    (float)Math.Cos(stepSize * i + toothAngleStepSize + toothTipStepSize),
                                    -(radius + toothHeight) *
                                    (float)Math.Sin(stepSize * i + toothAngleStepSize + toothTipStepSize));
                    gearVertices.Add(vertex);
                    leadingVertices.Add(vertex);
                }

                vertex = new Vector2((radius + toothHeight) *
                                         (float)Math.Cos(stepSize * i + toothAngleStepSize),
                                         -(radius + toothHeight) *
                                         (float)Math.Sin(stepSize * i + toothAngleStepSize));
                gearVertices.Add(vertex);
                trailingVertices.Add(vertex);

                vertex = new Vector2(radius * (float)Math.Cos(stepSize * i),
                                         -radius * (float)Math.Sin(stepSize * i));
                gearVertices.Add(vertex);
                trailingVertices.Add(vertex);
            }

            GearShape = new Shape(device, color * 0.25f, gearVertices, true);
            LeadingEdgesShape = new Shape(device, Color.Red, leadingVertices, false);
            TrailingEdgesShape = new Shape(device, Color.Blue, trailingVertices, false);
        }


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

            var radii = GearShape.Radius + target.GearShape.Radius;

            if(distance < radii)
            {
                if (GotGrip(target))
                {
                    target.Spin(-angularVelocity * 2);
                }
            }
        }

        private bool GotGrip(Gear target)
        {
            return true;
        }
    }
}
