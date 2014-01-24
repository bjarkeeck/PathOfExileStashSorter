using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    /// <summary>
    /// Representation of 2D vectors and points.
    /// </summary>
    [Serializable]
    public struct Vector2
    {
        public float X, Y;
     

        /// <summary>
        /// Returns the length of this vector (Read Only)
        /// </summary>
        public float magnitude
        {
            get { return Mathf.Sqrt(sqrMagnitude); }
        }

        /// <summary>
        /// Returns the squared length of this vector (Read Only)
        /// </summary>
        public float sqrMagnitude
        {
            get { return X * X + Y * Y; }
        }

        /// <summary>
        /// Returns this vector with a magnitude of 1 (Read Only)
        /// </summary>
        public Vector2 normalized
        {
            get
            {
                if (magnitude == 0)
                    return this;
                return new Vector2(X / magnitude, Y / magnitude);
            }
        }

        /// <summary>
        /// Shorthand for writing Vector2(1, 1);
        /// </summary>
        public static Vector2 one { get { return new Vector2(1, 1); } }

        /// <summary>
        /// Shorthand for writing Vector2(0, 0);
        /// </summary>
        public static Vector2 zero { get { return new Vector2(0, 0); } }

        /// <summary>
        /// Shorthand for writing Vector2(1, 0);
        /// </summary>
        public static Vector2 right { get { return new Vector2(1, 0); } }

        /// <summary>
        /// Shorthand for writing Vector2(-1, 0);
        /// </summary>
        public static Vector2 left { get { return new Vector2(-1, 0); } }

        /// <summary>
        /// Shorthand for writing Vector2(0, 1);
        /// </summary>
        public static Vector2 down { get { return new Vector2(0, 1); } }

        /// <summary>
        /// Shorthand for writing Vector2(0, -1);
        /// </summary>
        public static Vector2 up { get { return new Vector2(0, -1); } }

        /// <summary>
        /// Initializes a new instance of the Vector2 struct with the specified components
        /// </summary>
        /// <param name="x">x component of the vector</param>
        /// <param name="y">y component of the vector</param>
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }


        // Operators
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj is Vector2 == false)
                return false;
            return X == ((Vector2)obj).X && Y == ((Vector2)obj).Y;
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (X + "-" + Y).GetHashCode();
        }

        public static Vector2 operator *(Vector2 v1, float a)
        {
            return new Vector2(v1.X * a, v1.Y * a);
        }
        public static Vector2 operator *(float a, Vector2 v1)
        {
            return v1 * a;
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static Vector2 operator /(Vector2 v1, float a)
        {
            return new Vector2(v1.X / a, v1.Y / a);
        }
        public static Vector2 operator /(float a, Vector2 v1)
        {
            return v1 / a;
        }

        /// <summary>
        /// Returns a (not so) nicely formatted string for this vector
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0};{1}", X, Y);
        }

        /// <summary>
        /// Gets the average of the given Vector2's.
        /// </summary>
        /// <returns></returns>
        public static Vector2 Average(params Vector2[] vectors)
        {
            Vector2 result = Vector2.zero;

            foreach (var v in vectors)
                result += v;

            return result / vectors.Length;
        }

        /// <summary>
        /// Returns the angle in degress between two vectors
        /// </summary>
        public static float Angle(Vector2 v1, Vector2 v2)
        {
            return Mathf.Atan2(v2.Y - v1.Y, v2.X - v1.X);
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to maxLength
        /// </summary>
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            if (vector.magnitude > maxLength)
                return vector.normalized * maxLength;
            else
                return vector;
        }

        /// <summary>
        /// Gets a vector 1 unit long, pointing in the given direction.
        /// </summary>
        /// <param name="degree">The degree to point in.</param>
        public static Vector2 DirectionVector(float degree)
        {
            return new Vector2(Mathf.Round(Mathf.Cos(degree * Mathf.Deg2Rad), 2), Mathf.Round(Mathf.Sin(degree * Mathf.Deg2Rad), 2));
        }

        /// <summary>
        /// Returns the distance between two vectors
        /// </summary>
        public static float Distance(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude;
        }
        /// <summary>
        /// Returns the lowest distance between point p and a line defined by the endpoints v and w.
        /// Source: http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        /// Adapted from C++
        /// </summary>
        public static float Distance(Vector2 p, Vector2 v, Vector2 w)
        {
            float l2 = (w - v).sqrMagnitude;  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return Vector2.Distance(p, v);   // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            float t = Vector2.Dot(p - v, w - v) / l2;
            if (t < 0.0) return Vector2.Distance(p, v);       // Beyond the 'v' end of the segment
            else if (t > 1.0) return Vector2.Distance(p, w);  // Beyond the 'w' end of the segment
            Vector2 projection = v + t * (w - v);  // Projection falls on the segment
            return Vector2.Distance(p, projection);
        }

        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y);
        }

        /// <summary>
        /// Linearly interpolates between two vectors by amount of t
        /// </summary>
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
        {
            if (t >= 1)
                return to;
            else if (t <= 0)
                return from;
            else
                return from + (to - from) * t;
        }

        /// <summary>
        /// Returns a vector that is made from the largest components of two vectors
        /// </summary>
        public static Vector2 Max(Vector2 v1, Vector2 v2)
        {
            return new Vector2(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));
        }

        /// <summary>
        /// Returns a vector that is made from the smallest components of two vectors
        /// </summary>
        public static Vector2 Min(Vector2 v1, Vector2 v2)
        {
            return new Vector2(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));
        }

        /// <summary>
        /// Multiplies two vectors component-wise
        /// </summary>
        public static Vector2 Scale(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        /// <summary>
        /// Reflects the specified vector off of the normal vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="normal">The normal.</param>
        /// <returns>The reflected vector.</returns>
        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            return vector - 2 * Vector2.Dot(vector, normal) * normal;
        }

        /// <summary>
        /// Tries to parse out a Vector2 from the given string.
        /// </summary>
        public static bool TryParse(string str, out Vector2 result)
        {
            result = Vector2.zero;
            string[] split = str.Split(';');

            if (split.Length != 2)
                return false;

            float x, y;

            if (!float.TryParse(split[0], out x))
                return false;
            if (!float.TryParse(split[1], out y))
                return false;

            result = new Vector2(x, y);

            return true;
        }
}
