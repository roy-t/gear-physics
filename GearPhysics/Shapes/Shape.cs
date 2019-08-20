using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Math;

namespace GearPhysics.Shapes
{
    public sealed class Shape
    {
        private readonly VertexPositionColor[] vertices;
        private readonly short[] indices;
        private readonly Texture2D texture;

        public Shape(GraphicsDevice device, Color color, List<Vector2> outline)
        {
            texture = new Texture2D(device, 1, 1);
            texture.SetData(new Color[] { color });


            vertices = new VertexPositionColor[outline.Count];
            indices = new short[vertices.Length * 2];

            var min = Vector2.One * float.MaxValue;
            var max = Vector2.One * float.MinValue;

            var ii = 0;
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = outline[i];
                vertices[i] = new VertexPositionColor(new Vector3(vertex.X, 0, vertex.Y), color);

                indices[ii++] = (short)i;
                indices[ii++] = (short)((i + 1) % vertices.Length);

                min.X = Min(min.X, vertex.X);
                min.Y = Min(min.Y, vertex.Y);
                max.X = Max(max.X, vertex.X);
                max.Y = Max(max.Y, vertex.Y);
            }

            Radius = BoundingSphere.CreateFromPoints(vertices.Select(v => v.Position)).Radius;
        }        

        public float Radius { get; }

        public void Draw(GraphicsDevice device)
        {            
            device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.vertices, 0, this.vertices.Length, this.indices, 0, this.indices.Length / 2);
        }
    }
}
