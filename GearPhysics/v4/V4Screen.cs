using System;
using System.Collections.Generic;
using GearPhysics.v2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v4
{
    public sealed class V4Screen : IScreen
    {
        private readonly GraphicsDevice Device;
        private readonly List<Gear> Gears;

        private bool keepRotating = false;

        public V4Screen(GraphicsDevice device)
        {
            this.Device = device;
            this.Gears = new List<Gear>();
            //{
            //    new Gear(device, 1.0f, 24, Vector3.Zero, 1.0f),
            //    new Gear(device, 1.0f, 18, Vector3.Right * 7.75f, -1.0f),
            //};

            CreateDriveTrain(50, 6);
        }      

        private void CreateDriveTrain(params int[] teeth)
        {
            var toothSideLength = 1.0f;
            var halfToothSideLength = toothSideLength / 2.0f;
            var toothHeight = (float)(halfToothSideLength * Math.Sqrt(3.0f));

            Gear gear = null;
            
            var direction = 1;
            for (var i = 0; i < teeth.Length; i++)
            {
                var x = 0.0f;
                if (gear != null)
                {
                    x = gear.Position.X;

                    // Place the gears so that they don't touch each other
                    x += gear.OutsideDiameter / 2.0f;
                    x += (GearDescription.GetRootDiameter(toothSideLength, teeth[i]) + 2*toothHeight) / 2.0f;

                    // Then place then almost one tooth height closer to each other so that they nicely connect
                    x -= toothHeight * 0.80f;
                }


                gear = new Gear(this.Device, toothSideLength, teeth[i], new Vector3(x, 0, 0), direction);                
                this.Gears.Add(gear);
                

                direction = -direction;
            }
        }

        
        public void Update(GameTime gameTime, KeyboardState keyboard)
        { 
            var driver = this.Gears[0];
            if (keyboard.IsKeyDown(Keys.Space))
            {
                driver.Spin((float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver4);
            }

            if (keyboard.IsKeyDown(Keys.LeftAlt))
            {
                keepRotating = true;
            }
            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                keepRotating = false;
            }


            if (keyboard.IsKeyDown(Keys.LeftShift) || keepRotating)
            {
                var radiansPerSecond = MathHelper.PiOver4 / 10.0f;
                var radiansInStep = (float)gameTime.ElapsedGameTime.TotalSeconds * radiansPerSecond;
                this.Gears[0].Spin(radiansInStep);
                for (var i = 0; i < this.Gears.Count - 1; i++)
                {
                    this.Gears[i].Transfer(radiansInStep, this.Gears[i + 1]);
                }
            }
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
