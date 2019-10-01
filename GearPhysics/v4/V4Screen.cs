using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v4
{
    public sealed class V4Screen : IScreen
    {
        private readonly GraphicsDevice Device;
        private readonly List<Gear> Gears;

        public V4Screen(GraphicsDevice device)
        {            
            this.Device = device;
            this.Gears = new List<Gear>()
            {
                new Gear(device, 1.0f, 24, Vector3.Zero, 1.0f)
            };
        }

        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
        }

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < this.Gears.Count; i++)
            {
                var gear = this.Gears[i];
                gear.Draw(effect);
            }
        }
    }
}
