using System.Collections.Generic;
using GearPhysics.Bodies;
using GearPhysics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    sealed class SimLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private BasicEffect effect;

        private List<Gear> gearList;

        public SimLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1000,
                PreferredBackBufferHeight = 1000,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            effect = new BasicEffect(Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World              = Matrix.Identity,
                View               = Matrix.CreateLookAt(Vector3.Up, Vector3.Down, Vector3.Forward),
                Projection         = Matrix.CreateOrthographic(100, 100, 0.01f, 100.0f)
            };

            var radius = 20.0f;
            var teeth = 20;
            var tipRatio = 0.3f;
            var toothHeight = 4.0f;
            var toothOverlap = 0.75f;

            var outline = GearShapeFactory.CreateGear(radius, teeth, tipRatio, toothHeight);

            var gearA = new Gear(new Shape(GraphicsDevice, Color.Red, outline), Vector3.Zero);
            var gearB = new Gear(new Shape(GraphicsDevice, Color.Blue, outline), Vector3.Right * (radius + toothHeight * toothOverlap) * 2);

            gearList = new List<Gear>() { gearA, gearB };

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Space))
            {
                gearList[0].Spin(0.075);
                gearList[1].Spin(-0.075);

            }
            else
            {
                gearList[0].Spin(0.0);
                gearList[1].Spin(0.0);
            }

            for (var i = 0; i < gearList.Count; i++)
            {
                var gear = gearList[i];

                gear.Update(gameTime);
            }

            gearList[0].Transfer(gearList[1]);

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            for (var i = 0; i < gearList.Count; i++)
            {                
                var gear = gearList[i];                

                effect.World = gear.World;
                effect.CurrentTechnique.Passes[0].Apply();
                gear.Shape.Draw(GraphicsDevice);
            }
                                    
            base.Draw(gameTime);
        }

    }
}
