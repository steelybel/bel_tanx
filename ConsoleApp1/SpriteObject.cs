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
    public class AnimObject : SceneObject
    {
        Texture2D[] textures;
        int frame = 0;
        float framespeed = 1f;
        float current = 0f;
        //Image image = new Image();
        public float Width { get { return textures[frame].width; } }
        public float Height { get { return textures[frame].height; } }
        public AnimObject()
        {

        }
        public void Load(Texture2D[] files)
        {
            textures = files;
        }
        public override void OnUpdate(float deltaTime)
        {
            if (Parent is Effect e)
            {
                if (e.TimeLeft > 0)
                {
                    float fParTime = (float)e.LifeTime - (float)e.TimeLeft;
                    float fParTime_ = (float)e.LifeTime;
                    float fMyTime = (float)frame;
                    frame = (int)((fParTime / fParTime_) * (textures.Length - 1));
                }
                else
                {
                    frame = 0;
                }
            }
            else
            {
                if (current < framespeed)
                {
                    current += deltaTime;
                }
                else
                {
                    current = 0;

                    if (frame < textures.Length) { frame += 1; }
                    else frame = 0;
                }
            }

        }
        
        public override void OnDraw()
        {
            float rotation = (float)Math.Atan2(
                globalTransform.m2, globalTransform.m1);
            if (Parent is Effect e)
            {
                if (e.TimeLeft <= 0) return;
            }
            DrawTextureEx(
                textures[frame],
                new Vector2(globalTransform.m7, globalTransform.m8),
                rotation * (float)(180.0f / Math.PI),
                1, Color.WHITE);
        }
        public void Reset()
        {
            frame = 0;
        }
    }
    public class Body
    {
        Texture2D tex;
        float moveSpeed = 100f;
        float turnSpeed = 1f;
        float armor = 100f;
        float barrelPlace = 0f;
        public Texture2D Tex { get { return tex; } }
        public float MoveSpeed { get { return moveSpeed; } }
        public float TurnSpeed { get { return turnSpeed; } }
        public float Armor { get { return armor; } }
        public float BarrelPlace { get { return barrelPlace; } }

        public Body()
        {
            moveSpeed = 100f;
            turnSpeed = 1f;
            armor = 100f;
            barrelPlace = 0f;
        }

        public Body(Texture2D texture, float move, float turn, float arm, float barrel)
        {
            tex = texture;
            moveSpeed = move;
            turnSpeed = turn;
            armor = arm;
            barrelPlace = barrel;
        }
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
        Texture2D bulTex;
        float damageMult;
        float speed;
        float reloadTime;
        int lifetime;
        Vector2 offset = new Vector2(-10, 0);
        public Texture2D Tex {get { return tex; } }
        public Texture2D BulTex {get { return bulTex; } }
        public float DamageMult {get { return damageMult; } }
        public float Speed { get { return speed; } }
        public float ReloadTime {get { return reloadTime; } }
        public int LifeTime {get { return lifetime; } }
        public Vector2 Offset {get { return offset; } }
        public Barrel()
        {
            damageMult = 1;
            reloadTime = 60;
        }
        public Barrel(Texture2D tex, Texture2D tex_, float damageMult, float speed, int lifetime, float reloadTime, float offset, float offset_)
        {
            this.tex = tex;
            bulTex = tex_;
            this.damageMult = damageMult;
            this.speed = speed;
            this.reloadTime = reloadTime;
            this.offset.x = offset;
            this.offset.y = offset_;
            this.lifetime = lifetime;
        }
    }
    
}
