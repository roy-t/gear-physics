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

            this.Vertices = new VertexPositionColor[shape.Outline.Count];
            this.Indices = new short[shape.Outline.Count * 2];

            var v = 0;
            for (var i = 0; i < shape.Outline.Count; i++)
            {
                var point = shape.Outline[i];

                var position = new Vector3(point.X, point.Y, 0.0f);

                this.Vertices[i] = new VertexPositionColor(position, color);
                this.Indices[v++] = (short)i;
                this.Indices[v++] = (short)((i + 1) % shape.Outline.Count);
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

            var distance = (parentShape.PitchDiameter / 2.0f) + (childShape.PitchDiameter / 2.0f);

            var offsetX = (float)Math.Cos(jointAngle) * distance;
            var offsetY = (float)Math.Sin(jointAngle) * distance;

            child.Position = this.Position + new Vector3(offsetX, offsetY, 0);
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

                child.Rotation = MathHelper.Pi - ((parentRotation + jointAngle) * gearRatio) - jointAngle;                 
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
