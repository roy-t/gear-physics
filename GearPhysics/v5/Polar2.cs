using System;

namespace GearPhysics.v5
{
    public struct Polar2 : IEquatable<Polar2>
    {
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
        /// Angle from the reference point
        /// </summary>
        public float A { get; set; }

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

        public override int GetHashCode()
        {
            return R.GetHashCode() * 397 ^ A.GetHashCode();
        }
    }
}
