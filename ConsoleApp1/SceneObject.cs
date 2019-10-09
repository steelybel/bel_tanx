using System;
using System.Collections.Generic;
using System.Text;
using Raylib;
using rl = Raylib.Raylib;

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
        public SceneObject()
        {

        }
        public int GetChildCount() { return children.Count; }

        public SceneObject GetChild(int index) { return children[index]; }
        public void AddChild (SceneObject child)
        {
            //Debug.Assert(child.parent == null);
            child.parent = this;
            children.Add(child);
        }
        public void RemoveChild (SceneObject child)
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
    }
}
