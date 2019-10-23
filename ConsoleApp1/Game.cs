using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Raylib;
using static Raylib.Raylib;

namespace ConsoleApp1
{
    class Game
    {
        public bool paused = false;
        Stopwatch stopwatch = new Stopwatch();
        private long currentTime = 0;
        private long lastTime = 0;
        private float timer = 0;
        private int fps = 1;
        private int frames;
        private float deltaTime = 0.005f;
        public float reloading = 60f;
        private float reload = 0f;
        public float maxArmor = 100f;
        private float armor = 100f;
        private float rotSpeed = 1f;
        private float rotVolume = 0f;
        private Color loadColor = Color.GREEN;

        bool soundOn = false;


        Sound snd_drive;
        Sound snd_rotate;
        Sound snd_fire;

        SceneObject tankObject = new SceneObject();
        SceneObject turretObject = new SceneObject();

        List<Bullet> bullets = new List<Bullet>();
        SpriteObject tankSprite = new SpriteObject();
        SpriteObject turretSprite = new SpriteObject();
        Effect explosionFX = new Effect();
        AnimObject expSprite = new AnimObject();

        Bullet bullet = new Bullet();
        SpriteObject bulspr = new SpriteObject();

        Effect shotFX = new Effect();
        SpriteObject shotSprite = new SpriteObject();

        //EQUIPMENT

        //MODULAR STUFF
        Body body = new Body();
        Barrel barrel = new Barrel();
        Ammo ammo = new Ammo();

        List<Body> bodies = new List<Body>();
        List<Barrel> barrels = new List<Barrel>();
        List<Ammo> ammos = new List<Ammo>();

        Texture2D[] expSpr = new Texture2D[5];

        //OBJECT POOL
        //ObjectPool<Bullet> objPool = new ObjectPool<Bullet>();
        //ObjectPool<SpriteObject> sprPool = new ObjectPool<SpriteObject>();
        public void Init()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            InitAudioDevice();
            snd_drive = LoadSound("Resources/Sound/drive.ogg");
            snd_rotate = LoadSound("Resources/Sound/rotate.ogg");
            snd_fire = LoadSound("Resources/Sound/fire.ogg");


            Texture2D body1 = LoadTexture("Resources/tankBody_blue_outline.png");
            Texture2D body2 = LoadTexture("Resources/tankBody_bigRed_outline.png");
            Texture2D body3 = LoadTexture("Resources/tankBody_huge_outline.png");
            Texture2D barrel1 = LoadTexture("Resources/tankBlue_barrel1_outline.png");
            Texture2D barrel2 = LoadTexture("Resources/tankBlue_barrel2_outline.png");
            Texture2D barrel3 = LoadTexture("Resources/tankBlue_barrel3_outline.png");
            Texture2D s_barrel1 = LoadTexture("Resources/specialBarrel1_outline.png");
            Texture2D s_barrel2 = LoadTexture("Resources/specialBarrel2_outline.png");
            Texture2D s_barrel3 = LoadTexture("Resources/specialBarrel3_outline.png");
            Texture2D s_barrel4 = LoadTexture("Resources/specialBarrel4_outline.png");
            Texture2D s_barrel5 = LoadTexture("Resources/specialBarrel5_outline.png");
            Texture2D s_barrel6 = LoadTexture("Resources/specialBarrel6_outline.png");
            Texture2D s_barrel7 = LoadTexture("Resources/specialBarrel7_outline.png");
            Texture2D bullet1 = LoadTexture("Resources/bulletBlue1_outline.png");
            Texture2D bullet2 = LoadTexture("Resources/bulletBlue2_outline.png");
            Texture2D bullet3 = LoadTexture("Resources/bulletBlue3_outline.png");
            Texture2D bulletM = LoadTexture("Resources/bulletDark1.png");

            Body bodyBasic = new Body(body1, 100f, 1f, 100f, 0f);
            Body bodyBig = new Body(body2, 50f, 2f, 80f, 16f);
            Body bodyBigger = new Body(body3, 75f, 0.5f, 120f, -16f);

            bodies.Add(bodyBasic);
            bodies.Add(bodyBig);
            bodies.Add(bodyBigger);

            Ammo ammoAvg = new Ammo(bullet1, 1f, 600);
            Ammo ammoFst = new Ammo(bullet3, 0.75f, 700);
            Ammo ammoHvy = new Ammo(bullet2, 1.25f, 500);

