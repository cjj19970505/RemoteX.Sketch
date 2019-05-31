using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.Skia
{
    public static class Utility
    {
        public static SKPoint ToSKPoint(this Vector2 self)
        {
            return new SKPoint(self.X, self.Y);
        }

        /// <summary>
        /// Not tested
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static (Vector2 Min, Vector2 Max) Transform(this Matrix3x2 self, (Vector2 Min, Vector2 Max) rect)
        {
            Vector2 p1 = Vector2.Transform(rect.Min, self);
            Vector2 p2 = Vector2.Transform(rect.Max, self);
            if(p1.X <= p2.X && p1.Y <= p2.Y)
            {
                return (p1, p2);
            }
            else if(p1.X >= p2.X && p1.Y>=p2.Y)
            {
                return (p2, p1);
            }
            else
            {
                Vector2 p3 = new Vector2(p1.X, p2.Y);
                Vector2 p4 = new Vector2(p2.X, p1.Y);
                if(p3.X<=p4.X&& p3.Y<=p4.Y)
                {
                    return (p3, p4);
                }
                else
                {
                    return (p4, p3);
                }
            }
        }

        /// <summary>
        /// 通过任意对角点返回一个矩形
        /// </summary>
        /// <param name="diaPoints"></param>
        /// <returns></returns>
        public static (Vector2 Min, Vector2 Max) ToRect(this (Vector2 p1, Vector2 p2) diaPoints)
        {
            Vector2 p1 = diaPoints.p1;
            Vector2 p2 = diaPoints.p2;
            if (p1.X <= p2.X && p1.Y <= p2.Y)
            {
                return (p1, p2);
            }
            else if (p1.X >= p2.X && p1.Y >= p2.Y)
            {
                return (p2, p1);
            }
            else
            {
                Vector2 p3 = new Vector2(p1.X, p2.Y);
                Vector2 p4 = new Vector2(p2.X, p1.Y);
                if (p3.X <= p4.X && p3.Y <= p4.Y)
                {
                    return (p3, p4);
                }
                else
                {
                    return (p4, p3);
                }
            }
        }

        public static SKRect ToSKRect(this (Vector2 Min, Vector2 Max) self)
        {
            return new SKRect(self.Min.X, self.Min.Y, self.Max.X, self.Max.Y);
        }
    }
}
