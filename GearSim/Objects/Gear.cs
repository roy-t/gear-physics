using System;
using System.Collections.Generic;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public sealed class Gear
    {        
        private readonly List<GearConnection> Children;
        private readonly IVisualization Visualization;

        public Gear(InvoluteGearShape shape, Color color)
        {
            this.Shape = shape;
            this.Children = new List<GearConnection>(0);

            //this.Visualization = new LineVisualization(shape, color);
            this.Visualization = new TriangleVisualization(shape, color);
        }

        public InvoluteGearShape Shape { get; }
        
        public Vector3 Position { get; set; }
        
        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation { get; set; }

        public void AddChild(Gear child, float jointAngle)
        {
            var parentShape = this.Shape;
            var childShape = child.Shape;

            if (parentShape.GearType == GearType.Internal && childShape.GearType == GearType.External)
            {
                var distance = (parentShape.PitchDiameter / 2.0f) - (childShape.PitchDiameter / 2.0f);
                var offsetX = (float)Math.Cos(jointAngle) * distance;
                var offsetY = (float)Math.Sin(jointAngle) * distance;

                child.Position = this.Position + new Vector3(offsetX, offsetY, 0);
            }

            else if (parentShape.GearType == GearType.External && childShape.GearType == GearType.External)
            {
                var distance = (parentShape.PitchDiameter / 2.0f) + (childShape.PitchDiameter / 2.0f);

                var offsetX = (float)Math.Cos(jointAngle) * distance;
                var offsetY = (float)Math.Sin(jointAngle) * distance;

                child.Position = this.Position + new Vector3(offsetX, offsetY, 0);
            }
            else
            {
                throw new NotSupportedException($"Unsupported configuration parent: {parentShape.GearType}, child: {childShape.GearType}");
            }
            this.Children.Add(new GearConnection(child, jointAngle));
        }

        public void Update()
        {
            for (var i = 0; i < this.Children.Count; i++)
            {
                var connection = this.Children[i];
                var child = connection.Child;

                var gearRatio = this.Shape.Teeth / (float)child.Shape.Teeth;

                var parentRotation = this.Rotation;
                var jointAngle = connection.JointAngle;

                if (this.Shape.GearType == GearType.Internal)
                {
                    child.Rotation = MathHelper.Pi + ((parentRotation + jointAngle) * gearRatio) - jointAngle;
                }
                else
                {
                    child.Rotation = MathHelper.Pi - ((parentRotation + jointAngle) * gearRatio) - jointAngle;
                }
                child.Update();
            }
        }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {            
            var world =  Matrix.CreateFromAxisAngle(Vector3.Forward, this.Rotation) * Matrix.CreateTranslation(this.Position);
            this.Visualization.Draw(world, device, effect);            
        }
    }
}
