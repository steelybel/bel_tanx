using System;
using System.Collections.Generic;
using System.Text;
using Raylib;
using static Raylib.Raylib;

namespace ConsoleApp1
{
    class Player
    {
        public int wins = 0;
        public int winStreak = 0;
        public float reloading = 60f;
        public float reload = 0f;
        public Color loadColor = Color.GREEN;
        public float maxArmor = 100f;
        public float armor = 100f;
        //private float rotSpeed = 1f;
        private Body body;
        private List<Body> bodies;
        private Barrel gun;
        private List<Barrel> barrels;
        public int bodyPick = 0;
        public int gunPick = 0;
        SceneObject tankObject = new SceneObject();
        SpriteObject tankSprite = new SpriteObject();
        SceneObject turretObject = new SceneObject();
        SpriteObject turretSprite = new SpriteObject();
        public SceneObject TurretPointer { get { return turretObject; } }
        SceneObject tankTL = new SceneObject();
        SceneObject tankTR = new SceneObject();
        SceneObject tankBL = new SceneObject();
        SceneObject tankBR = new SceneObject();

        public Effect shotFX = new Effect();
        public SpriteObject shotSprite = new SpriteObject();
        public AABB Hitbox
        {
            get { return tankObject.blankHitBox; }
        }
        public Player()
        {

        }
        public Player(List<Body> bodyz, List<Barrel> gunz)
        {
            bodies = bodyz; barrels = gunz;
        }
        public void Fire()
        {
            reload = 0;
            shotFX.Activate();
        }
        void SetBody(int b)
        {
            if (body != bodies[b])
            {
                body = bodies[b];
                maxArmor = bodies[b].Armor;
                tankSprite.Load(bodies[b].Tex);
                tankSprite.SetPosition(-tankSprite.Height / 2, tankSprite.Width / 2);
                turretObject.SetPosition(body.BarrelPlace, 0);
            }
        }
        void SetGun(int b)
        {
            if (gun != barrels[b])
            {
                gun = barrels[b];
                turretSprite.Load(barrels[b].Tex);
                turretSprite.SetPosition(gun.Offset.x, (turretSprite.Width / 2.0f) + gun.Offset.y);
                reloading = gun.ReloadTime;
            }
        }
        public void Reset()
        {
            //bodyPick = 0;
            //gunPick = 0;
            reloading = gun.ReloadTime;
            maxArmor = body.Armor;
            reload = reloading;
            armor = maxArmor;
        }
        public void TakeDamage(float damage)
        {
            armor -= damage;
        }
        public void Init(List<Body> bodys, List<Barrel> barels, int player)
        {
            bodies = bodys; barrels = barels;
            bodyPick = 0;
            gunPick = 0;
            SetBody(0);
            SetGun(0);
            reload = reloading;
            armor = maxArmor;

            if (player == 1) { tankObject.SetRotate(0); }
            if (player == 2) { tankObject.SetRotate(180 * (float)(Math.PI / 180.0f)); }
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            turretSprite.SetPosition(-10, turretSprite.Width / 2.0f);

            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);
            turretObject.AddChild(turretSprite);
            turretObject.AddChild(turretSprite);

            tankObject.AddChild(tankTL);
            tankObject.AddChild(tankTR);
            tankObject.AddChild(tankBL);
            tankObject.AddChild(tankBR);
            tankTL.SetPosition(-tankSprite.Height / 2, -tankSprite.Width / 2);
            tankTR.SetPosition(tankSprite.Height / 2, -tankSprite.Width / 2);
            tankBR.SetPosition(tankSprite.Height / 2, tankSprite.Width / 2);
            tankBL.SetPosition(-tankSprite.Height / 2, tankSprite.Width / 2);

            tankObject.myPoints.Add(tankTL.position);
            tankObject.myPoints.Add(tankTR.position);
            tankObject.myPoints.Add(tankBR.position);
            tankObject.myPoints.Add(tankBL.position);

            turretSprite.AddChild(shotFX);
            shotSprite.Load("Resources/shotLarge.png");
            shotFX.AddChild(shotSprite);
            shotFX.SetPosition(turretSprite.Width / 2.0f, turretSprite.Height);
            shotSprite.SetPosition(-(shotSprite.Width / 2.0f), 0);
        }
        public void Update(float deltaTime)
        {
            SetBody(bodyPick);
            SetGun(gunPick);
            if (reload < reloading) { reload++; loadColor = Color.LIME; }
            else { reload = reloading; loadColor = Color.GREEN; }

            tankObject.Update(deltaTime);

            tankTL.SetPosition(-tankSprite.Height / 2, -tankSprite.Width / 2);
            tankTR.SetPosition(tankSprite.Height / 2, -tankSprite.Width / 2);
            tankBR.SetPosition(tankSprite.Height / 2, tankSprite.Width / 2);
            tankBL.SetPosition(-tankSprite.Height / 2, tankSprite.Width / 2);

            tankObject.myPoints[0] = new Vector3(tankTL.position.x - tankObject.position.x, tankTL.position.y - tankObject.position.y, tankTL.GlobalTransform.m9);
            tankObject.myPoints[1] = new Vector3(tankTR.position.x - tankObject.position.x, tankTR.position.y - tankObject.position.y, tankTR.GlobalTransform.m9);
            tankObject.myPoints[2] = new Vector3(tankBR.position.x - tankObject.position.x, tankBR.position.y - tankObject.position.y, tankBR.GlobalTransform.m9);
            tankObject.myPoints[3] = new Vector3(tankBL.position.x - tankObject.position.x, tankBL.position.y - tankObject.position.y, tankBL.GlobalTransform.m9);

            shotFX.SetPosition((turretSprite.Width / 2.0f) + gun.Offset.y, turretSprite.Height);
            shotSprite.SetPosition(-(shotSprite.Width / 2.0f), 0);
        }
        public void HitboxToggle()
        {
            tankObject.hitboxDisplay = !tankObject.hitboxDisplay;
        }
        public void Draw()
        {
            tankObject.Draw();
        }
        public void Move(float deltaTime, bool forwards)
        {
            if (forwards)
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * body.MoveSpeed;
                tankObject.Translate(facing.x, facing.y);
            }
            if (!forwards)
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * (-body.MoveSpeed);
                tankObject.Translate(facing.x, facing.y);
            }
        }

        public void Move(float deltaTime, float vel)
        {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * vel;
                tankObject.Translate(facing.x, facing.y);
        }
        public void Rotate(float amt)
        {
            tankObject.Rotate(amt);
        }
        public void GunRotate(float amt)
        {
            turretObject.Rotate(amt);
        }
        public void Place(Vector2 position)
        {
            tankObject.SetPosition(position.x, position.y);
        }
        public void SetRotate(float angle)
        {
            tankObject.SetRotate(angle * (float)(Math.PI / 180.0f));
        }
    }
}
