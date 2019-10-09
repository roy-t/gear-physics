using System;
using System.Collections.Generic;
using GearPhysics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GearPhysics.v5
{
    public sealed class Gear
    {
        private readonly List<Line> Lines;
        private readonly GraphicsDevice Device;

        public Gear(GraphicsDevice device, Vector2 position, int n)
            : this(device, position, n, n / 4.0f, 4.0f, 20.0f)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Number of teeth</param>
        /// <param name="d">Pitch diameter</param>
        /// <param name="p">Diametrical pitch</param>
        /// <param name="pa">Pressure angle (in degrees)</param>
        /// <param name="I">internal</param>
        public Gear(GraphicsDevice device, Vector2 position, int n, float d, float p, float pa, bool I = false)
        {
            // Not yet:
            // ca, connection angle
            // ID, id of the gear
            // H hole
            // S spokes
            // SM spokem (?)

            var od = (n + (I ? 2.3f : 2.0f)) / p; // outer diameter
            var rd = (n - (I ? 2 : 2.3f)) / p; // root diameter
            var bc = (float)(d * Math.Cos(pa * Math.PI / 180.0));
            var rmin = rd / 2;
            var rmax = od / 2;
            var rbase = bc / 2;

            var ac = 0.0f;
            var aca = 0.0f;
                        
            var step = 0.1f;
            var first = true;
            var fix = 0;

            var polarPoints = new List<Polar2>();
            polarPoints.Add(new Polar2(rmin, 0));

            // All hard to follow since its in polar coordinates, TODO: describe better!
            for (var i = 1.0f; i < 100.0f; i += step)
            {
                var bpl = this.PolarTolinear(new Polar2(rbase, -i));
                var len = rbase * MathHelper.Pi * 2 / 360.0f * i;
                var opl = this.PolarTolinear(new Polar2(len, -i + 90));
                var np = this.LinearToPolar(new Vector2(bpl.X + opl.X, bpl.Y + opl.Y));

                if (np.R >= rmin)
                {
                    if (first)
                    {
                        first = false;
                        step = (2 / (float)n) * 10;
                    }
                    if (np.R < d / 2)
                    {
                        ac = np.A;
                        aca = i;
                    }
                    if (np.R > rmax)
                    {
                        if (++fix < 10)
                        {
                            i -= step;
                            step /= 2;
                            continue;
                        }

                        np.R = rmax;
                        polarPoints.Add(np);
                        break;
                    }
                    polarPoints.Add(np);
                }
            }

            // Mirror
            var fa = 360 / (float)n;
            var ma = fa / 2 + 2 * ac;
            var fpa = (fa - ma) > 0 ? 0 : -(fa - ma) / 2;
            var m = polarPoints.Count;

            polarPoints[0] = new Polar2(rmin, fpa);

            while (polarPoints[m - 1].A > ma / 2.0f)
            {
                Splice(polarPoints, m - 1, 1);
                m--;
            }

            for (var i = polarPoints.Count - 1; i >= 0; i--)
            {
                var bp = polarPoints[i];
                var na = ma - bp.A;
                polarPoints.Add(new Polar2(bp.R, na));
            }

            m = polarPoints.Count;
            for (var i = 1; i < n; i++)
            {
                for (var pi = 0; pi < m; pi++)
                {
                    var bp = polarPoints[pi];
                    var na = bp.A + fa * i;
                    polarPoints.Add(new Polar2(bp.R, na));
                }
            }


            this.Points = new List<Vector2>();
            for (var i = 0; i < polarPoints.Count; i++)
            {
                var point = this.PolarTolinear(polarPoints[i]);
                var x = this.Fix6(point.X);
                var y = this.Fix6(point.Y);

                this.Points.Add(new Vector2(x, y));
            }

            this.Device = device;
            this.Position = position;
            this.N = n;


            this.Lines = new List<Line>();

            for (var i = 1; i < this.Points.Count; i++)
            {
                var prev = this.Points[i - 1];
                var curr = this.Points[i - 0];

                var line = new Line(device, Color.Yellow, prev, curr);
                line.Position = new Vector3(position.X, 0, position.Y);
                line.Update();
                this.Lines.Add(line);
            }

            var p0 = this.Points[this.Points.Count - 1];
            var p1 = this.Points[0];

            var closingLine = new Line(device, Color.Yellow, p0, p1);
            closingLine.Position = new Vector3(position.X, 0, position.Y);
            closingLine.Update();
            this.Lines.Add(closingLine);
        }

        public List<Vector2> Points { get; }
        public float Rotation { get; private set; }
        public Vector2 Position { get; }
        public int N { get; }

        private Polar2 LinearToPolar(Vector2 c)
        {
            var x = c.X;
            var y = c.Y;
            var r = (float)Math.Sqrt(x * x + y * y);
            var a = MathHelper.ToDegrees((float)Math.Asin(y / r));

            
            if (x < 0)
            {
                a = 180 - a;
            }

            a = (a + 360) % 360;
            return new Polar2(r, a);
        }

        private Vector2 PolarTolinear(Polar2 p)
        {
            var r = p.R;
            var a = p.A;

            a = MathHelper.ToRadians(((a + 360) % 360));
            var x = (float)Math.Cos(a) * r;
            var y = -(float)Math.Sin(a) * r;
            return new Vector2(x, y);
        }

        private float Fix6(float v) => (float)Math.Round(1000.0f * v) / 1000.0f;

        public static List<T> Splice<T>(List<T> source, int index, int count)
        {
            var items = source.GetRange(index, count);
            source.RemoveRange(index, count);
            return items;
        }

        public void Spin(float radians)
        {
            this.Rotation += radians;
            this.Rotation %= MathHelper.TwoPi;
            for (var i = 0; i < this.Lines.Count; i++)
            {
                var line = this.Lines[i];
                line.Angle = this.Rotation;
                line.Update();
            }
        }

        public void Draw(BasicEffect effect)
        {
            for (var i = 0; i < this.Lines.Count; i++)
            {
                var line = this.Lines[i];
                effect.World = line.World;
                effect.CurrentTechnique.Passes[0].Apply();
                line.LineShape.Draw(this.Device);
            }
        }
    }
}
