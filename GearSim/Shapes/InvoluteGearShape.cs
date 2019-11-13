using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GearSim.Shapes
{
    public sealed class InvoluteGearShape
    {
        private const float TwentyDegreesInRadians = 0.3490659f;

        private readonly List<Vector2> OutsidePoints;
        private readonly List<Vector2> InsidePoints;

        /// <summary>
        /// Describes the shape of an involute gear.
        ///                 
        /// Gears should touch at specific point on a their teeth. The pitch circle is the circle that encompasses all these points.
        /// If two gears are interlocking their pitch circles should touch, but not opverlap. The pitch diameter is the diameter of 
        /// the pitch circle. It is defined as teeth / diametral pitch.
        /// 
        /// The diametral pitch defines the size of the teeth. It is defined as the number of teeth per inch or centimeter of the 
        /// pitch diameter. So a larger diametral pitch means smaller tooth, and a smaller gear. 
        /// 
        /// The pressure angle is the angle between the tooth face and gear wheel tangent. It defines the shape of the tooth
        /// In the real world values of 14.5, 20, and 25 degrees are often used. Two interlocking gears should have the same pressure angle.
        /// </summary>
        /// <param name="teeth">Number of teeth, should be at least 5</param>
        /// <param name="diametralPitch">Number of teeth per inch or centimer of pitch diameter, should be more than 0</param>
        /// <param name="axleRadius">Radius of the hole for the axle, for internal gears this becomes the outer radius</param>
        /// <param name="pressureAngle">Pressure angle (specified in radians), should be between 10 and 35 degrees</param>
        /// <param name="gearType">Wether this is an internal gear (teeth on the inside) or an external gear (teeth on the outside)</param>
        public InvoluteGearShape(int teeth, float diametralPitch, float axleRadius, float pressureAngle = TwentyDegreesInRadians, GearType gearType = GearType.External)
        {
            CheckTeeth(teeth);
            CheckDiameteralPitch(diametralPitch);
            CheckPressureAngle(pressureAngle);

            this.OutsidePoints = new List<Vector2>();
            this.InsidePoints = new List<Vector2>();

            var pitchDiameter = teeth / diametralPitch;
            var pitchRadius = pitchDiameter / 2.0f;

            this.Teeth = teeth;
            this.DiametralPitch = diametralPitch;
            this.PitchDiameter = pitchDiameter;
            this.PressureAngle = pressureAngle;
            this.GearType = gearType;

            var radiusMin = RadiusMin(teeth, diametralPitch, gearType);
            var radiusMax = RadiusMax(teeth, diametralPitch, gearType);
            CheckAxleRadius(axleRadius, gearType, radiusMin, radiusMax);

            // Pressure angle projected onto the x-axis
            var rbase = pitchRadius * (float)Math.Cos(pressureAngle);

            // TODO: this is relative rotation the point where the tooth shape intersects with pitch circle, in degrees, how to name it?
            var ac = 0.0f;

            // TODO: check if we can figure out a better way to compute the number of steps the algorithm should run
            var step = 0.1f;
            var first = true;

            var polarPoints = new List<Polar2>
            {
                new Polar2(radiusMin, 0)
            };

            // Create a single side of a single tooth, it is nicely curved
            /*             
             *     \
             *      \ 
             *       |
             *       /
             */

            for (var i = 1.0f; i < 100.0f; i += step)
            {
                // Find the new point
                var basePointLinear = new Polar2(rbase, -i).ToLinear();
                var length = rbase * MathHelper.TwoPi / 360.0f * i;
                var offset = new Polar2(length, -i + 90).ToLinear();
                var pointPolar = Polar2.LinearToPolar(new Vector2(basePointLinear.X + offset.X, basePointLinear.Y + offset.Y));

                if (pointPolar.R >= radiusMin)
                {
                    if (first)
                    {
                        first = false;
                        step = 2.0f / teeth * 10;
                    }

                    if (pointPolar.R < pitchRadius)
                    {
                        ac = pointPolar.A;
                    }

                    if (pointPolar.R > radiusMax)
                    {
                        pointPolar.R = radiusMax;
                        polarPoints.Add(pointPolar);
                        break;
                    }
                    else
                    {
                        polarPoints.Add(pointPolar);
                    }
                }
            }

            /*
             * Remove artefacts at the top
             *    X
             *     \
             *      \ 
             *       |
             *       /
             */
            var degreesPerTooth = 360.0f / teeth;
            var ma = (degreesPerTooth / 2) + (2 * ac);

            var fpa = (degreesPerTooth - ma) > 0 ? 0 : -(degreesPerTooth - ma) / 2;
            polarPoints[0] = new Polar2(radiusMin, fpa);

            var c = polarPoints.Count;
            while (polarPoints[c - 1].A > ma / 2.0f)
            {
                // Remove points with an extreme angle
                polarPoints.RemoveRange(c - 1, 1);
                c--;
            }

            // Mirror the side and connect the two along the top
            /*             
             *    /---\
             *   /     \ 
             *  |       |
             *  \       /
             */
            for (var i = polarPoints.Count - 1; i >= 0; i--)
            {
                var point = polarPoints[i];
                var na = ma - point.A;
                polarPoints.Add(new Polar2(point.R, na));
            }

            // Copy and paste these teeth equal along the entire gear
            // we now have a full gear, in polar coordinates
            c = polarPoints.Count;
            for (var i = 1; i < teeth; i++)
            {
                for (var pi = 0; pi < c; pi++)
                {
                    var bp = polarPoints[pi];
                    var na = bp.A + (degreesPerTooth * i);
                    polarPoints.Add(new Polar2(bp.R, na));
                }
            }

            // Convert back from polar to linear coordinates

            var baseAngleMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(ac));

            var points = gearType == GearType.Internal ? this.InsidePoints : this.OutsidePoints;
            for (var i = 0; i < polarPoints.Count; i++)
            {
                var point = polarPoints[i].ToLinear();

                // Apply a small rotation to every point so the point where a tooth should touch another gear's tooth is at 
                // a rotation of zero degrees
                point = Vector2.Transform(point, baseAngleMatrix);
                points.Add(point);
            }

            if (gearType == GearType.Internal)
            {
                this.OutsidePoints = this.CreateCircle(this.InsidePoints.Count, axleRadius);
            }
            else
            {
                this.InsidePoints = this.CreateCircle(this.OutsidePoints.Count, axleRadius);
            }
        }       

        /// <summary>
        /// Wether this is an internal gear (teeth on the inside) or external gear (teeth on the outside
        /// </summary>
        public GearType GearType { get; }

        /// <summary>
        /// Number of teeth
        /// </summary>
        public int Teeth { get; }

        /// <summary>
        /// Size of the teeth, defined as number of teeth per inch, or centimeter, of the pitch diameter
        /// </summary>
        public float DiametralPitch { get; }

        /// <summary>
        /// Diameter of the pitch circle
        /// </summary>
        public float PitchDiameter { get; }

        /// <summary>
        /// In Degrees
        /// </summary>
        public float PressureAngle { get; }

        public IReadOnlyList<Vector2> Outside => this.OutsidePoints;
        public IReadOnlyList<Vector2> Inside => this.InsidePoints;

        private List<Vector2> CreateCircle(int steps, float radius)
        {
            var points = new List<Vector2>();

            var stepSize = MathHelper.TwoPi / steps;
            for (var i = 0; i < steps; i++)
            {
                var x = (float)Math.Sin(stepSize * i) * radius;
                var y = (float)Math.Cos(stepSize * i) * radius;
                points.Add(new Vector2(x, y));
            }

            return points;
        }

        public static float RadiusMax(int teeth, float diametralPitch, GearType gearType)
        {
            var outerDiameter = (teeth + (gearType == GearType.Internal ? 2.3f : 2.0f)) / diametralPitch;
            return outerDiameter / 2.0f;
        }

        public static float RadiusMin(int teeth, float diametralPitch, GearType gearType)
        {
            var innerDiameter = (teeth - (gearType == GearType.Internal ? 2.0f : 2.3f)) / diametralPitch;
            return innerDiameter / 2.0f;
        }

        private static void CheckAxleRadius(float axleRadius, GearType gearType, float radiusMin, float radiusMax)
        {
            if (axleRadius <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(axleRadius), "Axle radius should be larger than 0");
            }

            if (gearType == GearType.Internal && axleRadius < radiusMax)
            {
                throw new ArgumentOutOfRangeException(nameof(axleRadius), "For internal gears the axle radius should be larger than the outer diameter of the gear");
            }

            if (gearType == GearType.External && axleRadius > radiusMin)
            {
                throw new ArgumentOutOfRangeException(nameof(axleRadius), "For external gears the axle radius should be smaller than the inner diameter of the gear");
            }
        }

        private static void CheckPressureAngle(float pressureAngle)
        {
            if (pressureAngle > MathHelper.ToRadians(35.0f) || pressureAngle < MathHelper.ToRadians(10))
            {
                throw new ArgumentOutOfRangeException(nameof(pressureAngle), "Pressure angle should be between 10 and 35 degrees and is specified in radians");
            }
        }

        private static void CheckDiameteralPitch(float diametralPitch)
        {
            if (diametralPitch <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(diametralPitch), "Diameteral pitch should be larger than zero");
            }
        }

        private static void CheckTeeth(int teeth)
        {
            if (teeth < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(teeth), "A gear should have at least 5 teeth");
            }
        }
    }
}
