using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearSim.Objects
{
    public interface IVisualization
    {
        void Draw(Matrix world, GraphicsDevice device, BasicEffect effect);
    }
}
