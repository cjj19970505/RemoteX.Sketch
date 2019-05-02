using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.GraphicMath
{
    public struct Matrix3x3
    {
        public float a00;
        public float a01;
        public float a02;
        public float a10;
        public float a11;
        public float a12;
        public float a20;
        public float a21;
        public float a22;


        public Matrix3x3(float a00, float a01, float a02, float a10, float a11, float a12, float a20, float a21, float a22)
        {
            this.a00 = a00;
            this.a01 = a01;
            this.a02 = a02;
            this.a10 = a10;
            this.a11 = a11;
            this.a12 = a12;
            this.a20 = a20;
            this.a21 = a21;
            this.a22 = a22;
        }

        public static Matrix3x3 Identity
        {
            get
            {
                return new Matrix3x3(1, 0, 0,
                                    0, 1, 0,
                                    0, 0, 1);

            }
        }

        public static Matrix3x3 Zero
        {
            get
            {
                return new Matrix3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            }
        }

        public float this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex == 0 && columnIndex == 0)
                {
                    return a00;
                }
                else if (rowIndex == 0 && columnIndex == 1)
                {
                    return a01;
                }
                else if (rowIndex == 0 && columnIndex == 2)
                {
                    return a02;
                }
                else if (rowIndex == 1 && columnIndex == 0)
                {
                    return a10;
                }
                else if (rowIndex == 1 && columnIndex == 1)
                {
                    return a11;
                }
                else if (rowIndex == 1 && columnIndex == 2)
                {
                    return a12;
                }
                else if (rowIndex == 2 && columnIndex == 0)
                {
                    return a20;
                }
                else if (rowIndex == 2 && columnIndex == 1)
                {
                    return a21;
                }
                else if (rowIndex == 2 && columnIndex == 2)
                {
                    return a22;
                }
                throw new IndexOutOfRangeException();
            }

            set
            {
                if (rowIndex == 0 && columnIndex == 0)
                {
                    a00 = value;
                }
                else if (rowIndex == 0 && columnIndex == 1)
                {
                    a01 = value;
                }
                else if (rowIndex == 0 && columnIndex == 2)
                {
                    a02 = value;
                }
                else if (rowIndex == 1 && columnIndex == 0)
                {
                    a10 = value;
                }
                else if (rowIndex == 1 && columnIndex == 1)
                {
                    a11 = value;
                }
                else if (rowIndex == 1 && columnIndex == 2)
                {
                    a12 = value;
                }
                else if (rowIndex == 2 && columnIndex == 0)
                {
                    a20 = value;
                }
                else if (rowIndex == 2 && columnIndex == 1)
                {
                    a21 = value;
                }
                else if (rowIndex == 2 && columnIndex == 2)
                {
                    a22 = value;
                }
                throw new IndexOutOfRangeException();

            }
        }

        public Vector2 MultiplyPoint(Vector2 point)
        {
            Vector3 pointInVec3 = new Vector3(point.x, point.y, 1);
            return new Vector3(Vector3.Dot(GetRow(0), pointInVec3), Vector3.Dot(GetRow(1), pointInVec3), Vector3.Dot(GetRow(2), pointInVec3));
        }



        public Vector3 GetColumn(int index)
        {
            return new Vector3(this[index, 0], this[index, 1], this[index, 2]);
        }

        public Vector3 GetRow(int index)
        {
            return new Vector3(this[0, index], this[1, index], this[2, index]);
        }


    }
}
