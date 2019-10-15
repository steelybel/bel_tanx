using System;
using System.Collections.Generic;
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

        private float rotSpeed = 1f;

        SceneObject tankObject = new SceneObject();
        SceneObject turretObject = new SceneObject();

        List<Bullet> bullets = new List<Bullet>();
        SpriteObject tankSprite = new SpriteObject();
        SpriteObject turretSprite = new SpriteObject();

        public void Init()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            tankSprite.Load("Resources/tankBody_blue_outline.png");
            tankSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            tankSprite.SetPosition(-tankSprite.Width / 2.0f, tankSprite.Height / 2.0f);

            turretSprite.Load("Resources/tankBlue_barrel1_outline.png");
            turretSprite.SetRotate(-90 * (float)(Math.PI / 180.0f));
            turretSprite.SetPosition(-10, turretSprite.Width / 2.0f);

            turretObject.AddChild(turretSprite);
            tankObject.AddChild(tankSprite);
            tankObject.AddChild(turretObject);
            tankObject.SetPosition(GetScreenWidth() / 2f, GetScreenHeight() / 2f);
        }
        public void Shutdown()
        { }
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

            foreach (Bullet shot in bullets)
            {
                if (shot.TimeLeft <= 0)
                {
                    //bullets.Remove(shot);
                }
                shot.Update(deltaTime);
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
            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                Bullet bul = new Bullet();
                SpriteObject bulspr = new SpriteObject();
                bulspr.Load("Resources/bulletBlue1_outline.png");
                bul.AddChild(bulspr);
                bullets.Insert(0,bul);
                bullets[0].Face(tankObject, turretObject);
            }
            tankObject.Update(deltaTime);

            lastTime = currentTime;
        }
        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.WHITE);
            DrawText(fps.ToString(), 10, 10, 12, Color.LIME);
            DrawText($"{tankObject.GlobalTransform.m1},{tankObject.GlobalTransform.m2}\n{tankObject.GlobalTransform.m4},{tankObject.GlobalTransform.m5}\n{tankObject.GlobalTransform.m7},{tankObject.GlobalTransform.m8}", 10, 20, 12, Color.LIME);
            DrawText(fps.ToString(), 10, 10, 12, Color.LIME);

            tankObject.Draw();
            foreach (Bullet bul in bullets)
            {
                bul.Draw();
            }

            EndDrawing();
        }
    }

}
