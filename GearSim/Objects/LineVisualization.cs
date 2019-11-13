using System;
using System.Collections.Generic;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public sealed class LineVisualization : IVisualization
    {
        private readonly short[] Indices;
        private readonly VertexPositionColor[] Vertices;
        private const int CirclePoints = 100;

        public LineVisualization(InvoluteGearShape shape, Color color)
        {
            this.Vertices = new VertexPositionColor[shape.Outline.Count + CirclePoints];
            this.Indices = new short[(shape.Outline.Count + CirclePoints) * 2];

            this.CreateLineList(color, shape.Outline, 0, 0);
            this.CreateLineList(color, CreateCircle(CirclePoints, shape.AxleRadius), shape.Outline.Count, shape.Outline.Count * 2);
        }

        private void CreateLineList(Color color, IReadOnlyList<Vector2> points, int vertexStart, int indexStart)
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

        private List<Vector2> CreateCircle(int steps, float radius)
        {
            var points = new List<Vector2>();

            var stepSize = MathHelper.TwoPi / steps;
            for (var i = 0; i < steps; i++)
            {
                var x = (float)Math.Sin(stepSize * i) * radius;
                var y = (float)Math.Cos(stepSize * i) * radius;
                points.Add(new Vector2(x, y));
            }

            return points;
        }

        public void Draw(Matrix world, GraphicsDevice device, BasicEffect effect)
        {
            effect.World = world;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.Vertices, 0, this.Vertices.Length, this.Indices, 0, this.Indices.Length / 2);
        }
    }
}
