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
        Stopwatch stopwatch = new Stopwatch();
        private long currentTime = 0;
        private long lastTime = 0;
        private float timer = 0;
        private int fps = 1;
        private int frames;
        private float deltaTime = 0.005f;
        public float reloading = 60f;
        private float reload = 0f;
        private float rotSpeed = 1f;
        private float rotVolume = 0f;
        private Color loadColor = Color.GREEN;

        Sound snd_drive;
        Sound snd_rotate;
        Sound snd_fire;

        SceneObject tankObject = new SceneObject();
        SceneObject turretObject = new SceneObject();

        List<Bullet> bullets = new List<Bullet>();
        SpriteObject tankSprite = new SpriteObject();
        SpriteObject turretSprite = new SpriteObject();

        Hitbox tankHit = new Hitbox(20f, 20f);

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

            Texture2D barrel1 = LoadTexture("Resources/tankBlue_barrel1_outline.png");
            Texture2D barrel2 = LoadTexture("Resources/tankBlue_barrel2_outline.png");
            Texture2D barrel3 = LoadTexture("Resources/tankBlue_barrel3_outline.png");
            Texture2D bullet1 = LoadTexture("Resources/bulletBlue1_outline.png");
            Texture2D bullet2 = LoadTexture("Resources/bulletBlue2_outline.png");
            Texture2D bullet3 = LoadTexture("Resources/bulletBlue3_outline.png");

            Barrel barrelAvg = new Barrel(barrel3, 0.5f, 75, -8f);
            Barrel barrelFst = new Barrel(barrel2, 1, 60, -8f);
            Barrel barrelHvy = new Barrel(barrel1, 1, 90, -8f);

            barrels.Add(barrelAvg);
            barrels.Add(barrelFst);
            barrels.Add(barrelHvy);

            Ammo ammoAvg = new Ammo(bullet1,1f,600);
            Ammo ammoFst = new Ammo(bullet3,0.75f,700);
            Ammo ammoHvy = new Ammo(bullet2,1.25f,500);

            ammos.Add(ammoAvg);
            ammos.Add(ammoFst);
            ammos.Add(ammoHvy);

            barrel = barrelAvg;
            ammo = ammoFst;

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

            bullet.AddChild(bulspr);

            turretObject.AddChild(turretSprite);
            turretSprite.AddChild(shotFX);
            tankSprite.AddChild(tankHit);
            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);
            tankObject.SetPosition(GetScreenWidth() / 2f, GetScreenHeight() / 2f);

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

            turretSprite.Load(barrel.Tex);
            turretSprite.SetPosition(barrel.Offset, turretSprite.Width / 2.0f);
            bulspr.Load(ammo.Tex);
            bulspr.SetPosition(0, -bulspr.Width / 2.0f);
            shotFX.SetPosition(turretSprite.Width / 2.0f, turretSprite.Height);
            shotSprite.SetPosition(-(shotSprite.Width / 2.0f), 0);

            reloading = barrel.ReloadTime;

            if (reload < reloading) { reload++; loadColor = Color.LIME; }
            else { reload = reloading; loadColor = Color.GREEN; }

            //DEBUG
            if (IsKeyDown(KeyboardKey.KEY_ONE))
            {
                barrel = barrels[0];
            }
            if (IsKeyDown(KeyboardKey.KEY_TWO))
            {
                barrel = barrels[1];
            }
            if (IsKeyDown(KeyboardKey.KEY_THREE))
            {
                barrel = barrels[2];
            }
            if (IsKeyDown(KeyboardKey.KEY_FOUR))
            {
                ammo = ammos[0];
            }
            if (IsKeyDown(KeyboardKey.KEY_FIVE))
            {
                ammo = ammos[1];
            }
            if (IsKeyDown(KeyboardKey.KEY_SIX))
            {
                ammo = ammos[2];
            }
            //tank control
            if (IsKeyDown(KeyboardKey.KEY_A))
            {
                tankObject.Rotate(-deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                tankObject.Rotate(deltaTime);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * 100;
                tankObject.Translate(facing.x, facing.y);
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                Vector3 facing = new Vector3(
                    tankObject.LocalTransform.m1,
                    tankObject.LocalTransform.m2, 1) * deltaTime * -100;
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
                PlaySound(snd_fire);
            }

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
            tankObject.Update(deltaTime);

            if (bullet.TimeLeft > 0)
            { bullet.Update(deltaTime); }
            else
            {
                bullet.Face(turretObject);
            }


            lastTime = currentTime;
        }
        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);
            DrawText(fps.ToString(), 10, 10, 12, Color.LIME);
            DrawText($"{tankObject.GlobalTransform.m1},{tankObject.GlobalTransform.m2}\n{tankObject.GlobalTransform.m4},{tankObject.GlobalTransform.m5}\n{tankObject.GlobalTransform.m7},{tankObject.GlobalTransform.m8}", 10, 20, 12, Color.LIME);
            DrawText($"{shotFX.TimeLeft}", 10, 80, 12, Color.LIME);

            tankObject.Draw();
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
}
