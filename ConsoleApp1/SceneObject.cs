using System;
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
        public SceneObject()
        {

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
            foreach(SceneObject child in children)
            {
                child.Draw();
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
        int bulletSpeed = 600;
        public int TimeLeft { get { return timeLeft; } }
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
            else timeLeft = 0;
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
        }
    }
    public class Effect : SceneObject
    {
        private int lifetime = 10;
        private int timeLeft = 0;
        public int TimeLeft { get { return timeLeft; } }
        public override void OnUpdate(float deltaTime)
        {
            if (timeLeft > 0) timeLeft--;
        }
        public override void OnDraw()
        {
            if (timeLeft <= 0) return;
        }
        public void Activate()
        {
            timeLeft = lifetime;
        }
    }
}
