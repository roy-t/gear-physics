using System;
using System.Collections.Generic;
using System.Linq;
using GearSim.Objects;
using GearSim.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearSim
{
    public sealed class GameLoop : Game
    {
        private const float DiametralPitch = 25.0f;
        private const float PressureAngleDeg = 20.0f;

        private static Random Random = new Random();

        private readonly GraphicsDeviceManager Graphics;
        private readonly List<Gear> Gears;

        private BasicEffect effect;
        private KeyboardState lastState;

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

            var teethValues = this.GenerateNumbers(4, 2);
            var angleValues = this.GenerateNumbers(0, 25);
            var tuples = teethValues.Zip(angleValues, (a, b) => (a, (float)b)).Take(40);

            this.CreateDriveTrain(tuples.ToArray());

            base.LoadContent();
        }

        private IEnumerable<int> GenerateNumbers(int start, int increase)
        {
            for(var i = 0; i < int.MaxValue; i++ )
            {
                yield return start + (i * increase);
            }
        }

        private void CreateDriveTrain(params (int teeth, float jointAngle)[] values)
        {
            Gear previousGear = null;

            for (var i = 0; i < values.Length; i ++)
            {
                var (teeth, jointAngle) = values[i];

                var shape = new InvoluteGearShape(teeth, DiametralPitch, PressureAngleDeg);
                var gear = new Gear(shape, this.GetRandomNamedColor());
                this.Gears.Add(gear);

                if (previousGear != null)
                {
                    previousGear.AddChild(gear, MathHelper.ToRadians(jointAngle));
                }

                previousGear = gear;
            }
        }

        private Color GetRandomNamedColor()
        {
            var colorType = typeof(Color);
            var properties = colorType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var colors = properties
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(colorType, null))
                .ToArray();

            return colors[Random.Next(colors.Length)];
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            var rootGear = this.Gears[0];

            //if (keyboardState.IsKeyDown(Keys.Space))
            {                
                rootGear.Rotation += elapsed * 1.5f;                
            }

            rootGear.Update();

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
            this.GraphicsDevice.Clear(Color.Black);

            for(var i = 0; i < this.Gears.Count; i++)
            {
                this.Gears[i].Draw(this.GraphicsDevice, this.effect);
            }

            base.Draw(gameTime);
        }
    }
}
