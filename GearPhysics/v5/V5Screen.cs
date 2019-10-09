using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GearPhysics.v5
{
    // TODO:
    // - Make it possible to define a lot of gears in one go that are next form a long chaing to see if everything keeps working
    // - Investigate inner gears
    public sealed class V5Screen : IScreen
    {
        private readonly GraphicsDevice Device;

        private readonly List<Gear> Gears;
        

        public V5Screen(GraphicsDevice device)
        {
            this.Device = device;

            this.Gears = new List<Gear>();

            var parent = new Gear(device, Vector2.UnitX * 0.0f, 10);

            var childTeeth = 53;

            // Evil that we need to create an entire gear just to get that magic Rsc value
            // Refactor into a gear description and an actual gear so that we have everything we need beforehand
            var tmp = new Gear(device, Vector2.Zero, childTeeth);

            var childRsc = tmp.Rsc;

            var dist = parent.Rsc + childRsc;
            var jointAngleDeg = 20.0f;
            var offsetX = (float)Math.Cos(MathHelper.ToRadians(jointAngleDeg)) * dist;
            var offsetY = -(float)Math.Sin(MathHelper.ToRadians(jointAngleDeg)) * dist;

            var position = parent.Position + new Vector2(offsetX, offsetY);
            var child = new Gear(device, position, childTeeth);
            child.JointAngleDeg = jointAngleDeg;


            this.Gears.Add(parent);
            this.Gears.Add(child);

            UpdateGears(0.0f);
        }      

        private void CreateDriveTrain(params int[] teeth)
        {
        }


        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            var spin = (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver4;

            if (!keyboard.IsKeyDown(Keys.Space))
            {
                UpdateGears(spin);
            }
        }

        private void UpdateGears(float spin)
        {
            // TODO: make sure we update every next one
            // and also make sure that we update N+1 with the speed of N, not with the speed of the first gear as in v4!!
            var parent = this.Gears[0];
            parent.Spin(spin);

            var child = this.Gears[1];

            var parentRotation = MathHelper.ToDegrees(parent.Rotation);

            var gearRatio = parent.N / (float)child.N;
            var jointAngleDeg = child.JointAngleDeg;
            var childRotation = 180 - ((parentRotation + jointAngleDeg) * gearRatio) - jointAngleDeg;

            child.SetRotation(MathHelper.ToRadians(childRotation));
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
