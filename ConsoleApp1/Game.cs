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
        public bool paused = true;
        Stopwatch stopwatch = new Stopwatch();
        private long currentTime = 0;
        private long lastTime = 0;
        private float timer = 0;
        private int fps = 1;
        private int frames;
        private float deltaTime = 0.005f;
        private float rotSpeed = 1f;
        bool soundOn = false;


        Sound snd_drive;
        Sound snd_rotate;
        Sound snd_fire;
        Sound snd_load;

        Player p1 = new Player();
        Player p2 = new Player();

        float bulRad = 15.0f;
        float obsRad = 24.0f;
        float obsHealth = 50.0f;

        Effect explosionFX = new Effect();
        AnimObject expSprite = new AnimObject();
        Effect explosionFX_ = new Effect();
        AnimObject expSprite_ = new AnimObject();

        Bullet bullet = new Bullet();
        SpriteObject bulspr = new SpriteObject();

        Bullet bullet_ = new Bullet();
        SpriteObject bulspr_ = new SpriteObject();
        //OBSTACLES

        List<SceneObject> obstacles = new List<SceneObject>();
        Vector3[] obsPositions = new Vector3[9];

        //MODULAR STUFF
        Body body = new Body();
        int bodyPick = 0;
        Barrel barrel = new Barrel();
        int gunPick = 0;

        List<Body> bodies = new List<Body>();
        List<Body> bodies2 = new List<Body>();
        List<Barrel> barrels = new List<Barrel>();

        Texture2D[] expSpr = new Texture2D[5];

        //OBJECT POOL
        //ObjectPool<Bullet> objPool = new ObjectPool<Bullet>();
        //ObjectPool<SpriteObject> sprPool = new ObjectPool<SpriteObject>();
        string helper = $"Press Enter when you are both done.";
        string helper0 = $"Debug keys:\nH - toggle hitbox display\nInsert - Salty reset";
        string helper1 = $"P1:\nW, A, S, D to move\nQ, E to rotate turret\nSpace to fire";
        string helper2 = $"P2 (numpad):\n8, 4, 5, 6 to move\n7, 9 to rotate turret\n0 to fire";
        Rectangle crap = new Rectangle(0, 0, 128, 32);

        public void Init()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            InitAudioDevice();
            snd_drive = LoadSound("Resources/Sound/drive.ogg");
            snd_rotate = LoadSound("Resources/Sound/rotate.ogg");
            snd_fire = LoadSound("Resources/Sound/fire.ogg");
            snd_load = LoadSound("Resources/Sound/reload.ogg");


            for (int p = 0; p < 9; p++)
            {
                float b = (obsPositions.Length / 2);
                float c = p - b;
                obsPositions[p] = new Vector3((GetScreenWidth() / 2) + (p*64) - (b*64), (GetScreenHeight() / 2) + (((float)Math.Pow(c,3)) + (9*c)),0);
            }

            Texture2D body1 = LoadTexture("Resources/tankBody_blue_outline.png");
            Texture2D body1a = LoadTexture("Resources/tankBody_red_outline.png");
            Texture2D body2 = LoadTexture("Resources/tankBody_darkLarge_outline.png");
            Texture2D body3 = LoadTexture("Resources/tankBody_huge_outline.png");
            Texture2D bodyS = LoadTexture("Resources/tankBody_dark.png");
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
            Texture2D rocketL = LoadTexture("Resources/barrelGrey_side.png");
            Texture2D bullet1 = LoadTexture("Resources/bulletDark1_outline.png");
            Texture2D bullet2 = LoadTexture("Resources/bulletDark2_outline.png");
            Texture2D bullet3 = LoadTexture("Resources/bulletDark3_outline.png");
            Texture2D bulletM = LoadTexture("Resources/bulletDark1.png");
            Texture2D rocket1 = LoadTexture("Resources/spaceMissiles_003.png");
            Texture2D rocket2 = LoadTexture("Resources/spaceMissiles_006.png");
            Texture2D obs1 = LoadTexture("Resources/barricadeWood.png");
            Texture2D obs2 = LoadTexture("Resources/sandbagBeige.png");

            Body bodyBasic = new Body(body1, 100f, 1f, 100f, 0f);
            bodyBasic.Rename("Cruiser tank");
            Body bodyBasic2 = new Body(body1a, 100f, 1f, 100f, 0f);
            bodyBasic2.Rename("Cruiser tank");
            Body bodyBig = new Body(body2, 75f, 0.8f, 125f, 8f);
            bodyBig.Rename("Infantry tank");
            Body bodyBigger = new Body(body3, 50f, 0.6f, 150f, -32f);
            bodyBigger.Rename("Super-heavy tank");

            //bodies.Add(bodySmall);
            bodies.Add(bodyBasic);
            bodies.Add(bodyBig);
            bodies.Add(bodyBigger);

            bodies2.Add(bodyBasic2);
            bodies2.Add(bodyBig);
            bodies2.Add(bodyBigger);

            Ammo ammoAvg = new Ammo(bullet1, 1f, 600);
            Ammo ammoFst = new Ammo(bullet3, 0.75f, 700);
            Ammo ammoHvy = new Ammo(bullet2, 1.25f, 500);

            Barrel barrelAvg = new Barrel(s_barrel6, bullet1, 1f, 600, 59, 75, -8f, 0);
            barrelAvg.Rename("105mm cannon");
            barrelAvg.Details(snd_load, 45, false, 0.0f);
            Barrel barrelPrc = new Barrel(s_barrel7, bullet3, 0.75f, 700, 58, 60, -8f, 0);
            barrelPrc.Rename("Self-propelled Sabot");
            barrelPrc.Details(snd_load, 45, false, 0.0f);
            Barrel barrelHwz = new Barrel(s_barrel5, bullet2, 1.25f, 500, 59, 90, -8f, 0);
            barrelHwz.Rename("120mm Howitzer");
            barrelHwz.Details(snd_load, 45, false, 0.0f);
            Barrel barrel45g = new Barrel(s_barrel4, bulletM, 0.5f, 900, 28, 30, -16f, -4f); //20mm machine gun
            barrel45g.Rename("45mm gun");
            barrel45g.Details(snd_load, 45, false, 0.0f);
            Barrel barrelMrl = new Barrel(s_barrel1, rocket1, 2f, 850, 28, 120, -16f, 0);
            barrelMrl.Rename("Rocket launcher");
            barrelMrl.Details(snd_load, 45, false, 0.0f);
            Barrel barrelGui = new Barrel(rocketL, rocket1, 2f, 500, 90, 240, -24f, 0);
            barrelGui.Rename("Guided missile");
            barrelGui.Details(snd_load,45,true, 3.0f);
            Barrel barrelMac = new Barrel(s_barrel4, bulletM, 0.5f, 900, 28, 30, -16f, -4f);

            barrels.Add(barrelAvg);
            barrels.Add(barrelPrc);
            barrels.Add(barrelHwz);
            barrels.Add(barrel45g);
            barrels.Add(barrelGui);

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
            expSprite_.Load(expSpr);

            bulspr.Load("Resources/bulletBlue1_outline.png");
            bulspr.SetRotate(90 * (float)(Math.PI / 180.0f));
            bulspr.SetPosition(90, 0);

            bulspr_.Load("Resources/bulletBlue1_outline.png");
            bulspr_.SetRotate(90 * (float)(Math.PI / 180.0f));
            bulspr_.SetPosition(90, 0);

            explosionFX.AddChild(expSprite);
            explosionFX.ChangeLifetime(10);
            explosionFX_.AddChild(expSprite_);
            explosionFX_.ChangeLifetime(10);

            bullet.explosion = explosionFX;
            bullet_.explosion = explosionFX_;

            bullet.AddChild(bulspr);
            bullet_.AddChild(bulspr_);

            bullet.myPoints.Add(new Vector3(-16, 16, 0));
            bullet.myPoints.Add(new Vector3(16, 16, 0));
            bullet.myPoints.Add(new Vector3(16, -16, 0));
            bullet.myPoints.Add(new Vector3(-16, -16, 0));
            bullet_.myPoints.Add(new Vector3(-16, 16, 0));
            bullet_.myPoints.Add(new Vector3(16, 16, 0));
            bullet_.myPoints.Add(new Vector3(16, -16, 0));
            bullet_.myPoints.Add(new Vector3(-16, -16, 0));

            p1.Init(bodies, barrels, 1);
            p2.Init(bodies2, barrels, 2);

            //
            for (int p = 0; p < obsPositions.Length; p++)
            {
                float half = obsPositions.Length / 2;
                float third = obsPositions.Length / 3;
                SceneObject buh = new SceneObject(obsHealth, false);
                SpriteObject crap = new SpriteObject(); crap.Load(obs1);
                buh.AddChild(crap);
                //buh.Rotate(pos.z * ((float)(Math.PI / 180.0f)));
                buh.SetPosition(obsPositions[p].x, obsPositions[p].y);
                crap.SetPosition(-crap.Width / 2, -crap.Height / 2);
                buh.myPoints.Add(new Vector3(-obsRad, obsRad, 0));
                buh.myPoints.Add(new Vector3(obsRad, obsRad, 0));
                buh.myPoints.Add(new Vector3(obsRad, -obsRad, 0));
                buh.myPoints.Add(new Vector3(-obsRad, -obsRad, 0));
                obstacles.Add(buh);
                if (p < (int)third-1 || p > (int)(third*2)-1)
                {
                    crap.Load(obs2);
                }
            }
            //
            p1.Place(new Vector2(GetScreenWidth() / 4, GetScreenHeight() / 4 * 3));
            p2.Place(new Vector2(GetScreenWidth() / 4 * 3, GetScreenHeight() / 4));
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

            body = bodies[bodyPick];
            barrel = barrels[gunPick];
            //WIN CONDITION CHECK
            if (p1.armor <= 0 || p2.armor <= 0)
            {
                paused = true;
                if (p1.armor <= 0)
                {
                    p2.wins++;
                    p2.winStreak++;
                    p1.winStreak = 0;
                }
                if (p2.armor <= 0)
                {
                    p1.wins++;
                    p1.winStreak++;
                    p2.winStreak = 0;
                }
            }
            if (IsKeyPressed(KeyboardKey.KEY_INSERT))
            {
                Reset();
                paused = true;
            }

            if (IsKeyPressed(KeyboardKey.KEY_M))
            {
                //soundOn = !soundOn;
            }

            if (IsKeyPressed(KeyboardKey.KEY_H))
            {
                p1.HitboxToggle();
                p2.HitboxToggle();
                bullet.hitboxDisplay = !bullet.hitboxDisplay;
                bullet_.hitboxDisplay = !bullet_.hitboxDisplay;
                foreach(SceneObject o in obstacles)
                {
                    o.hitboxDisplay = !o.hitboxDisplay;
                }
            }



            //
            //COLLISIONS!!!!!!!!!!!!!!
            // -- Bullet collides with player
            if (p1.Hitbox.Overlaps(bullet_.blankHitBox) && bullet_.TimeLeft > 0)
            {
                bullet_.Destroy();
                p1.TakeDamage(25 * barrels[p2.gunPick].DamageMult);
            }
            if (p2.Hitbox.Overlaps(bullet.blankHitBox) && bullet.TimeLeft > 0)
            {
                bullet.Destroy();
                p2.TakeDamage(25 * barrels[p1.gunPick].DamageMult);
            }
            // - Obstacle collisions
            foreach (SceneObject obs in obstacles)
            {
                if (obs.health > 0)
                {
                    // -- with bullet
                    if (obs.blankHitBox.Overlaps(bullet.blankHitBox) && bullet.TimeLeft > 0)
                    {
                        if (!obs.passThru)
                        {
                            bullet.Destroy();
                            obs.Damage(25 * barrels[p1.gunPick].DamageMult);
                            Console.Write("butt");
                        }
                        else
                        {
                            if (!bullet.pass)
                            {
                                bullet.pass = true;
                                obs.Damage(25 * barrels[p1.gunPick].DamageMult);
                                Console.Write("butt");
                            }
                            else
                            {

                            }
                        }
                    }
                    if (obs.blankHitBox.Overlaps(bullet_.blankHitBox) && bullet_.TimeLeft > 0)
                    {
                        if (!obs.passThru)
                        {
                            bullet_.Destroy();
                            obs.Damage(25 * barrels[p2.gunPick].DamageMult);
                            //Console.Write("butt");
                        }
                        else
                        {
                            if (!bullet.pass)
                            {
                                bullet_.pass = true;
                                obs.Damage(25 * barrels[p2.gunPick].DamageMult);
                                //Console.Write("butt");
                            }
                            else
                            {

                            }
                        }
                    }
                    // with player
                    if (obs.blankHitBox.Overlaps(p1.Hitbox))
                    {
                        if (!obs.passThru)
                        {
                            p1.Move(deltaTime, -64);
                        }
                    }
                    if (obs.blankHitBox.Overlaps(p2.Hitbox))
                    {
                        if (!obs.passThru)
                        {
                            p2.Move(deltaTime, -64);
                        }
                    }
                }
            }
            //
            bulspr.Load(barrels[p1.gunPick].BulTex);
            bulspr.SetPosition(bulspr.Height / 2.0f, -bulspr.Width / 2.0f);
            bulspr_.Load(barrels[p2.gunPick].BulTex);
            bulspr_.SetPosition(bulspr.Height / 2.0f, -bulspr.Width / 2.0f);


            if (explosionFX.TimeLeft <= 0)
            {
                expSprite.Reset();
            }
            explosionFX.Update(deltaTime);
            expSprite.SetPosition(-expSprite.Width / 2.0f, -expSprite.Height / 2.0f);

            if (explosionFX_.TimeLeft <= 0)
            {
                expSprite_.Reset();
            }
            explosionFX_.Update(deltaTime);
            expSprite_.SetPosition(-expSprite_.Width / 2.0f, -expSprite_.Height / 2.0f);

            bullet.SetStats(barrels[p1.gunPick]);
            bullet_.SetStats(barrels[p2.gunPick]);
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
                    bullet.Face(p1.TurretPointer);
                }
            }
            if (bullet_.TimeLeft > 0)
            {
                bullet_.Update(deltaTime);
            }
            else
            {
                if (!bullet_.Explod)
                {
                    bullet_.Explode(explosionFX_);
                }
                else
                {
                    bullet_.Face(p2.TurretPointer);
                }
            }

            //DEBUG
            //if (IsKeyDown(KeyboardKey.KEY_KP_1))
            //{
            //    gunPick = 0;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_2))
            //{
            //    gunPick = 1;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_3))
            //{
            //    gunPick = 2;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_4))
            //{
            //    gunPick = 3;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_5))
            //{
            //    gunPick = 4;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_6))
            //{
            //    gunPick = 0;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_7))
            //{
            //    bodyPick = 0;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_8))
            //{
            //    bodyPick = 1;
            //}
            //if (IsKeyDown(KeyboardKey.KEY_KP_9))
            //{
            //    bodyPick = 2;
            //}
            //tank control
            if (IsKeyDown(KeyboardKey.KEY_A))
            {
                p1.Rotate(-deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_D))
            {
                p1.Rotate(deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                p1.Move(deltaTime, true);
            }
            if (IsKeyDown(KeyboardKey.KEY_S))
            {
                p1.Move(deltaTime, false);
            }
            if (IsKeyDown(KeyboardKey.KEY_Q))
            {
                p1.GunRotate(-deltaTime * rotSpeed);
                if (barrels[p1.gunPick].guided) bullet.Rotate(-deltaTime * barrels[p1.gunPick].steer);
            }
            if (IsKeyDown(KeyboardKey.KEY_E))
            {
                p1.GunRotate(deltaTime * rotSpeed);
                if (barrels[p1.gunPick].guided) bullet.Rotate(deltaTime * barrels[p1.gunPick].steer);
            }
            if (IsKeyPressed(KeyboardKey.KEY_SPACE) && p1.reload >= p1.reloading && bullet.TimeLeft <= 0)
            {
                bullet.Reset();
                p1.Fire();
                if (soundOn) PlaySound(snd_fire);
            }


            if (IsKeyDown(KeyboardKey.KEY_KP_4))
            {
                p2.Rotate(-deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_6))
            {
                p2.Rotate(deltaTime * body.TurnSpeed);
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_8))
            {
                p2.Move(deltaTime, true);
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_5))
            {
                p2.Move(deltaTime, false);
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_7))
            {
                p2.GunRotate(-deltaTime * rotSpeed);
                if (barrels[p2.gunPick].guided) bullet_.Rotate(-deltaTime * barrels[p2.gunPick].steer);
            }
            if (IsKeyDown(KeyboardKey.KEY_KP_9))
            {
                p2.GunRotate(deltaTime * rotSpeed);
                if (barrels[p2.gunPick].guided) bullet_.Rotate(deltaTime * barrels[p2.gunPick].steer);
            }
            if (IsKeyPressed(KeyboardKey.KEY_KP_0) && p2.reload >= p2.reloading && bullet_.TimeLeft <= 0)
            {
                bullet_.Reset();
                p2.Fire();
                if (soundOn) PlaySound(snd_fire);
            }

            p1.Update(deltaTime);
            p2.Update(deltaTime);

            if (soundOn)
            {
                if (p1.reload == p1.reloading - 45) PlaySound(snd_load);
                if ((IsKeyDown(KeyboardKey.KEY_W) || IsKeyDown(KeyboardKey.KEY_S)))
                {
                    if (IsSoundPlaying(snd_drive))
                    {
                        ResumeSound(snd_drive);
                    }
                    else
                    {
                        PlaySound(snd_drive);
                    }
                }
                else
                {
                    if (IsSoundPlaying(snd_drive))
                    {
                        PauseSound(snd_drive);
                    }
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

            if (!(p1.armor > 0) || !(p2.armor > 0))
            {
                if (IsKeyReleased(KeyboardKey.KEY_ENTER))
                {
                    Reset();
                    p1.Reset();
                    p2.Reset();
                }
            }
            else
            {
                if (IsKeyPressed(KeyboardKey.KEY_ENTER))
                {
                    paused = false;
                    p1.Reset();
                    p2.Reset();
                }

                if (IsKeyPressed(KeyboardKey.KEY_A))
                {
                    if (p1.bodyPick == 0) p1.bodyPick = 2;
                    else p1.bodyPick -= 1;
                }
                if (IsKeyPressed(KeyboardKey.KEY_D))
                {
                    if (p1.bodyPick == bodies.Count - 1) p1.bodyPick = 0;
                    else p1.bodyPick++;
                }
                if (IsKeyPressed(KeyboardKey.KEY_W))
                {
                    if (p1.gunPick == 0) p1.gunPick = barrels.Count - 1;
                    else p1.gunPick -= 1;
                }
                if (IsKeyPressed(KeyboardKey.KEY_S))
                {
                    if (p1.gunPick == barrels.Count - 1) p1.gunPick = 0;
                    else p1.gunPick++;
                }

                if (IsKeyPressed(KeyboardKey.KEY_KP_4))
                {
                    if (p2.bodyPick == 0) p2.bodyPick = 2;
                    else p2.bodyPick -= 1;
                }
                if (IsKeyPressed(KeyboardKey.KEY_KP_6))
                {
                    if (p2.bodyPick == bodies.Count - 1) p2.bodyPick = 0;
                    else p2.bodyPick++;
                }
                if (IsKeyPressed(KeyboardKey.KEY_KP_8))
                {
                    if (p2.gunPick == 0) p2.gunPick = barrels.Count - 1;
                    else p2.gunPick -= 1;
                }
                if (IsKeyPressed(KeyboardKey.KEY_KP_5))
                {
                    if (p2.gunPick == barrels.Count - 1) p2.gunPick = 0;
                    else p2.gunPick++;
                }
                lastTime = currentTime;
            }
        }
        public void Draw()
        {
            BeginDrawing();

            ClearBackground(Color.BEIGE);
            DrawText(fps.ToString(), 10, 10, 10, Color.LIME);
            foreach (SceneObject obstacle in obstacles)
            {
                if (obstacle.health > 0)obstacle.Draw();
            }
            //tankObject.Draw();
            p1.Draw();
            p2.Draw();
            explosionFX.Draw();
            if (bullet.TimeLeft > 0)bullet.Draw();
            explosionFX_.Draw();
            if (bullet_.TimeLeft > 0)bullet_.Draw();

            string p1wins = $"P1 wins: {p1.wins}";
            int p1winM = MeasureText(p1wins, 20);
            string p1winstr = $"P1 win streak: {p1.winStreak}";
            int p1winstrM = MeasureText(p1wins, 20);
            string p2wins = $"P2 wins: {p2.wins}";
            int p2winM = MeasureText(p2wins, 20);
            string p2winstr = $"P1 win streak: {p2.winStreak}";
            int p2winstrM = MeasureText(p2wins, 20);

            

            //P1 HUD
            DrawText("P1", 10, GetScreenHeight() - 30, 20, Color.BLUE);
            DrawRectangle(40, GetScreenHeight() - 192, 24, 128, Color.GRAY);
            DrawRectangle(40, GetScreenHeight() - 64 - (int)((p1.reload / p1.reloading) * 128), 24, (int)((p1.reload / p1.reloading) * 128), p1.loadColor);
            DrawText($"R\nE\nL\nO\nA\nD",10, GetScreenHeight() - 224,20,Color.WHITE);
            DrawRectangle(64, GetScreenHeight() - 64, 128, 24, Color.GRAY);
            DrawRectangle(64, GetScreenHeight() - 64, (int)((p1.armor / p1.maxArmor) * 128), 24, Color.SKYBLUE);
            DrawText($"A R M O R", 64, GetScreenHeight() - 30, 20, Color.WHITE);
            if (p1.winStreak > 0)
            {
                //DrawText(p1winstr,128, GetScreenHeight() - 96,20,Color.BLUE);
            }
            //P2 HUD
            DrawText("P2", GetScreenWidth() - 32, 10, 20, Color.RED);
            DrawRectangle(GetScreenWidth() - 64, 64, 24, 128, Color.GRAY);
            DrawRectangle(GetScreenWidth() - 64, 64, 24, (int)((p2.reload / p2.reloading) * 128), p2.loadColor);
            DrawText($"R\nE\nL\nO\nA\nD", GetScreenWidth() - 24, 48, 20, Color.WHITE);
            DrawRectangle(GetScreenWidth() - 192, 40, 128, 24, Color.GRAY);
            DrawRectangle(GetScreenWidth() - 192, 40, (int)((p2.armor / p2.maxArmor) * 128), 24, Color.SKYBLUE);
            DrawText($"A R M O R", GetScreenWidth() - 168 , 10, 20, Color.WHITE);
            //DrawCircle((int)tankTL.GlobalTransform.m7, (int)tankTL.GlobalTransform.m8,3,Color.GREEN);
            if (p2.winStreak > 0)
            {
                //DrawText(p2winstr, GetScreenWidth() - 168, 42, 20, Color.RED);
            }


            if (paused)
            {
                if (p1.armor > 0 && p2.armor > 0)
                {

                    Vector2 bodyWritePos = new Vector2((GetScreenWidth() / 5) - (((bodies.Count - 1) * 128) - 48), (GetScreenHeight() / 2) - ((barrels.Count - 1) * 64) + 32);
                    Vector2 barrelWritePos = new Vector2(GetScreenWidth() / 5, (GetScreenHeight() / 2) - ((barrels.Count - 1) * 32));
                    Vector2 bodyWritePos_ = new Vector2((GetScreenWidth() / 5 * 4) - (((bodies.Count - 1) * 128) - 48), (GetScreenHeight() / 2) - ((barrels.Count - 1) * 64) + 32);
                    Vector2 barrelWritePos_ = new Vector2((GetScreenWidth() / 5) * 4, (GetScreenHeight() / 2) - ((barrels.Count - 1) * 32));

                    int bottomTip = MeasureText(helper, 20);
                    int debugTip = MeasureText(helper0, 10);

                    for (int b = 0; b < bodies.Count; b++)
                    {
                        DrawText(helper, (GetScreenWidth() / 2) - (bottomTip/2), GetScreenHeight() - 64, 20, Color.BROWN);
                        DrawText(helper0, (GetScreenWidth() / 2) - (debugTip/2), GetScreenHeight() - 128, 10, Color.BROWN);
                        DrawText(helper1, (int)bodyWritePos.x, (int)bodyWritePos.y - 128, 20, Color.BLUE);
                        DrawText(helper2, (int)bodyWritePos_.x, (int)bodyWritePos_.y - 128, 20, Color.RED);
                        DrawRectangle((int)bodyWritePos.x + (b * 144), (int)bodyWritePos.y, 128, 32, (p1.bodyPick == b) ? Color.SKYBLUE : Color.LIGHTGRAY);
                        DrawRectangleLines((int)bodyWritePos.x + (b * 144), (int)bodyWritePos.y, 128, 32, (p1.bodyPick == b) ? Color.BLUE : Color.GRAY);
                        int tempM = MeasureText(bodies[b].Name, 10);
                        DrawText(bodies[b].Name, (int)(bodyWritePos.x + (b * 144)) + 64 - (tempM / 2), (int)bodyWritePos.y, 10, Color.WHITE);

                        DrawRectangle((int)bodyWritePos_.x + (b * 144), (int)bodyWritePos_.y, 128, 32, (p2.bodyPick == b) ? Color.SKYBLUE : Color.LIGHTGRAY);
                        DrawRectangleLines((int)bodyWritePos_.x + (b * 144), (int)bodyWritePos_.y, 128, 32, (p2.bodyPick == b) ? Color.BLUE : Color.GRAY);
                        DrawText(bodies[b].Name, (int)(bodyWritePos_.x + (b * 144)) + 64 - (tempM / 2), (int)bodyWritePos_.y, 10, Color.WHITE);
                    }
                    for (int b = 0; b < barrels.Count; b++)
                    {
                        DrawRectangle((int)barrelWritePos.x - 72, (int)barrelWritePos.y + b * 64 - 16, 144, 32, (p1.gunPick == b) ? Color.SKYBLUE : Color.LIGHTGRAY);
                        DrawRectangleLines((int)barrelWritePos.x - 72, (int)barrelWritePos.y + b * 64 - 16, 144, 32, (p1.gunPick == b) ? Color.BLUE : Color.GRAY);
                        int tempM = MeasureText(barrels[b].Name, 10);
                        DrawText(barrels[b].Name, (int)barrelWritePos.x - (tempM / 2), (int)barrelWritePos.y + (b * 64), 10, Color.WHITE);

                        DrawRectangle((int)barrelWritePos_.x - 72, (int)barrelWritePos_.y + b * 64 - 16, 144, 32, (p2.gunPick == b) ? Color.SKYBLUE : Color.LIGHTGRAY);
                        DrawRectangleLines((int)barrelWritePos_.x - 72, (int)barrelWritePos_.y + b * 64 - 16, 144, 32, (p2.gunPick == b) ? Color.BLUE : Color.GRAY);
                        DrawText(barrels[b].Name, (int)barrelWritePos_.x - (tempM / 2), (int)barrelWritePos_.y + (b * 64), 10, Color.WHITE);
                    }
                }
                else
                {
                    if (p1.armor <= 0)
                    {
                        int meas = MeasureText("PLAYER 2 WINS!!!", 40);
                        int meas_ = MeasureText($"Win Streak: {p2.winStreak}", 20);
                        DrawText($"PLAYER 2 WINS!!!", (GetScreenWidth() / 2) - (meas / 2), GetScreenHeight() /2, 40,Color.RED);
                        DrawText($"Win Streak: {p2.winStreak}", (GetScreenWidth() / 2) - (meas_ / 2), (GetScreenHeight() / 2) + 40, 20,Color.RED);
                    }
                    if (p2.armor <= 0)
                    {
                        int meas = MeasureText("PLAYER 1 WINS!!!", 40);
                        int meas_ = MeasureText($"Win Streak: {p1.winStreak}", 20);
                        DrawText($"PLAYER 1 WINS!!!", (GetScreenWidth() / 2) - (meas / 2), GetScreenHeight() / 2, 40, Color.BLUE);
                        DrawText($"Win Streak: {p1.winStreak}", (GetScreenWidth() / 2) - (meas_ / 2), (GetScreenHeight() / 2) + 40, 20, Color.BLUE);
                    }
                }
            }

            EndDrawing();
            
        }
        public void Reset()
        {
            p1.Init(bodies, barrels, 1);
            p2.Init(bodies2, barrels, 2);
            p1.Place(new Vector2(GetScreenWidth() / 4, GetScreenHeight() / 4 * 3));
            p2.Place(new Vector2(GetScreenWidth() / 4 * 3, GetScreenHeight() / 4));
            bullet.Destroy();
            bullet_.Destroy();
            explosionFX.Deactivate();
            explosionFX_.Deactivate();
            foreach (SceneObject o in obstacles)
            {
                o.health = obsHealth;
            }
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
