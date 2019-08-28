using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    sealed class SimLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private KeyboardState lastState;

        private BasicEffect effect;
        
        private IScreen current;
        private List<IScreen> screens;        

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


            screens = new List<IScreen>
            {
                new GearsScreen(GraphicsDevice),
                new LineScreen(GraphicsDevice)
            };

            current = screens[1];

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (keyboard.IsKeyDown(Keys.Tab) && lastState.IsKeyUp(Keys.Tab))
            {
                var index = screens.IndexOf(current);
                current = screens[(index + 1) % screens.Count];
            }

            current.Update(gameTime, keyboard);

            lastState = keyboard;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            current.Draw(effect);
            
                                    
            base.Draw(gameTime);
        }

    }
}
