/* Author: Amy Hua
 * File Name: Game1.cs
 * Project Name: AHua_Pass3
 * Creation Date: Jan. 6, 2022
 * Modification Date: Jan. 21, 2022
 * Description: Plays shooting game called Chroma Bullet
 */
using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AHua_Pass3
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        static Random rng = new Random();

        //stores variable defaults
        string gameState;

        //stores fonts
        SpriteFont generalFont;
        SpriteFont titleFont;

        //stores sounds
        Song menuMusic;
        Song gamePlayMusic;

        //stores sound effects
        SoundEffect bulletSound;

        //stores game background objects
        Texture2D title;
        Rectangle titleRec;
        Texture2D background;
        Rectangle backgroundRec;

        //stores screen overlay
        Texture2D colourOverlay;
        Rectangle colourOverlayRec;
        Texture2D controlsOverlay;
        Rectangle controlsOverlayRec;

        //stores bools for showing screen overlays
        bool showControls = false;
        bool showHowToPlay = false;

        //stores displayed words
        Texture2D start;
        Rectangle startRec;
        Texture2D controls;
        Rectangle controlsRec;
        Texture2D howToPlay;
        Rectangle howToPlayRec;
        string howToPlayText;
        Texture2D quit;
        Rectangle quitRec;
        Texture2D resume;
        Rectangle resumeRec;
        Texture2D quitToMenu;
        Rectangle quitToMenuRec;        

        //stores button settings
        Vector2 buttonRes = new Vector2(228, 90);
        Vector2 menuButtonsLoc = new Vector2(280, 280);
        int buttonSpacing = 5;

        //stores constant for game pieces
        const int gamePieceSpeed = 5;
        const int defaultBulletNum = 10;
        const int defaultObstacleNum = 20;
        const int bulletIntervalTime = 20;
        const int damagePerCollision = 10;

        //stores rng variables for creating obstacles
        int obstacleNumRng;
        int obstacleColourRng;
        int obstacleXLocRng;

        //stores images for game pieces
        Texture2D blueBullet;
        Rectangle blueBulletRec;
        Rectangle[] blueBullets = new Rectangle[defaultBulletNum];
        Texture2D redBullet;
        Rectangle redBulletRec;
        Rectangle[] redBullets = new Rectangle[defaultBulletNum];
        Texture2D blueObstacle;
        Rectangle blueObstacleRec;
        Rectangle[] blueObstacles = new Rectangle[defaultObstacleNum];
        Texture2D redObstacle;
        Rectangle redObstacleRec;
        Rectangle[] redObstacles = new Rectangle[defaultObstacleNum];
        Texture2D rocket;
        Rectangle rocketRec;

        //stores default rectangles for game pieces
        Rectangle rocketDefaultRec;
        Rectangle bulletDefaultRec;
        Rectangle obstacleDefaultRec;

        //stores invisible barriers for game pieces
        int leftBarrier, rightBarrier;

        //stores timer
        int timer = 0;

        //stores color opacity for when player takes damage
        float damageOpacity = 0f;

        //keeps track of bullet count        
        int redBulletCount;
        int blueBulletCount;

        //stores location of changing gameplay variables shown to player
        Rectangle redBulletCountRec;
        Rectangle blueBulletCountRec;
        Vector2 scoreLoc = new Vector2(15, 25);

        //stores default variables
        const string gameStateDefault = "menu";

        //keeps track of score
        int score = 0;
        int highScore = 0;        

        //stores input states
        MouseState mouse, prevMouse;
        KeyboardState kb, prevKb;

        //stores screen measurements
        int screenWidth, screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //sets width and height of screen
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 580;
            graphics.ApplyChanges();

            ResetDefaultSettings();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //creates a new SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //assigns screen width and height to variables
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //loads text fonts
            generalFont = Content.Load<SpriteFont>("Fonts/GeneralFont");
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");

            //loads music
            menuMusic = Content.Load<Song>("Audio/Music/BgMusic");
            gamePlayMusic = Content.Load<Song>("Audio/Music/GameplayMusic");

            //loads sound effects
            bulletSound = Content.Load<SoundEffect>("Audio/Sound/BulletSfx");

            //sets sound settings
            MediaPlayer.Volume = 1f;
            MediaPlayer.IsRepeating = true;
            SoundEffect.MasterVolume = 0.8f;

            //loads game background
            title = Content.Load<Texture2D>("Background/Title");
            titleRec = new Rectangle(0, 95, title.Width, title.Height);
            background = Content.Load<Texture2D>("Background/background");
            backgroundRec = new Rectangle(0, 0, background.Width, background.Height);

            //loads overlayed screens
            colourOverlay = Content.Load<Texture2D>("Background/colourOverlay");
            colourOverlayRec = new Rectangle(0, 0, screenWidth, screenHeight);
            controlsOverlay = Content.Load<Texture2D>("Background/controlsOverlay");
            controlsOverlayRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //loads buttons
            start = Content.Load<Texture2D>("Buttons/Start");
            startRec = new Rectangle((int)menuButtonsLoc.X, (int)menuButtonsLoc.Y, (int)buttonRes.X, (int)buttonRes.Y);
            controls = Content.Load<Texture2D>("Buttons/Controls");
            controlsRec = new Rectangle(0, 0, (int)(buttonRes.X * 0.8), (int)(buttonRes.Y * 0.8));
            howToPlay = Content.Load<Texture2D>("Buttons/HowToPlay");
            howToPlayRec = new Rectangle((int)(menuButtonsLoc.X), (int)(menuButtonsLoc.Y + (buttonRes.Y + buttonSpacing)), (int)buttonRes.X, (int)buttonRes.Y);
            quit = Content.Load<Texture2D>("Buttons/Quit");
            quitRec = new Rectangle((int)menuButtonsLoc.X, (int)(menuButtonsLoc.Y + 2 * (buttonRes.Y + buttonSpacing)), (int)buttonRes.X, (int)buttonRes.Y);
            resume = Content.Load<Texture2D>("Buttons/Resume");
            resumeRec = new Rectangle((int)menuButtonsLoc.X, (int)(menuButtonsLoc.Y), (int)buttonRes.X, (int)buttonRes.Y);
            quitToMenu = Content.Load<Texture2D>("Buttons/QuitToMenu");
            quitToMenuRec = new Rectangle((int)(menuButtonsLoc.X), (int)(menuButtonsLoc.Y + (buttonRes.Y + buttonSpacing)), (int)buttonRes.X, (int)buttonRes.Y);

            //stores displayed text;
            howToPlayText = "- Move the ship to avoid asteroids and shoot bullets \nto destroy them\n- Bullets can only destroy asteroids of the \nsame colour \n- Destroying an asteroid will refund 2 bullets \nof the same type \n- If the ship hits an asteroid, you will lose 10 bullets \nof each colour (bullet count will not go below 0). \n- If you run out of one bullet type, destroying \nan asteroid with the other bullet type will refund \n1 bullet per colour\n-Game ends when you have 0 bullets of each type";

            //loads game pieces
            rocket = Content.Load<Texture2D>("Game Pieces/Rocket");
            redBullet = Content.Load<Texture2D>("Game Pieces/RedBullet");
            blueBullet = Content.Load<Texture2D>("Game Pieces/BlueBullet");
            redObstacle = Content.Load<Texture2D>("Game Pieces/RedObstacle");
            blueObstacle = Content.Load<Texture2D>("Game Pieces/BlueObstacle");

            //sets default game piece rectangles
            rocketDefaultRec = new Rectangle(370, 450, rocket.Width, rocket.Height);
            bulletDefaultRec = new Rectangle(-100, 100, (int)(redBullet.Width * 0.4), (int)(redBullet.Height * 0.4));
            obstacleDefaultRec = new Rectangle(-100, -70, redObstacle.Width, redObstacle.Height);
            
            //sets game pieces to default rectangles
            rocketRec = rocketDefaultRec;
            redBulletRec = bulletDefaultRec;
            blueBulletRec = bulletDefaultRec;
            redObstacleRec = obstacleDefaultRec;
            blueObstacleRec = obstacleDefaultRec;

            //sets invisible barriers for game pieces
            leftBarrier = 20;
            rightBarrier = screenWidth - rocketRec.Width - 20;

            //stores rectangle for bullet count display
            redBulletCountRec = new Rectangle(680, 30, bulletDefaultRec.Width, bulletDefaultRec.Height);
            blueBulletCountRec = new Rectangle(redBulletCountRec.X, redBulletCountRec.Y + 25, bulletDefaultRec.Width, bulletDefaultRec.Height);

            //stores bullets in arrays
            for (int i = 0; i < defaultBulletNum; i++)
            {
                redBullets[i] = redBulletRec;
                blueBullets[i] = blueBulletRec;
            }

            //stores obstacles in arrays
            for (int i = 0; i < defaultObstacleNum; i++)
            {
                redObstacles[i] = redObstacleRec;
                blueObstacles[i] = blueObstacleRec;
            }

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            //keeps track of input states
            prevMouse = mouse;
            mouse = Mouse.GetState();
            prevKb = kb;
            kb = Keyboard.GetState();

            switch (gameState)
            {
                case "menu":
                    //plays music if not already playing
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(menuMusic);
                    }

                    //closes overlaying tabs if open
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        showControls = false;
                        showHowToPlay = false;
                    }

                    //alters gamestates and opens/closes on-screen overlays
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //closes overlaying tabs if open
                        if (showControls == true || showHowToPlay == true)
                        {
                            showControls = false;
                            showHowToPlay = false;
                            break;
                        }
                        
                        //overlays controls
                        if (showControls == false && controlsRec.Contains(mouse.Position))
                        {
                            showControls = true;
                            break;
                        }
                        
                        //overlays how to play tab
                        else if (showHowToPlay == false && howToPlayRec.Contains(mouse.Position))
                        {
                            showHowToPlay = true;
                            break;
                        }                        

                        //quits game
                        if (quitRec.Contains(mouse.Position))
                        {
                            Exit();
                        }

                        //starts gameplay
                        if (startRec.Contains(mouse.Position))
                        {
                            IsMouseVisible = false;
                            MediaPlayer.Stop();
                            gameState = "gamePlay";
                            break;
                        }
                    }
                    break;

                case "gamePlay":                  
                    //plays music if not already playing
                    if (MediaPlayer.State != MediaState.Playing && timer >= 20)
                    {
                        MediaPlayer.Play(gamePlayMusic);
                    }

                    //checks if player pauses game
                    if (kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyDown(Keys.Escape) != true)
                    {
                        timer = 0;
                        IsMouseVisible = true;
                        MediaPlayer.Pause();
                        gameState = "gamePaused";
                        break;
                    }

                    //generates obstacles                    
                    if (timer % 15 == 0)
                    {
                        //generates the number of obstacles spawned
                        obstacleNumRng = rng.Next(0, 2);
                        
                        //generates obstacles
                        int i = 0;
                        while (i <= obstacleNumRng)
                        {
                            //generates obstacle colour
                            obstacleColourRng = rng.Next(1, 3);

                            //generates obstacle according to generated colour
                            if (obstacleColourRng == 1)
                            {
                                GenerateObstacle(redObstacles);
                            }
                            else
                            {
                                GenerateObstacle(blueObstacles);
                            }
                            i++;
                        }
                        
                    }
                    
                    //checks to let player move rocket left or right
                    if (kb.IsKeyDown(Keys.A) && rocketRec.X > leftBarrier)
                    {
                        rocketRec.X -= gamePieceSpeed;
                    }
                    if (kb.IsKeyDown(Keys.D) && rocketRec.X < rightBarrier)
                    {
                        rocketRec.X += gamePieceSpeed;
                    }

                    //checks to let player shoot bullets
                    if (mouse.LeftButton == ButtonState.Pressed && timer > bulletIntervalTime && redBulletCount > 0)
                    {
                        redBulletCount = ShootBullet(redBullets, redBulletCount);
                    }
                    if (mouse.RightButton == ButtonState.Pressed && timer > bulletIntervalTime && blueBulletCount > 0)
                    {
                        blueBulletCount =  ShootBullet(blueBullets, blueBulletCount);
                    }

                    //moves and resets position of bullets
                    MoveBullets(redBullets);
                    MoveBullets(blueBullets);

                    //moves and resets position of obstacles
                    MoveObstacles(redObstacles);
                    MoveObstacles(blueObstacles);

                    //checks for bullet collision with obstacles
                    CheckBulletCollision(redBullets, "red");
                    CheckBulletCollision(blueBullets, "blue");

                    //checks for rocket collision with obstacles
                    CheckRocketCollision(redObstacles, redObstacle);
                    CheckRocketCollision(blueObstacles, blueObstacle);

                    //checks if game is over
                    if (redBulletCount == 0 && blueBulletCount == 0 && AllBulletsDefault() == true)
                    {
                        IsMouseVisible = true;
                        MediaPlayer.Stop();
                        gameState = "gameEnd";
                    }

                    //decreases opacity when damage is taken
                    if (damageOpacity > 0f)
                    {
                        damageOpacity = damageOpacity - 0.02f;
                    }

                    //increases increases time based variables
                    timer++;
                    score++;
                    break;

                case "gamePaused":
                    //resumes gameplay
                    if (kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyDown(Keys.Escape) != true)
                    {
                        IsMouseVisible = false;
                        MediaPlayer.Resume();
                        gameState = "gamePlay";
                        break;
                    }

                    //changes gamestate depending on mouse input
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //resumes gameplay
                        if (resumeRec.Contains(mouse.Position))
                        {
                            IsMouseVisible = false;
                            MediaPlayer.Resume();
                            gameState = "gamePlay";
                            break;
                        }

                        //quits back to menu
                        if (quitToMenuRec.Contains(mouse.Position))
                        {
                            CalculateHighScore();
                            ResetDefaultSettings();
                            gameState = "menu";
                            break;
                        }
                    }
                    break;

                case "gameEnd":
                    //returns to menu
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        CalculateHighScore();
                        ResetDefaultSettings();
                        gameState = "menu";
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //draws graphics
            spriteBatch.Begin();
            switch (gameState)
            {
                case "menu":
                    //draws background
                    spriteBatch.Draw(background, backgroundRec, Color.White);
                    spriteBatch.Draw(title, titleRec, Color.White);

                    //draws buttons
                    spriteBatch.Draw(start, startRec, Color.White);
                    spriteBatch.Draw(howToPlay, howToPlayRec, Color.White);
                    spriteBatch.Draw(quit, quitRec, Color.White);
                    spriteBatch.Draw(controls, controlsRec, Color.White);

                    //draws high score
                    spriteBatch.DrawString(generalFont, "High Score: " + highScore, new Vector2(CenterTextX("High Score: " + highScore, generalFont), 250), Color.White);
                    
                    //shows controls overlay
                    if (showControls == true)
                    {
                        spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Black * 0.8f);
                        spriteBatch.Draw(controlsOverlay, controlsOverlayRec, Color.White);
                    }

                    //shows how to play overlay
                    if (showHowToPlay == true)
                    {
                        spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Black * 0.8f);
                        spriteBatch.DrawString(generalFont, howToPlayText, new Vector2(10, 130), Color.White);
                    }
                    break;

                case "gamePlay":
                    DrawGame();
                    
                    //draws damage colour overlay if player has taken damage
                    spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Red * damageOpacity);
                    break;

                case "gamePaused":
                    DrawGame();
                    
                    //overlays dark layer
                    spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Black * 0.6f);

                    //draws buttons
                    spriteBatch.Draw(resume, resumeRec, Color.White);
                    spriteBatch.Draw(quitToMenu, quitToMenuRec, Color.White);
                    break;

                case "gameEnd":
                    DrawGame();

                    //overlays dark layer
                    spriteBatch.Draw(colourOverlay, colourOverlayRec, Color.Black * 0.6f);

                    //draws text over background
                    spriteBatch.DrawString(titleFont, "Game Over", new Vector2(252, 220), Color.White);
                    spriteBatch.DrawString(generalFont, "Score: " + Convert.ToString(score), new Vector2(CenterTextX("Score" + Convert.ToString(score), generalFont), 290), Color.White);
                    spriteBatch.DrawString(generalFont, "Tap anywhere to Continue", new Vector2(214, 320), Color.BlueViolet);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //resets game variables to default
        private void ResetDefaultSettings()
        {
            //resets all bullet positions to default
            for (int i = 0; i < defaultBulletNum; i++)
            {
                redBullets[i].X = (int)bulletDefaultRec.X;
                redBullets[i].Y = (int)bulletDefaultRec.Y;
                blueBullets[i].X = (int)bulletDefaultRec.X;
                blueBullets[i].Y = (int)bulletDefaultRec.Y;
            }

            //resets all obstacles positions to default
            for (int i = 0; i < defaultBulletNum; i++)
            {
                redObstacles[i].X = (int)obstacleDefaultRec.X;
                redObstacles[i].Y = (int)obstacleDefaultRec.Y;
                blueObstacles[i].X = (int)obstacleDefaultRec.X;
                blueObstacles[i].Y = (int)obstacleDefaultRec.Y;
            }

            //resets default bullet count
            redBulletCount = defaultBulletNum;
            blueBulletCount = defaultBulletNum;
            
            //resets all other variables
            gameState = gameStateDefault;
            score = 0;
            timer = 0;
            rocketRec = rocketDefaultRec;
            damageOpacity = 0f;
            IsMouseVisible = true;
        }

        //calculates new high score
        private void CalculateHighScore()
        {
            if (score > highScore)
            {
                highScore = score;
            }
        }

        //spawns obstacle
        private void GenerateObstacle(Rectangle[] obstacle)
        {
            //generates x position of obstacle between invisible barriers
            obstacleXLocRng = rng.Next(leftBarrier, rightBarrier);

            //sets new location of obstacle
            for (int i = 0; i < defaultObstacleNum; i++)
            {
                if (obstacle[i].X == obstacleDefaultRec.X)
                {
                    obstacle[i].X = obstacleXLocRng;
                    break;
                }
            }
        }

        //shoots bullet
        private int ShootBullet(Rectangle[] bullets, int bulletCount)
        {
            for (int i = 0; i < defaultBulletNum; i++)
            {
                if (bullets[i] == bulletDefaultRec)
                {
                    bullets[i].X = rocketRec.X + 19;
                    bullets[i].Y = rocketRec.Y - 5;
                    bulletCount--;
                    bulletSound.CreateInstance().Play();
                    break;
                }
            }
            
            //resets bullet timer
            timer = 0;
            return bulletCount;
        }

        //moves and reset position of bullets
        private void MoveBullets(Rectangle[] bullets)
        {
            //moves bullets in the screen
            for (int i = 0; i < defaultBulletNum; i++)
            {
                if (bullets[i] != bulletDefaultRec)
                {
                    bullets[i].Y = bullets[i].Y - gamePieceSpeed;
                }
                
                //resets position of bullets outside screen
                if (bullets[i].Y < -10)
                {
                    bullets[i].X = (int)bulletDefaultRec.X;
                    bullets[i].Y = (int)bulletDefaultRec.Y;
                }
            }
        }

        //moves and reset position of obstacles
        private void MoveObstacles(Rectangle[] obstacles)
        {
            //moves obstacles in the screen
            for (int i = 0; i < defaultObstacleNum; i++)
            {
                if (obstacles[i] != obstacleDefaultRec)
                {
                    obstacles[i].Y = obstacles[i].Y + gamePieceSpeed;
                }

                //resets position of obstacles outside of screen
                if (obstacles[i].Y > 580)
                {
                    obstacles[i].X = (int)obstacleDefaultRec.X;
                    obstacles[i].Y = (int)obstacleDefaultRec.Y;
                }
            }
        }

        //checks for collision between rocket and obstacles
        private void CheckRocketCollision(Rectangle[] obstacles, Texture2D obstacle)
        {
            //checks if obstacle has made collision with rocket
            for (int i = 0; i < defaultObstacleNum; i++)
            {
                if (obstacles[i].Intersects(rocketRec))
                {                    
                    //deducts bullet count if collision is made
                    redBulletCount = redBulletCount - damagePerCollision;
                    blueBulletCount = blueBulletCount - damagePerCollision;

                    //sets bullet count to 0 if bullet count is negative
                    if (redBulletCount < 0)
                    {
                        redBulletCount = 0;
                    }
                    if (blueBulletCount < 0)
                    {
                        blueBulletCount = 0;
                    }

                    //resets position of obstacle
                    obstacles[i].X = (int)obstacleDefaultRec.X;
                    obstacles[i].Y = (int)obstacleDefaultRec.Y;

                    //changes damage overlay opacity
                    damageOpacity = 0.3f;
                }
            }
        }

        //checks for bullet collision with obstacles
        private void CheckBulletCollision(Rectangle[] bullets, string colour)
        {
            for (int i = 0; i < defaultBulletNum; i++)
            {
                for (int j = 0; j < defaultObstacleNum; j++)
                {
                    //checks if bullet has hit a red obstacle
                    if (bullets[i].Intersects(redObstacles[j]))
                    {                        
                        //changes bullet count depending on situation
                        if (colour.Equals("red"))
                        {
                            redObstacles[j] = obstacleDefaultRec;
                            if (blueBulletCount == 0)
                            {
                                blueBulletCount++;
                                redBulletCount++;
                            }
                            else
                            {
                                redBulletCount = redBulletCount + 2;
                            }
                            
                        }
                        //resets position of bullet
                        bullets[i] = bulletDefaultRec;
                    }

                    //checks if bullet has hit a blue obstacle
                    else if (bullets[i].Intersects(blueObstacles[j]))
                    {                        
                        //changes bullet count depending on situation
                        if (colour.Equals("blue"))
                        {
                            blueObstacles[j] = obstacleDefaultRec;
                            if (redBulletCount == 0)
                            {
                                redBulletCount++;
                                blueBulletCount++;
                            }
                            else
                            {
                                blueBulletCount = blueBulletCount + 2;
                            }
                        }                        
                        //resets position of bullet
                        bullets[i] = bulletDefaultRec;
                    }
                }
            }
        }
        
        //checks if all bullets are at default position
        private bool AllBulletsDefault()
        {
            bool allBulletsDefault = true;
            for (int i = 0; i < defaultBulletNum; i++)
            {
                if (redBullets[i] != bulletDefaultRec || blueBullets[i] != bulletDefaultRec)
                {
                    allBulletsDefault = false;
                    break;
                }
            }
            return allBulletsDefault;
        }

        //calcualtes x position of centered text
        private int CenterTextX(string text, SpriteFont font)
        {
            int locX = (int)(screenWidth/2 - font.MeasureString(text).X / 2);
            return locX;
        }

        //draws all game pieces during game play
        private void DrawGame()
        {
            //draws background
            spriteBatch.Draw(background, backgroundRec, Color.White);
            
            //draws bullets
            for (int i = 0; i < defaultBulletNum; i++)
            {
                spriteBatch.Draw(redBullet, redBullets[i], Color.White);
                spriteBatch.Draw(blueBullet, blueBullets[i], Color.White);
            }

            //draws obstacles
            for (int i = 0; i < defaultObstacleNum; i++)
            {
                spriteBatch.Draw(redObstacle, redObstacles[i], Color.White);
                spriteBatch.Draw(blueObstacle, blueObstacles[i], Color.White);
            }

            //draws rocket
            spriteBatch.Draw(rocket, rocketRec, Color.White);

            //draws bullet count
            spriteBatch.Draw(redBullet, redBulletCountRec, Color.White);
            spriteBatch.DrawString(generalFont, Convert.ToString(redBulletCount), new Vector2(redBulletCountRec.X + 15, redBulletCountRec.Y - 2), Color.White);
            spriteBatch.Draw(blueBullet, blueBulletCountRec, Color.White);
            spriteBatch.DrawString(generalFont, Convert.ToString(blueBulletCount), new Vector2(blueBulletCountRec.X + 15, blueBulletCountRec.Y - 2), Color.White);

            //draws scores
            spriteBatch.DrawString(generalFont, "Score: " + Convert.ToString(score), scoreLoc, Color.White);
            spriteBatch.DrawString(generalFont, "High Score: " + Convert.ToString(highScore), new Vector2(scoreLoc.X, scoreLoc.Y + 25), Color.White);
        }
    }
}