            Barrel barrelAvg = new Barrel(barrel2, bullet1, 1f, 600, 59, 75, -8f, 0);
            Barrel barrelPrc = new Barrel(barrel3, bullet3, 0.75f, 700, 59, 60, -8f, 0);
            Barrel barrelHwz = new Barrel(barrel1, bullet2, 1.25f, 500, 59, 90, -8f, 0); //HOWITZER
            Barrel barrelTmm = new Barrel(s_barrel4, bulletM, 0.5f, 900, 29, 30, -16f, -4f); //20mm machine gun

            barrels.Add(barrelAvg);
            barrels.Add(barrelPrc);
            barrels.Add(barrelHwz);
            barrels.Add(barrelTmm);

            body = bodyBasic;
            barrel = barrelAvg;

            expSpr = new Texture2D[5]
            {
                LoadTexture("Resources/explosion1.png"),
                LoadTexture("Resources/explosion2.png"),
                LoadTexture("Resources/explosion3.png"),
                LoadTexture("Resources/explosion4.png"),
                LoadTexture("Resources/explosion5.png"),
            };
            expSprite.Load(expSpr);

            tankSprite.Load("Resources/tankBody_blue_outline.png");
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            turretSprite.Load("Resources/tankBlue_barrel1_outline.png");
            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            turretSprite.SetPosition(-10, turretSprite.Width / 2.0f);

            bulspr.Load("Resources/bulletBlue1_outline.png");
            bulspr.SetRotate(90 * (float)(Math.PI / 180.0f));
            bulspr.SetPosition(0, -bulspr.Width / 2.0f);

            shotSprite.Load("Resources/shotLarge.png");
            shotFX.AddChild(shotSprite);
            shotFX.SetPosition(turretSprite.Width / 2.0f, turretSprite.Height);
            shotSprite.SetPosition( - (shotSprite.Width / 2.0f), 0);

            explosionFX.AddChild(expSprite);
            explosionFX.ChangeLifetime(10);

            bullet.explosion = explosionFX;
            bullet.AddChild(bulspr);

