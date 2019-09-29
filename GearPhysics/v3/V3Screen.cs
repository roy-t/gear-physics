using System.Collections.Generic;
using GearPhysics.v2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v3
{
    public sealed class V3Screen : IScreen
    {
        private readonly GraphicsDevice Device;
        private readonly List<Gear> Gears;

        public V3Screen(GraphicsDevice device)
        {            
            this.Device = device;
            this.Gears = new List<Gear>();

            CreateDriveTrain(1.95f, 0.01f, 30, 20, 10);
            
        }

        private void CreateDriveTrain(float toothHeight, float profileRatio, params int[] teeth)
        {
            // TODO: find the perfect
            var x = 0.0f;

            var toothBaseWidth = 1.0f;// (startDiameter * MathHelper.Pi) / teeth[0];            
            var previousRadius = 0.0f;

            for (var i = 0; i < teeth.Length; i++)
            {
                var t = teeth[i];

                // omtrek = 2 * pi * straal
                // omtrek = 2 * straal * pi
                // omtrek = diameter * pi;
                // omtrek / pi = diameter
                // diameter = omtrek / pi;

                // toothBaseWidth * t = X * pi;
                // tbwt = X * pi
                // tbwt / pi = X;                

                var diameter = (toothBaseWidth * t) / MathHelper.Pi;


                if (previousRadius != 0)
                {
                    x += previousRadius + ((diameter + toothHeight) / 2.0f) - (toothHeight / 3.0f); // 2.55
                }
                previousRadius = (diameter + toothHeight) / 2.0f;

                var description = GearDescription.DescribeGear(diameter + toothHeight, diameter, t, profileRatio);

                var position = new Vector3(x, 0, 0);
                this.Gears.Add(new Gear(this.Device, description, position, i % 2 == 0 ? 1.0f : -1.0f));
            }


            //var teethA = 24;
            //var teethB = 34;

            //var ratio = teethA / (float)teethB;

            //var sizeB = 10;
            //var sizeA = sizeB * ratio;
            //{
            //    new Gear(device, GearDescription.DescribeGear(sizeA, sizeA * 0.8f, teethA, 0.02f), Vector3.Zero, 1.0f),
            //    new Gear(device, GearDescription.DescribeGear(sizeB, sizeB * 0.8f, teethB, 0.02f), Vector3.Right * 8.0f, -1.0f),
            //};

        }

        private float stepSign = 1.0f;
        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            var division = 1;
            for (var f = 0; f < division; f++)
            {
                var speed = MathHelper.PiOver4;
                var step = (float)(speed * gameTime.ElapsedGameTime.TotalSeconds / division);

                var gear = this.Gears[0];
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    stepSign = 1.0f;
                    gear.Spin(step * stepSign);
                }

                if (keyboard.IsKeyDown(Keys.LeftAlt))
                {
                    stepSign = -1.0f;
                    gear.Spin(step * stepSign);
                }

                for (var i = 1; i < this.Gears.Count; i++)
                {
                    var from = this.Gears[i - 1];
                    var to = this.Gears[i - 0];

                    from.Transfer(to, step * stepSign);
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
