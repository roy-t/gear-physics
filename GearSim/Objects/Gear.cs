using System;
using System.Collections.Generic;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public sealed class Gear
    {
        private readonly short[] Indices;
        private readonly VertexPositionColor[] Vertices;

        private readonly List<GearConnection> Children;

        public Gear(InvoluteGearShape shape, Color color)
        {
            this.Shape = shape;
            this.Children = new List<GearConnection>(0);

            this.Vertices = new VertexPositionColor[shape.Outside.Count + shape.Inside.Count];
            this.Indices = new short[(shape.Outside.Count * 2) + (shape.Inside.Count * 2)];

            this.AddCircle(color, shape.Outside, 0, 0);
            this.AddCircle(color, shape.Inside, shape.Outside.Count, shape.Outside.Count * 2);
        }

        private void AddCircle(Color color, IReadOnlyList<Vector2> points, int vertexStart, int indexStart)
        {
            var ii = 0;
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var position = new Vector3(point.X, point.Y, 0.0f);

                this.Vertices[vertexStart + i] = new VertexPositionColor(position, color);
                this.Indices[indexStart + ii++] = (short)(vertexStart + i);
                this.Indices[indexStart + ii++] = (short)(vertexStart + ((i + 1) % points.Count));
            }
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
            effect.World = Matrix.CreateFromAxisAngle(Vector3.Forward, this.Rotation) * Matrix.CreateTranslation(this.Position);
            effect.CurrentTechnique.Passes[0].Apply();

            device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.Vertices, 0, this.Vertices.Length, this.Indices, 0, this.Indices.Length / 2);
        }
    }
}
