using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v5
{
    public sealed class V5Screen : IScreen
    {
        private readonly GraphicsDevice Device;

        private readonly List<Gear> Gears;
        

        public V5Screen(GraphicsDevice device)
        {
            this.Device = device;

            this.Gears = new List<Gear>()
            {
                new Gear(device, Vector2.UnitX * 0.0f, 10),
                new Gear(device, Vector2.UnitX * 2.5f, 10)
            };

           
        }      

        private void CreateDriveTrain(params int[] teeth)
        {
        }


        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            var driver = this.Gears[0];
            if (keyboard.IsKeyDown(Keys.Space))
            {
                driver.Spin((float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver4);
                this.Gears[1].Spin((float)gameTime.ElapsedGameTime.TotalSeconds * -MathHelper.PiOver4);
            }
        }

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < this.Gears.Count; i++)
            {
                this.Gears[i].Draw(effect);
            }
        }
    }
}
