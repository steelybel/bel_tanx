using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib;
using rl = Raylib.Raylib;

namespace ConsoleApp1
{
    public struct Matrix4
    {
        public float m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m14, m15, m16;
        public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 wAxis)
        {
            m1 = xAxis.x; m2 = xAxis.y; m3 = xAxis.z; m4 = xAxis.w;
            m5 = yAxis.x; m6 = yAxis.y; m7 = zAxis.z; m8 = xAxis.w;
            m9 = zAxis.x; m10 = zAxis.y; m11 = zAxis.z; m12 = xAxis.w;
            m13 = wAxis.x; m14 = wAxis.y; m15 = wAxis.z; m16 = wAxis.w;
        }
        public Matrix4(float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float m14, float m15, float m16)
        {
            this.m1 = m1;   this.m2 = m2;   this.m3 = m3;   this.m4 = m4;
            this.m5 = m5;   this.m6 = m6;   this.m7 = m7;   this.m8 = m8;
            this.m9 = m9;   this.m10 = m10;   this.m11 = m11;   this.m12 = m12;
            this.m13 = m13;   this.m14 = m14;   this.m15 = m15;   this.m16 = m16;

        }
        public static Matrix4 identity = new Matrix4
           (1, 0, 0, 0, 
            0, 1, 0, 0,
            0, 0, 1, 0, 
            0, 0 ,0, 1);
        public static Matrix4 operator *(Matrix4 rhs, Matrix4 lhs)
        {
            return new Matrix4(
                lhs.m1 * rhs.m1 + lhs.m2 * rhs.m5 + lhs.m3 * rhs.m9 + lhs.m4 * rhs.m13,
                lhs.m1 * rhs.m2 + lhs.m2 * rhs.m6 + lhs.m3 * rhs.m10 + lhs.m4 * rhs.m14,
                lhs.m1 * rhs.m3 + lhs.m2 * rhs.m7 + lhs.m3 * rhs.m11 + lhs.m4 * rhs.m15,
                lhs.m1 * rhs.m4 + lhs.m2 * rhs.m8 + lhs.m3 * rhs.m12 + lhs.m4 * rhs.m16,

                lhs.m5 * rhs.m1 + lhs.m6 * rhs.m5 + lhs.m7 * rhs.m9 + lhs.m8 * rhs.m13,
                lhs.m5 * rhs.m2 + lhs.m6 * rhs.m6 + lhs.m7 * rhs.m10 + lhs.m8 * rhs.m14,
                lhs.m5 * rhs.m3 + lhs.m6 * rhs.m7 + lhs.m7 * rhs.m11 + lhs.m8 * rhs.m15,
                lhs.m5 * rhs.m4 + lhs.m6 * rhs.m8 + lhs.m7 * rhs.m12 + lhs.m8 * rhs.m16,

                lhs.m9 * rhs.m1 + lhs.m10 * rhs.m5 + lhs.m11 * rhs.m9 + lhs.m12 * rhs.m13,
                lhs.m9 * rhs.m2 + lhs.m10 * rhs.m6 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m14,
                lhs.m9 * rhs.m3 + lhs.m10 * rhs.m7 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m15,
                lhs.m9 * rhs.m4 + lhs.m10 * rhs.m8 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m16,

                lhs.m13 * rhs.m1 + lhs.m14 * rhs.m5 + lhs.m15 * rhs.m9 + lhs.m16 * rhs.m13,
                lhs.m13 * rhs.m2 + lhs.m14 * rhs.m6 + lhs.m15 * rhs.m10 + lhs.m16 * rhs.m14,
                lhs.m13 * rhs.m3 + lhs.m14 * rhs.m7 + lhs.m15 * rhs.m11 + lhs.m16 * rhs.m15,
                lhs.m13 * rhs.m4 + lhs.m14 * rhs.m8 + lhs.m15 * rhs.m12 + lhs.m16 * rhs.m16
                );
        }
        public static Vector4 operator *(Matrix4 lhs, Vector4 rhs)
        {
            return new Vector4(
                lhs.m1 * rhs.x + lhs.m5 * rhs.y + lhs.m9 * rhs.z + lhs.m13 * rhs.w,
                lhs.m2 * rhs.x + lhs.m6 * rhs.y + lhs.m10 * rhs.z + lhs.m14 * rhs.w,
                lhs.m3 * rhs.x + lhs.m7 * rhs.y + lhs.m11 * rhs.z + lhs.m15 * rhs.w,
                lhs.m4 * rhs.x + lhs.m8 * rhs.y + lhs.m12 * rhs.z + lhs.m16 * rhs.w
                );
        }
        public void Set(Matrix4 set)
        {
            this = set;
        }
        public void Set(float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float m14, float m15, float m16)
        {
            this.m1 = m1; this.m2 = m2; this.m3 = m3; this.m4 = m4;
            this.m5 = m5; this.m6 = m6; this.m7 = m7; this.m8 = m8;
            this.m9 = m9; this.m10 = m10; this.m11 = m11; this.m12 = m12;
            this.m13 = m13; this.m14 = m14; this.m15 = m15; this.m16 = m16;

        }
        Matrix4 GetTransposed()
        {
            return new Matrix4(
                m1, m5, m9, m13,
                m2, m6, m10, m14,
                m3, m7, m11, m15,
                m4, m8, m12, m16);
        }
        public void SetScaled(float x, float y, float z, float w)
        {
            m1 = x; m2 = 0; m3 = 0; m4 = 0;
            m5 = 0; m6 = y; m7 = 0; m8 = 0;
            m9 = 0; m10 = 0; m11 = z; m12 = 0;
            m13 = 0; m14 = 0; m15 = 0; m16 = w;
        }
        public void SetScaled(Vector4 v)
        {
            m1 = v.x; m2 = 0; m3 = 0; m4 = 0;
            m5 = 0; m6 = v.y; m7 = 0; m8 = 0;
            m9 = 0; m10 = 0; m11 = v.z; m12 = 0;
            m13 = 0; m14 = 0; m15 = 0; m16 = v.w;
        }
        void Scale(float x, float y, float z, float w)
        {
            Matrix4 m = new Matrix4();
            m.SetScaled(x, y, z, w);
            Set(this * m);
        }
        public void SetRotateX(double radians)
        {
            Set(1, 0, 0, 0,
                0, (float)Math.Cos(radians), (float)Math.Sin(radians), 0,
                0, (float)-Math.Sin(radians), (float)Math.Cos(radians), 0,
                0, 0, 0, 1);
        }
        public void SetRotateY(double radians)
        {
            Set((float)Math.Cos(radians), 0, (float)-Math.Sin(radians), 0,
                0, 1, 0, 0,
                (float)Math.Sin(radians), 0, (float)Math.Cos(radians), 0,
                0, 0, 0, 1);
        }
        public void SetRotateZ(double radians)
        {
            Set((float)Math.Cos(radians), (float)Math.Sin(radians), 0, 0,
                (float)-Math.Sin(radians), (float)Math.Cos(radians), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }
        public void RotateX(double radians)
        {
            Matrix4 m = new Matrix4();
            m.SetRotateX(radians);
            Set(this * m);
        }
        public void RotateY(double radians)
        {
            Matrix4 m = new Matrix4();
            m.SetRotateY(radians);
            Set(this * m);
        }
        public void RotateZ(double radians)
        {
            Matrix4 m = new Matrix4();
            m.SetRotateZ(radians);
            Set(this * m);
        }
    }
}
