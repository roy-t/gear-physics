using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    public interface IScreen
    {
        void Draw(BasicEffect effect);
        void Update(GameTime gameTime, KeyboardState keyboard);
    }
}