            turretObject.AddChild(turretSprite);
            turretSprite.AddChild(shotFX);
            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);
            tankObject.SetPosition(GetScreenWidth() / 4f, (GetScreenHeight() / 4f) * 3);

        }
        public void Shutdown()
        {
            UnloadSound(snd_drive);
            UnloadSound(snd_rotate);
            UnloadSound(snd_fire);
            CloseAudioDevice();
        }
        public void Update()
        {
            currentTime = stopwatch.ElapsedMilliseconds;
            deltaTime = (currentTime - lastTime) / 1000.0f;

            timer += deltaTime;
            if (timer >= 1)
            {
                fps = frames;
                frames = 0;
                timer -= 1;
            }
            frames++;

            if (IsKeyPressed(KeyboardKey.KEY_P))
            {
                paused = true;
            }

            if (IsKeyPressed(KeyboardKey.KEY_M))
            {
                soundOn = !soundOn;
            }
            tankSprite.Load(body.Tex);
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);
            turretObject.SetPosition(body.BarrelPlace, 0);
            turretSprite.Load(barrel.Tex);
            turretSprite.SetPosition(barrel.Offset.x, (turretSprite.Width / 2.0f) + barrel.Offset.y);
            bulspr.Load(barrel.BulTex);
            bulspr.SetPosition(0, -bulspr.Width / 2.0f);
            shotFX.SetPosition((turretSprite.Width / 2.0f) + barrel.Offset.y, turretSprite.Height);
            shotSprite.SetPosition(-(shotSprite.Width / 2.0f), 0);

            reloading = barrel.ReloadTime;

            if (reload < reloading) { reload++; loadColor = Color.LIME; }
            else { reload = reloading; loadColor = Color.GREEN; }


            if (explosionFX.TimeLeft <= 0)
            {
                expSprite.Reset();
            }
            explosionFX.Update(deltaTime);
            expSprite.SetPosition(-expSprite.Width / 2.0f, -expSprite.Height / 2.0f);
            tankObject.Update(deltaTime);
            bullet.SetStats(barrel);
            if (bullet.TimeLeft > 0)
            {
                bullet.Update(deltaTime);
            }
            else
            {
                if (!bullet.Explod)
                {
                    bullet.Explode(explosionFX);
                }
                else
                {
                    bullet.Face(turretObject);
                }
            }
            

            //DEBUG
            if (IsKeyDown(KeyboardKey.KEY_KP_1))
            {
                barrel = barrels[0];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_2))
            {
                barrel = barrels[1];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_3))
            {
                barrel = barrels[2];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_4))
            {
                barrel = barrels[3];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_5))
            {
                barrel = barrels[2];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_6))
            {
                barrel = barrels[2];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_7))
            {
                barrel = barrels[2];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_8))
            {
                body = bodies[1];
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_9))
            {
                body = bodies[2];
            }
            //tank control
            if (IsKeyDown(KeyboardKey.KEY_A))
            {
                tankObject.Rotate(-deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                tankObject.Rotate(deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * body.MoveSpeed;
                tankObject.Translate(facing.x, facing.y);
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * -body.MoveSpeed;
                tankObject.Translate(facing.x, facing.y);
            }
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                turretObject.Rotate(-deltaTime * rotSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                turretObject.Rotate(deltaTime * rotSpeed);
            }

            if (IsKeyPressed(KeyboardKey.KEY_SPACE) && reload >= reloading && bullet.TimeLeft <= 0)
            {
                bullet.Reset();
                reload = 0;
                shotFX.Activate();
                if (soundOn) PlaySound(snd_fire);
            }

            if(soundOn)
            {
                if ((IsKeyDown(KeyboardKey.KEY_W) || IsKeyDown(KeyboardKey.KEY_S)))
                {
                    if (IsSoundPlaying(snd_drive))
                    {
                        SetSoundVolume(snd_drive, 1.0f);
                    }
                    else
                    {
                        PlaySound(snd_drive);
                    }
                }
                else
                {
                    if (!IsSoundPlaying(snd_drive))
                    {
                        PlaySound(snd_drive);

                    }
                    SetSoundVolume(snd_drive, 0.2f);
                }

                if ((IsKeyDown(KeyboardKey.KEY_Q) || IsKeyDown(KeyboardKey.KEY_E)))
                {
                    if (IsSoundPlaying(snd_rotate))
                    {
                        SetSoundVolume(snd_rotate, 1.0f);
                    }
                    else
                    {
                        PlaySound(snd_rotate);
                    }
                }
                else
                {
                    if (IsSoundPlaying(snd_rotate))
                    {
                        SetSoundVolume(snd_rotate, 0.0f);
                    }
                }
            }


            lastTime = currentTime;
        }
        public void Paused()
        {
            currentTime = stopwatch.ElapsedMilliseconds;
            deltaTime = (currentTime - lastTime) / 1000.0f;

            timer += deltaTime;
            if (timer >= 1)
            {
                fps = frames;
                frames = 0;
                timer -= 1;
            }
            frames++;

            if (IsKeyPressed(KeyboardKey.KEY_P))
            {
                paused = false;
            }

            lastTime = currentTime;
        }
        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.BEIGE);
            DrawText(fps.ToString(), 10, 10, 12, Color.LIME);
            DrawText($"{tankObject.GlobalTransform.m1},{tankObject.GlobalTransform.m2}\n{tankObject.GlobalTransform.m4},{tankObject.GlobalTransform.m5}\n{tankObject.GlobalTransform.m7},{tankObject.GlobalTransform.m8}", 10, 20, 12, Color.LIME);
            DrawText($"{shotFX.TimeLeft}", 10, 80, 12, Color.LIME);

            tankObject.Draw();
            explosionFX.Draw();
            if (bullet.TimeLeft > 0)bullet.Draw();
            DrawRectangle(20, GetScreenHeight() - 148, 24, 128, Color.GRAY);
            DrawRectangle(20, GetScreenHeight() - 20 - (int)((reload / reloading) * 128), 24, (int)((reload / reloading) * 128), loadColor);
            EndDrawing();
            
        }
        public float Lerp(float begin, float end)
        {
            return (begin + end) / 2f;
        }
    }
    public class ObjectPool<T> where T : new()
    {
        private readonly ConcurrentBag<T> items = new ConcurrentBag<T>();
        private int counter = 0;
        private int MAX = 10;
        public void Release(T item)
        {
            if (counter < MAX)
            {
                items.Add(item);
                counter++;
            }
        }
        public T Get()
        {
            T item;
            if (items.TryTake(out item))
            {
                counter--;
                return item;
            }
            else
            {
                T obj = new T();
                items.Add(obj);
                counter++;
                return obj;
            }
        }
    }

    public class AABB
    {
        public Vector3 min = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public Vector3 max = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        public bool IsEmpty()
        {
            if (float.IsNegativeInfinity(min.x) &&
                float.IsNegativeInfinity(min.y) &&
                float.IsNegativeInfinity(min.z) &&
                float.IsInfinity(max.x) &&
                float.IsInfinity(max.y) &&
                float.IsInfinity(max.z))
                return true;
            return false;
        }
        public void Empty()
        {
            min = new Vector3(float.NegativeInfinity,
                              float.NegativeInfinity,
                              float.NegativeInfinity);
            max = new Vector3(float.PositiveInfinity,
                              float.PositiveInfinity,
                              float.PositiveInfinity);
        }
        void SetToTransformedBox(AABB box, Matrix3 m)
        {
            if (box.IsEmpty())
            {
                Empty();
                return;
            }
            if (m.m1 > 0.0f) { min.x += m.m1 * box.min.x;  max.x += m.m1 * box.max.x; }
            else { min.x += m.m1 * box.max.x;  max.x += m.m1 * box.min.x; }
            if (m.m2 > 0.0f) { min.x += m.m2 * box.min.x; max.x += m.m2 * box.max.x; }
            else { min.x += m.m2 * box.max.x; max.x += m.m2 * box.min.x; }
            if (m.m3 > 0.0f) { min.x += m.m3 * box.min.x; max.x += m.m3 * box.max.x; }
            else { min.x += m.m3 * box.max.x; max.x += m.m3 * box.min.x; }
            if (m.m4 > 0.0f) { min.x += m.m4 * box.min.x; max.x += m.m4 * box.max.x; }
            else { min.x += m.m4 * box.max.x; max.x += m.m4 * box.min.x; }
            if (m.m5 > 0.0f) { min.x += m.m5 * box.min.x; max.x += m.m5 * box.max.x; }
            else { min.x += m.m5 * box.max.x; max.x += m.m5 * box.min.x; }
            if (m.m6 > 0.0f) { min.x += m.m6 * box.min.x; max.x += m.m6 * box.max.x; }
            else { min.x += m.m6 * box.max.x; max.x += m.m6 * box.min.x; }
            if (m.m7 > 0.0f) { min.x += m.m7 * box.min.x; max.x += m.m7 * box.max.x; }
            else { min.x += m.m7 * box.max.x; max.x += m.m7 * box.min.x; }
            if (m.m8 > 0.0f) { min.x += m.m8 * box.min.x; max.x += m.m8 * box.max.x; }
            else { min.x += m.m8 * box.max.x; max.x += m.m8 * box.min.x; }
            if (m.m9 > 0.0f) { min.x += m.m9 * box.min.x; max.x += m.m9 * box.max.x; }
            else { min.x += m.m9 * box.max.x; max.x += m.m9 * box.min.x; }
        }
        public AABB()
        {

        }
        public AABB(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }
        public Vector3 Center()
        {
            return (min + max) * 0.5f;
        }
        public Vector3 Extents()
        {
            return new Vector3(Math.Abs(max.x - min.x) * 0.5f, Math.Abs(max.y - min.y) * 0.5f, Math.Abs(max.z - min.z) * 0.5f);
        }
        public List<Vector3> Corners()
        {
            List<Vector3> corners = new List<Vector3>(4);
            corners[0] = min;
            corners[1] = new Vector3(min.x, max.y, min.z);
            corners[2] = max;
            corners[3] = new Vector3(max.x, min.y, min.z);
            return corners;
        }
        public void Fit(List<Vector3> points)
        {
            min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            foreach (Vector3 p in points)
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        }
        public void Fit(Vector3[] points)
        {
            min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            foreach (Vector3 p in points)
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        }
        public bool Overlaps(Vector3 p)
        {
            return !(p.x < min.x || p.y < min.y || p.x > max.x || p.y > max.y);
        }
        public bool Overlaps(AABB other)
        {
            return !(max.x < other.min.x || max.y < other.min.y || min.x > other.max.x || min.y > other.max.y);
        }
        public Vector3 ClosestPoint(Vector3 p)
        {
            return Vector3.Clamp(p, min, max);
        }
    }
}
