using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch
{
    public interface IArea
    {
        bool IsOverlapPoint(Vector2 point);
    }

    public struct CircleArea : IArea
    {
        public float Radius { get; set; }
        public Vector2 Position { get; set; }

        public bool IsOverlapPoint(Vector2 point)
        {
            if ((point - Position).Length() < Radius)
            {
                return true;
            }
            return false;
        }
    }

    public struct RectArea : IArea
    {
        public (Vector2 Min, Vector2 Max) Rect { get; set; }

        public RectArea((Vector2 Min, Vector2 Max) rect)
        {
            Rect = rect;
        }

        public bool IsOverlapPoint(Vector2 point)
        {
            if (point.X <= Rect.Max.X && point.X >= Rect.Min.X && point.Y <= Rect.Max.Y && point.Y >= Rect.Min.Y)
            {
                return true;
            }
            return false;
        }
    }

    public struct AllArea : IArea
    {
        public bool IsOverlapPoint(Vector2 point)
        {
            return true;
        }
    }
}
