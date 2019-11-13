using System.Collections.Generic;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public sealed class TriangleVisualization : IVisualization
    {
        private readonly int[] Indices;
        private readonly VertexPositionColor[] Vertices;

        public TriangleVisualization(InvoluteGearShape shape, Color color)
        {            
            var vertices = new List<VertexPositionColor>();
            var indices = new List<int>();

            var outline = shape.Outline;

            var ii = 0;
            for (var i = 0; i < outline.Count; i++)
            {                
                var current =  new Vector3(outline[i].X, outline[i].Y, 0);
                var next = new Vector3(outline[(i + 1) % outline.Count].X, outline[(i + 1) % outline.Count].Y, 0);                
                var altCurrent = Vector3.Normalize(new Vector3(current.X, current.Y, 0)) * shape.AxleRadius;
                var altNext = Vector3.Normalize(new Vector3(next.X, next.Y, 0)) * shape.AxleRadius;

                {
                    var a = new VertexPositionColor(current, color);
                    var b = new VertexPositionColor(altCurrent, color);
                    var c = new VertexPositionColor(altNext, color);

                    vertices.Add(a);
                    vertices.Add(b);
                    vertices.Add(c);
                }

                {
                    var a = new VertexPositionColor(current, color);
                    var b = new VertexPositionColor(altNext, color);
                    var c = new VertexPositionColor(next, color);

                    vertices.Add(a);
                    vertices.Add(b);
                    vertices.Add(c);
                }


                if (shape.GearType == GearType.Internal)
                {
                    indices.Add(ii++);
                    indices.Add(ii++);
                    indices.Add(ii++);
                    indices.Add(ii++);
                    indices.Add(ii++);
                    indices.Add(ii++);
                }
                // To prevent counter clockwise faces from being culled we have to reverse the triangle order for external gears
                else 
                {                    
                    indices.Add(ii + 5);
                    indices.Add(ii + 4);
                    indices.Add(ii + 3);
                    indices.Add(ii + 2);
                    indices.Add(ii + 1);
                    indices.Add(ii + 0);

                    ii += 6;                    
                }
            }            

            this.Vertices = vertices.ToArray();
            this.Indices = indices.ToArray();
        }      

        public void Draw(Matrix world, GraphicsDevice device, BasicEffect effect)
        {
            effect.World = world;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.Vertices, 0, this.Vertices.Length, this.Indices, 0, this.Indices.Length / 3);
        }
    }
}
