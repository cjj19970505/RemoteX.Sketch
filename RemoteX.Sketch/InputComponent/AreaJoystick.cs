using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public class AreaJoystick : Joystick
    {
        public event EventHandler<AreaStatusChangeEventArgs> OnAreaStatusChanged;
        protected List<Area> AreaList { get; }
        public float MaxLength = 300;
        public Area this[string areaName]
        {
            get
            {
                foreach (var area in AreaList)
                {
                    if (area.AreaName == areaName)
                    {
                        return area;
                    }
                }
                return null;
            }
        }

        
        public void AddArea(Area area)
        {
            area.Status = AreaStatus.Released;
            AreaList.Add(area);
        }
        public AreaJoystick() : base()
        {
            AreaList = new List<Area>();
        }

        protected override void OnDeltaChanged()
        {
            base.OnDeltaChanged();
            var areaList = AreaList;
            foreach(var area in areaList)
            {
                if(area.IsInArea(this, Delta))
                {
                    var oldStatus = area.Status;
                    if(oldStatus != AreaStatus.Pressed)
                    {
                        //AreaStatusDict[area] = AreaStatus.Pressed;
                        area.Status = AreaStatus.Pressed;
                        OnAreaStatusChanged?.Invoke(this, new AreaStatusChangeEventArgs(area, oldStatus, AreaStatus.Pressed));
                    }
                }
                else
                {
                    var oldStatus = area.Status;
                    if (oldStatus != AreaStatus.Released)
                    {
                        //AreaStatusDict[area] = AreaStatus.Released;
                        area.Status = AreaStatus.Released;
                        OnAreaStatusChanged?.Invoke(this, new AreaStatusChangeEventArgs(area, oldStatus, AreaStatus.Released));
                    }
                }
            }
        }

        public enum AreaStatus { Released = 0, Pressed = 1 }
        public class Area
        {
            public float StartRadian { get; }
            public float EndRadian { get; }
            public float StartLength { get; }
            public float EndLength { get; }
            public string AreaName { get; set; }
            public AreaStatus Status { get; internal set; }

            public float StandardizedStartRadian
            {
                get
                {
                    return Standardize(StartRadian);
                }
            }

            public float StandardizedEndRadian
            {
                get
                {
                    return Standardize(EndRadian);
                }
            }
            public float StandardizeEndRadian { get; }
            public Area(string areaName, float startRadian, float endRadian, float startLength, float endLength)
            {
                AreaName = areaName;
                StartRadian = startRadian;
                EndRadian = endRadian;
                StartLength = startLength;
                EndLength = endLength;
            }

            public bool IsInArea(AreaJoystick areaJoystick, Vector2 delta)
            {
                if (delta.X == 0 && delta.Y == 0)
                {
                    return false;
                }
                var deltaRadian = (float)Math.Atan2(delta.Y, delta.X);
                var startToEndRange = EndRadian - StartRadian;
                if ((deltaRadian > StandardizedEndRadian - startToEndRange && deltaRadian < StandardizedEndRadian) || (deltaRadian < StandardizedStartRadian + startToEndRange && deltaRadian> StandardizedStartRadian))
                {

                }
                else
                {
                    return false;
                }
                if (delta.Length() > EndLength * areaJoystick.MaxLength || delta.Length() < StartLength * areaJoystick.MaxLength)
                {
                    return false;
                }
                return true;
            }

            private float Standardize(float radian)
            {
                var standarizeRadian = radian % (2 * Math.PI);
                if (standarizeRadian < -Math.PI)
                {
                    standarizeRadian += (2 * Math.PI);
                }
                else if (standarizeRadian > Math.PI)
                {
                    standarizeRadian -= (2 * Math.PI);
                }
                return (float)standarizeRadian;
            }

            public static Area CreateFromAngle(string areaName, float startAngle, float endAngle, float startLength, float endLength)
            {
                return new Area(areaName, (float)(startAngle * Math.PI / 180), (float)(endAngle * Math.PI / 180), startLength, endLength);
            }
        }

        public class AreaStatusChangeEventArgs:EventArgs
        {
            public Area Area { get; }
            public AreaStatus OldStatus { get; }
            public AreaStatus NewStatus { get; }
            public AreaStatusChangeEventArgs(Area area, AreaStatus oldStatus, AreaStatus newStatus) : base()
            {
                Area = area;
                OldStatus = oldStatus;
                NewStatus = newStatus;
            }
        }
    }


}
