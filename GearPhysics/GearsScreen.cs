using System.Collections.Generic;
using GearPhysics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    public sealed class GearsScreen : IScreen
    {
        private readonly GraphicsDevice Device;

        public GearsScreen(GraphicsDevice device)
        {
            var radius = 20.0f;
            var teeth = 20;
            var tipRatio = 0.3f;
            var toothHeight = 4.0f;
            var toothOverlap = 0.75f;

            var gearA = new Gear(device, Color.Red, Vector3.Zero, radius, teeth, tipRatio, toothHeight);
            var gearB = new Gear(device, Color.Blue, Vector3.Right * (radius + toothHeight * toothOverlap) * 2, radius, teeth, tipRatio, toothHeight);

            GearList = new List<Gear>() { gearA, gearB };

            this.Device = device;
        }

        public List<Gear> GearList { get; }

        public void Update(GameTime gameTime, KeyboardState keyboard)
        {            
            if (keyboard.IsKeyDown(Keys.Space))
            {
                GearList[0].Spin(0.075);

            }
            else
            {
                GearList[0].Spin(0.0);
                GearList[1].Spin(0.0);
            }

            for (var i = 0; i < GearList.Count; i++)
            {
                var gear = GearList[i];

                gear.Update(gameTime);
            }

            GearList[0].Transfer(GearList[1]);            
        }

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < GearList.Count; i++)
            {
                var gear = GearList[i];

                effect.World = gear.World;
                effect.CurrentTechnique.Passes[0].Apply();
                gear.GearShape.Draw(Device);
                gear.LeadingEdgesShape.Draw(Device);
                gear.TrailingEdgesShape.Draw(Device);

            }
        }
    }
}
