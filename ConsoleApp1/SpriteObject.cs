using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Raylib;
using static Raylib.Raylib;

namespace ConsoleApp1
{
    public class SpriteObject : SceneObject
    {
        Texture2D texture = new Texture2D();
        //Image image = new Image();
        public float Width { get { return texture.width; } }
        public float Height { get { return texture.height; } }
        public SpriteObject()
        {

        }
        public void Load(string filename)
        {
            Image img = LoadImage(filename);
            texture = LoadTextureFromImage(img);
        }
        public void Load(Image file)
        {
            texture = LoadTextureFromImage(file);
        }
        public void Load(Texture2D file)
        {
            texture = file;
        }
        public override void OnDraw()
        {
            float rotation = (float)Math.Atan2(
                globalTransform.m2, globalTransform.m1);
            if(Parent is Effect e)
            {
                if (e.TimeLeft <= 0) return;
            }
            Raylib.Raylib.DrawTextureEx(
                texture,
                new Vector2(globalTransform.m7, globalTransform.m8),
                rotation * (float)(180.0f / Math.PI),
                1, Color.WHITE);
        }
    }
    public class Body
    {
        float moveSpeed = 100f;
        float turnSpeed = 1f;
        float armor = 100f;
    }

    public class Ammo
    {
        Texture2D tex;
        float damageMult;
        float speed;
        public Texture2D Tex { get { return tex; } }
        public float DamageMult { get { return damageMult; } }
        public float Speed { get { return speed; } }
        public Ammo()
        {
            damageMult = 1;
            speed = 60;
        }
        public Ammo(Texture2D tex, float damageMult, float speed)
        {
            this.tex = tex;
            this.damageMult = damageMult;
            this.speed = speed;
        }
    }

    public class Barrel
    {
        Texture2D tex;
        float damageMult;
        float reloadTime;
        public Texture2D Tex {get { return tex; } }
        public float DamageMult {get { return damageMult; } }
        public float ReloadTime {get { return reloadTime; } }
        public Barrel()
        {
            damageMult = 1;
            reloadTime = 60;
        }
        public Barrel(Texture2D tex, float damageMult, float reloadTime)
        {
            this.tex = tex;
            this.damageMult = damageMult;
            this.reloadTime = reloadTime;
        }
    }
    
}
