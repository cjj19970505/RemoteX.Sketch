using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.GraphicMath
{
    public struct Vector3
    {
        float x;
        float y;
        float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.x * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);

        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static Vector3 operator *(Vector3 lhs, float rhs)
        {
            return new Vector3(rhs * lhs.x, rhs * lhs.y, rhs * lhs.z);
        }
        public static Vector3 operator *(float lhs, Vector3 rhs)
        {
            return rhs * lhs;
        }

        public static implicit operator Vector2(Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }


    }
}
