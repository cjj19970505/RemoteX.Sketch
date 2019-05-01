﻿using System;

namespace RemoteX.GraphicMath
{
    public struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }
        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }
        public static Vector2 operator *(Vector2 lhs, float rhs)
        {
            return new Vector2(lhs.x * rhs, lhs.y * rhs);
        }
        public static Vector2 operator *(float lhs, Vector2 rhs)
        {
            return new Vector2(lhs * rhs.x, lhs * rhs.y);
        }
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.x == rhs.x) && (lhs.y == rhs.y);
        }
        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.x != rhs.x) || (lhs.y != rhs.y);
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }

    
}
