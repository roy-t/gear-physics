using System;
using Microsoft.Xna.Framework;

namespace GearSim.Shapes
{
    public struct Polar2 : IEquatable<Polar2>
    {
        /// <summary>
        /// Construct a polar coordinate
        /// </summary>
        /// <param name="r">radial distance from a hypothetical reference point</param>
        /// <param name="a">Angle (in degrees) from the reference point</param>
        public Polar2(float r, float a)
        {
            this.R = r;
            this.A = a;
        }

        /// <summary>
        /// Radial distance from the reference point
        /// </summary>
        public float R { get; set; }

        /// <summary>
        /// Angle (in degrees) from the reference point
        /// </summary>
        public float A { get; set; }

        public static Polar2 LinearToPolar(Vector2 position) => LinearToPolar(position.X, position.Y);

        public static Polar2 LinearToPolar(float x, float y)
        {
            var r = (float)Math.Sqrt((x * x) + (y * y));

            var a = MathHelper.ToDegrees((float)Math.Asin(y / r));

            if (x < 0)
            {
                a = 180 - a;
            }

            a = (a + 360) % 360;
            return new Polar2(r, a);
        }

        public Vector2 ToLinear()
        {
            var r = this.R;
            var a = this.A;

            a = MathHelper.ToRadians((a + 360) % 360);
            var x = (float)Math.Cos(a) * r;
            var y = -(float)Math.Sin(a) * r;
            return new Vector2(x, y);
        }

        public static Polar2 operator +(Polar2 value1, Polar2 value2)
        {
            value1.R += value2.R;
            value1.A += value2.A;
            return value1;
        }

        public static Polar2 operator -(Polar2 value1, Polar2 value2)
        {
            value1.R -= value2.R;
            value1.A -= value2.A;
            return value1;
        }

        public static Polar2 operator *(Polar2 value1, Polar2 value2)
        {
            value1.R *= value2.R;
            value1.A *= value2.A;
            return value1;
        }

        public static Polar2 operator *(Polar2 value, float scaleFactor)
        {
            value.R *= scaleFactor;
            value.A *= scaleFactor;
            return value;
        }

        public static Polar2 operator *(float scaleFactor, Polar2 value)
        {
            value.R *= scaleFactor;
            value.A *= scaleFactor;
            return value;
        }

        public static Polar2 operator /(Polar2 value1, Polar2 value2)
        {
            value1.R /= value2.R;
            value1.A /= value2.A;
            return value1;
        }

        public static Polar2 operator /(Polar2 value1, float divider)
        {
            float num = 1f / divider;
            value1.R *= num;
            value1.A *= num;
            return value1;
        }

        public static bool operator ==(Polar2 value1, Polar2 value2)
        {
            if (value1.R == value2.R)
            {
                return value1.A == value2.A;
            }
            return false;
        }

        public static bool operator !=(Polar2 value1, Polar2 value2)
        {
            if (value1.R == value2.R)
            {
                return value1.A != value2.A;
            }
            return true;
        }


        public override bool Equals(object obj)
        {
            if (obj is Polar2)
            {
                return Equals((Polar2)obj);
            }
            return false;
        }

        public bool Equals(Polar2 other)
        {
            if (R == other.R)
            {
                return A == other.A;
            }
            return false;
        }

        public override int GetHashCode() => (this.R.GetHashCode() * 397) ^ this.A.GetHashCode();

        public override string ToString() => $"R: {this.R}, A: {this.A}°";
    }
}
