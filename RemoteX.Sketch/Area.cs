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
            if((point - Position).Length() < Radius)
            {
                return true;
            }
            return false;
        }
    }
}
