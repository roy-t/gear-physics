using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public sealed class Gear
    {
        private readonly short[] Indices;
        private readonly VertexPositionColor[] Vertices;

        private readonly float baseAngleRad;

        public Gear(InvoluteGearShape shape, Color color)
        {
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

            this.Teeth = shape.Teeth;
            this.baseAngleRad = MathHelper.ToRadians(shape.BaseAngle);
        }

        public int Teeth { get; }
        
        public Vector3 Position { get; set; }

        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation { get; set; }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            effect.World =  Matrix.CreateRotationZ(this.Rotation + this.baseAngleRad) * Matrix.CreateTranslation(this.Position);
            effect.CurrentTechnique.Passes[0].Apply();

            device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.Vertices, 0, this.Vertices.Length, this.Indices, 0, this.Indices.Length / 2);
        }
    }
}
