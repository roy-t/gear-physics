using System;
using System.Collections.Generic;
using GearSim.Objects;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearSim
{
    public sealed class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly List<Gear> Gears;

        private BasicEffect effect;
        private KeyboardState lastState;

        private float jointAngle = MathHelper.ToRadians(20);

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.IsMouseVisible = true;

            this.Gears = new List<Gear>();
        }

        protected override void LoadContent()
        {
            this.effect = new BasicEffect(this.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(Vector3.Backward * 10.0f, Vector3.Zero, Vector3.Up)
            };

            var parentShape = new InvoluteGearShape(10, 4.0f, 20);
            var parentGear = new Gear(parentShape, Color.Red);
            this.Gears.Add(parentGear);


            var childShape = new InvoluteGearShape(53, 4.0f, 20);
            var childGear = new Gear(childShape, Color.Purple);


            var distance = (parentShape.PitchDiameter / 2.0f) + (childShape.PitchDiameter / 2.0f);
            

            var offsetX = (float)Math.Cos(jointAngle) * distance;
            var offsetY = (float)Math.Sin(jointAngle) * distance;

            childGear.Position = new Vector3(offsetX, offsetY, 0);
            

            this.Gears.Add(childGear);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }


            var parentGear = this.Gears[0];
            var childGear = this.Gears[1];

            if (keyboardState.IsKeyDown(Keys.Space))
            {                
                parentGear.Rotation += elapsed * 0.5f;
            }

            var gearRatio = parentGear.Teeth / (float)childGear.Teeth;

            var parentRotation = MathHelper.ToDegrees(parentGear.Rotation);
            
            var jointAngleDeg = MathHelper.ToDegrees(jointAngle);
            var childRotation = 180 - ((parentRotation + jointAngleDeg) * gearRatio) - jointAngleDeg;


            childGear.Rotation = MathHelper.ToRadians(childRotation);

            this.lastState = keyboardState;


            this.effect.Projection = Matrix.CreatePerspectiveFieldOfView
            (
                MathHelper.PiOver4,
                this.GraphicsDevice.Viewport.AspectRatio,
                0.01f,
                100.0f
            );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            for(var i = 0; i < this.Gears.Count; i++)
            {
                this.Gears[i].Draw(this.GraphicsDevice, this.effect);
            }

            base.Draw(gameTime);
        }
    }
}
