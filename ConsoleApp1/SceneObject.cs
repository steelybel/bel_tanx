﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Raylib;
using static Raylib.Raylib;

namespace ConsoleApp1
{
    public class SceneObject
    {
        protected SceneObject parent = null;
        protected List<SceneObject> children = new List<SceneObject>();
        protected Matrix3 localTransform = new Matrix3();
        protected Matrix3 globalTransform = new Matrix3();
        public float health = 0f;
        public bool passThru = false;
        private bool obstacle = false;
        public bool hitboxDisplay = false;

        public Matrix3 LocalTransform
        {
            get { return localTransform; }
        }
        public Matrix3 GlobalTransform
        {
            get { return globalTransform; }
        }
        public SceneObject Parent
        {
            get { return parent; }
        }
        public List<SceneObject> Children
        {
            get { return children; }
        }

        public SceneObject(float hp, bool pass)
        {
            obstacle = false;
            health = hp; passThru = pass;
        }

        public Vector3 position
        {
            get { return new Vector3(globalTransform.m7, globalTransform.m8, globalTransform.m9); }
        }

        Vector3 minVal = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Vector3 maxVal = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public Vector3 MinVal { get { return minVal; } }
        public Vector3 MaxVal { get { return maxVal; } }
        Rectangle fakeBox = new Rectangle(-16, 16, 32, 32);
        Color hitboxColor = Color.GREEN;
        public AABB blankHitBox = new AABB(Vector3.Zero,Vector3.Zero);
        public List<Vector3> myPoints = new List<Vector3>();
        public void Vector3MinMax()
        {
            minVal = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            maxVal = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            for (int i = 0; i < myPoints.Count; i++)
            {
                if (myPoints[i].x + globalTransform.m7 > maxVal.x)
                    maxVal.x = myPoints[i].x + globalTransform.m7;
                if (myPoints[i].y + globalTransform.m8 > maxVal.y)
                    maxVal.y = myPoints[i].y + globalTransform.m8;
                if (myPoints[i].x + globalTransform.m7 < minVal.x)
                    minVal.x = myPoints[i].x + globalTransform.m7;
                if (myPoints[i].y + globalTransform.m8 < minVal.y)
                    minVal.y = myPoints[i].y + globalTransform.m8;
            }
        }
        public void HitBox()
        {
            blankHitBox.Fit(myPoints);
            blankHitBox.min += position;
            blankHitBox.max += position;
        }
        public SceneObject()
        {

        }
        public void Damage(float damage)
        {
            health -= damage;
        }
        public int GetChildCount() { return children.Count; }

        public SceneObject GetChild(int index) { return children[index]; }
        public SpriteObject GetSprite(int index) { return (SpriteObject)children[index]; }
        public void AddChild (SceneObject child)
        {
            //Debug.Assert(child.parent == null);
            if (child.parent != null ) { child.parent.RemoveChild(child); }
            child.parent = this;
            children.Add(child);
        }
        public void RemoveChild (SceneObject child)
        {
            if (children.Remove(child) == true)
            {
                child.parent = null;
            }
        }
        public void UpdateTransform()
        {
            if (parent != null)
            {
                globalTransform = parent.globalTransform * localTransform;
            }
            else
            {
                globalTransform = localTransform;
            }
            foreach (SceneObject child in children) { child.UpdateTransform(); }
        }
        public void SetPosition (float x, float y)
        {
            localTransform.SetTranslation(x, y);
            UpdateTransform();
        }
        public void SetRotate (float radians)
        {
            localTransform.SetRotateZ(radians);
            UpdateTransform();
        }
        public void SetScale(float width, float height)
        {
            localTransform.SetScaled(width, height, 1);
            UpdateTransform();
        }
        public void Translate(float x, float y)
        {
            localTransform.Translate(x, y);
            UpdateTransform();
        }
        public void Rotate(float radians)
        {
            localTransform.RotateZ(radians);
            UpdateTransform();
        }
        public void Scale(float width, float height)
        {
            localTransform.Scale(width, height, 1);
            UpdateTransform();
        }
        public virtual void OnUpdate(float deltaTime)
        {
        }
        public virtual void OnDraw()
        {

        }
        public void Update (float deltaTime)
        {
            OnUpdate(deltaTime);
            foreach (SceneObject child in children)
            {
                child.Update(deltaTime);
            }
        }
        public void Draw()
        {
            OnDraw();
            if (obstacle && health <= 0) return;
            foreach(SceneObject child in children)
            {
                child.Draw();
            }
            //hitbox
            HitBox();
            Vector3MinMax();
            
            if (myPoints.Count > 0 && hitboxDisplay)
            {
                DrawLine((int)minVal.x, (int)minVal.y, (int)minVal.x, (int)maxVal.y, hitboxColor);
                DrawLine((int)minVal.x, (int)maxVal.y, (int)maxVal.x, (int)maxVal.y, hitboxColor);
                DrawLine((int)maxVal.x, (int)maxVal.y, (int)maxVal.x, (int)minVal.y, hitboxColor);
                DrawLine((int)maxVal.x, (int)minVal.y, (int)minVal.x, (int)minVal.y, hitboxColor);
            }
        }
        ~SceneObject()
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
            foreach (SceneObject so in children)
            {
                so.parent = null;
            }
        }
    }

    public class Bullet : SceneObject
    {
        private int lifetime = 59;
        private int timeLeft = 0;
        private float bulletSpeed = 600f;
        public int TimeLeft { get { return timeLeft; } }
        public bool Explod { get { return explode; } }
        private bool explode = true;
        public bool pass = false;
        public Effect explosion = new Effect();
        public Bullet()
        {

        }
        public override void OnUpdate(float deltaTime)
        {
            Vector3 facing = new Vector3(localTransform.m1, localTransform.m2, 1) * deltaTime * bulletSpeed;
            if (timeLeft > 0)
            {
                timeLeft--;
                Translate(facing.x, facing.y);
            }
            else
            {
                timeLeft = 0;
            }
        }
        public void Face(SceneObject turret)
        {
            localTransform.Set(turret.GlobalTransform);
            Vector3 facing = new Vector3(localTransform.m1, localTransform.m2, 1) * 50;
            Translate(facing.x, facing.y);
        }
        public void Reset()
        {
            timeLeft = lifetime;
            explode = false;
            pass = false;
        }
        public void Destroy()
        {
            timeLeft = 0;
        }
        public void SetStats(Barrel barrel)
        {
            bulletSpeed = barrel.Speed;
            lifetime = barrel.LifeTime;
        }
        public void Explode(Effect spawn)
        {
            timeLeft = 0;
            spawn.SetPosition(GlobalTransform.m7, GlobalTransform.m8);
            spawn.Activate();
            explode = true;
        }
    }
    public class Effect : SceneObject
    {
        private int lifetime = 10;
        private int timeLeft = 0;
        public int TimeLeft { get { return timeLeft; } }
        public int LifeTime { get { return lifetime; } }
        public override void OnUpdate(float deltaTime)
        {
            if (timeLeft > 0) timeLeft--;
        }
        public void Activate()
        {
            timeLeft = lifetime;
        }
        public void Deactivate()
        {
            timeLeft = 0;
        }
        public void ChangeLifetime(int lifetime)
        {
            this.lifetime = lifetime;
        }
    }
}
