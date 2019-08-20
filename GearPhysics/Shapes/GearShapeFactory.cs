using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GearPhysics.Shapes
{
    public sealed class GearShapeFactory
    {
        /// <summary>
        /// Creates a gear shape with the specified radius and number of teeth.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="numberOfTeeth">The number of teeth.</param>
        /// <param name="tipRatio">The tip ratio.</param>
        /// <param name="toothHeight">Height of the tooth.</param>
        /// <returns></returns>
        public static List<Vector2> CreateGear(float radius, int numberOfTeeth, float tipRatio, float toothHeight)
        {
            var vertices = new List<Vector2>();

            var stepSize = MathHelper.TwoPi / numberOfTeeth;
            MathHelper.Clamp(tipRatio, 0f, 1f);
            var toothTipStepSize = (stepSize / 2f) * tipRatio;

            var toothAngleStepSize = (stepSize - (toothTipStepSize * 2f)) / 2f;

            for (var i = numberOfTeeth - 1; i >= 0; --i)
            {
                if (toothTipStepSize > 0f)
                {
                    vertices.Add(
                        new Vector2(radius *
                                    (float)Math.Cos(stepSize * i + toothAngleStepSize * 2f + toothTipStepSize),
                                    -radius *
                                    (float)Math.Sin(stepSize * i + toothAngleStepSize * 2f + toothTipStepSize)));

                    vertices.Add(
                        new Vector2((radius + toothHeight) *
                                    (float)Math.Cos(stepSize * i + toothAngleStepSize + toothTipStepSize),
                                    -(radius + toothHeight) *
                                    (float)Math.Sin(stepSize * i + toothAngleStepSize + toothTipStepSize)));
                }

                vertices.Add(new Vector2((radius + toothHeight) *
                                         (float)Math.Cos(stepSize * i + toothAngleStepSize),
                                         -(radius + toothHeight) *
                                         (float)Math.Sin(stepSize * i + toothAngleStepSize)));

                vertices.Add(new Vector2(radius * (float)Math.Cos(stepSize * i),
                                         -radius * (float)Math.Sin(stepSize * i)));
            }

            return vertices;
        }
    }
}
