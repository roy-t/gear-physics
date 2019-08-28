using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics
{
    sealed class SimLoop : Game
    {
        private const float ZoomSpeed = 0.2f;
        private const float TranslateSpeed = 2.2f;
        private readonly GraphicsDeviceManager Graphics;
        private KeyboardState lastState;

        private BasicEffect effect;
        
        private IScreen current;
        private List<IScreen> screens;

        private float zoom = 1.0f;
        private Vector3 origin = Vector3.Right * 4;

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
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            if(keyboard.IsKeyDown(Keys.E))
            {
                zoom = zoom - (ZoomSpeed * elapsed);
            }
            else if (keyboard.IsKeyDown(Keys.Q))
            {
                zoom = zoom + (ZoomSpeed * elapsed);
            }


            if(keyboard.IsKeyDown(Keys.A))
            {
                origin += Vector3.Left * TranslateSpeed * elapsed;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                origin += Vector3.Right * TranslateSpeed * elapsed;
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                origin += Vector3.Backward * TranslateSpeed * elapsed;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                origin += Vector3.Forward * TranslateSpeed * elapsed;
            }


            var halfViewWidth = 10 * zoom;
            effect.Projection = Matrix.CreateOrthographicOffCenter(origin.X - halfViewWidth, origin.X + halfViewWidth, origin.Z - halfViewWidth, origin.Z + halfViewWidth, 0.01f, 100.0f);
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
