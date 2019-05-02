using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch
{
    public class RectTransform
    {
        public SketchInfo SketchInfo { get; set; }
        public Vector2 AnchoredPosition;
        public Vector2 Pivot;
        public Vector2 AnchorMax;
        public Vector2 AnchorMin;
        public Vector2 OffsetMax;
        public Vector2 OffsetMin;
        
        public RectTransform(SketchInfo sketchInfo)
        {
            SketchInfo = sketchInfo;
        }

        public (Vector2 Min, Vector2 Max) Rect
        {
            get
            {
                bool hasInv = Matrix3x2.Invert(SketchInfo.Sketch.SketchToSketchNormalizedMatrix, out Matrix3x2 sketchNormalizedToSketchMatrix);
                if(!hasInv)
                {
                    throw new NotImplementedException();
                }
                Vector2 denormalizedAnchorMax = Vector2.Transform(AnchorMax, sketchNormalizedToSketchMatrix);
                Vector2 denormalizedAnchorMin = Vector2.Transform(AnchorMin, sketchNormalizedToSketchMatrix);
                Vector2 maxCorner = denormalizedAnchorMax + OffsetMax;
                Vector2 minCorner = denormalizedAnchorMin + OffsetMin;
                return (minCorner, maxCorner);
            }
        }

        
    }
}
