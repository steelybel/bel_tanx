using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib;
using rl = Raylib.Raylib;

namespace ConsoleApp1
{
    public struct Matrix3
    {
        public float m1, m2, m3, m4, m5, m6, m7, m8, m9;
        public Matrix3(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            m1 = xAxis.x; m2 = xAxis.y; m3 = xAxis.z;
            m4 = yAxis.x; m5 = yAxis.y; m6 = zAxis.z;
            m7 = zAxis.x; m8 = zAxis.y; m9 = zAxis.z;
        }
        public Matrix3(float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9)
        {
            this.m1 = m1; this.m2 = m2; this.m3 = m3;
            this.m4 = m4; this.m5 = m5; this.m6 = m6;
            this.m7 = m7; this.m8 = m8; this.m9 = m9;
        }
        public static Matrix3 identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        public static Matrix3 operator *(Matrix3 rhs, Matrix3 lhs)
        {
            return new Matrix3(
                lhs.m1 * rhs.m1 + lhs.m2 * rhs.m4 + lhs.m3 * rhs.m7,
                lhs.m1 * rhs.m2 + lhs.m2 * rhs.m5 + lhs.m3 * rhs.m8,
                lhs.m1 * rhs.m3 + lhs.m2 * rhs.m6 + lhs.m3 * rhs.m9,

                lhs.m4 * rhs.m1 + lhs.m5 * rhs.m4 + lhs.m6 * rhs.m7,
                lhs.m4 * rhs.m2 + lhs.m5 * rhs.m5 + lhs.m6 * rhs.m8,
                lhs.m4 * rhs.m3 + lhs.m5 * rhs.m6 + lhs.m6 * rhs.m9,

                lhs.m7 * rhs.m1 + lhs.m8 * rhs.m4 + lhs.m9 * rhs.m7,
                lhs.m7 * rhs.m2 + lhs.m8 * rhs.m5 + lhs.m9 * rhs.m8,
                lhs.m7 * rhs.m3 + lhs.m8 * rhs.m6 + lhs.m9 * rhs.m9
                );
        }
        public static Vector3 operator *(Matrix3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.m1 * rhs.x + lhs.m4 * rhs.y + lhs.m7 * rhs.z,
                lhs.m2 * rhs.x + lhs.m5 * rhs.y + lhs.m8 * rhs.z,
                lhs.m3 * rhs.x + lhs.m6 * rhs.y + lhs.m9 * rhs.z
                );
        }
        Matrix3 GetTransposed()
        {
            return new Matrix3(
                m1, m4, m7,
                m2, m5, m8,
                m3, m6, m9);
        }
        public void Set (Matrix3 set)
        {
            this = set;
        }
        public void Set (float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9)
        {
            this.m1 = m1; this.m2 = m2; this.m3 = m3;
            this.m4 = m4; this.m5 = m5; this.m6 = m6;
            this.m7 = m7; this.m8 = m8; this.m9 = m9;
        }
        public void SetScaled(float x, float y, float z)
        {
            m1 = x; m2 = 0; m3 = 0;
            m4 = 0; m5 = y; m6 = 0;
            m7 = 0; m8 = 0; m9 = z;
        }
        public void SetScaled(Vector3 v)
        {
            m1 = v.x; m2 = 0; m3 = 0;
            m4 = 0; m5 = v.y; m6 = 0;
            m7 = 0; m8 = 0; m9 = v.z;
        }
        void Scale(float x, float y, float z)
        {
            Matrix3 m = new Matrix3();
            m.SetScaled(x, y, z);
            Set(this * m);
        }
        public void SetRotateX (double radians)
        {
            Set(1, 0, 0,
                0, (float)Math.Cos(radians), (float)Math.Sin(radians),
                0, (float)-Math.Sin(radians), (float)Math.Cos(radians));
        }
        public void SetRotateY(double radians)
        {
            Set((float)Math.Cos(radians), 0, (float)-Math.Sin(radians),
                0, 1, 0,
                (float)Math.Sin(radians), 0, (float)Math.Cos(radians));
        }
        public void SetRotateZ(double radians)
        {
            Set((float)Math.Cos(radians), (float)Math.Sin(radians), 0,
                (float)-Math.Sin(radians), (float)Math.Cos(radians), 0,
                0, 0, 1);
        }
        public void RotateX(double radians)
        {
            Matrix3 m = new Matrix3();
            m.SetRotateX(radians);
            Set(this * m);
        }
        public void RotateY(double radians)
        {
            Matrix3 m = new Matrix3();
            m.SetRotateY(radians);
            Set(this * m);
        }
        public void RotateZ(double radians)
        {
            Matrix3 m = new Matrix3();
            m.SetRotateZ(radians);
            Set(this * m);
        }
    }
}