using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XnaRace
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        private Texture2D mCar;
        private Texture2D mCar2;
        private Texture2D mBackground;
        private Texture2D mRoad;
        private Texture2D mRoad2;
        private Texture2D mHazard;
        private Texture2D mCoin;
        private Texture2D mNos;

        private KeyboardState mPreviousKeyboardState;
        private Vector2 mCarPosition = new Vector2(280, 440);
        private int mMoveCarX = 5;
        private int mVelocityY;
        private double mNextHazardAppearsIn;
        private double mNextAppleAppearsIn;
        private double mNextPineappleAppearsIn;
        private int mCarsRemaining;
        private int score;
        private int mIncreaseVelocity;
        private double mExitCountDown = 10;

        private int[] mRoadY = new int[2];
        private List<Hazard> mHazards = new List<Hazard>();
        private List<Coin> mCoins = new List<Coin>();
        private List<Nos> mNoss = new List<Nos>();

        private int CarSelect = 1;
        private Boolean existCoin = false;
        private Boolean existNos = false;

        private Random mRandom = new Random();
        private SpriteFont mFont;


        private enum State
        {
            TitleScreen,
            intro,
            Running,
            Crash,
            GameOver,
            Success
        }
        


        private State mCurrentState = State.TitleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 760;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mCar = Content.Load<Texture2D>("1car");
            mCar2 = Content.Load<Texture2D>("Car2");
            mBackground = Content.Load<Texture2D>("Background");
            mRoad = Content.Load<Texture2D>("1road");
            mRoad2 = Content.Load<Texture2D>("2road");
            mHazard = Content.Load<Texture2D>("1hazard");
            mCoin = Content.Load<Texture2D>("coin");
            mNos = Content.Load<Texture2D>("nos");
            mFont = Content.Load<SpriteFont>("MyFont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected void introGame()
        {
            mCurrentState = State.intro;
        }
        protected void StartGame()
        {
            mRoadY[0] = 0;
            mRoadY[1] = -1 * mRoad.Height;
            
            score = 0;
            if (CarSelect == 1)
            {
                mCarsRemaining = 5;
                mVelocityY = 6;
            }
            else if (CarSelect == 2)
            {
                mCarsRemaining = 5;
                mVelocityY = 4;
            }
            mNextHazardAppearsIn = 1;
            mNextAppleAppearsIn = 3.1;
            mNextPineappleAppearsIn = 5.2;
            mIncreaseVelocity = 5;

            mHazards.Clear();
            mCoins.Clear();

            mCurrentState = State.Running;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            //Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                aCurrentKeyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }

            if (aCurrentKeyboardState.IsKeyDown(Keys.M))
            {
                introGame();
            }
            switch (mCurrentState)
            {
                case State.TitleScreen:
                    {
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true/*&& mPreviousKeyboardState.IsKeyDown(Keys.Space) == false*/)
                        {
                            introGame();
                        }
                        break;
                    }
                case State.intro:
                case State.Success:
                case State.GameOver:
                    {
                        //ExitCountdown(gameTime);

                        if (aCurrentKeyboardState.IsKeyDown(Keys.A) == true && mPreviousKeyboardState.IsKeyDown(Keys.A) == false)
                        {
                            mMoveCarX = 8;
                            CarSelect = 1;
                            mVelocityY = 5;
                            StartGame();
                        }
                        if (aCurrentKeyboardState.IsKeyDown(Keys.S) == true && mPreviousKeyboardState.IsKeyDown(Keys.S) == false)
                        {
                            mMoveCarX = 5;
                            CarSelect = 2;
                            mVelocityY = 4;
                            StartGame();
                        }
                        break;
                    }

                case State.Running:
                    {
                        if (CarSelect == 1)
                            if (mVelocityY > 15) mVelocityY = 15;
                        if (CarSelect == 2)
                            if (mVelocityY > 10) mVelocityY = 10;
                        //If the user has pressed the Spacebar, then make the Car switch lanes
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
                        {
                            mCarPosition.X -= mMoveCarX;
                        }
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
                        {
                            mCarPosition.X += mMoveCarX;
                        }
                        if (mCarPosition.X >= 620 || mCarPosition.X <= 50)
                        {
                            mCurrentState = State.Crash;
                            mCarsRemaining--;
                            
                            if (mCarsRemaining < 0)
                            {
                                mCurrentState = State.GameOver;
                               
                                if (mCarPosition.X <= 50) mCarPosition.X += 5;
                                if (mCarPosition.X >= 600) mCarPosition.X -= 5;
                            }
                        }
                        ScrollRoad();
                        foreach (Hazard aHazard in mHazards)
                        {
                            if (CheckCollision(aHazard) == true)
                            {
                                break;
                            }
                            MoveHazard(aHazard);
                        }
                        foreach (Coin aCoins in mCoins)
                        {
                            if (CheckCoinCollision(aCoins) == 1)
                            {
                                break;
                            }
                            MoveCoin(aCoins);
                        }
                        foreach (Nos aNos in mNoss)
                        {
                            if (CheckNosPosition(aNos) == 1)
                            {
                                break;
                            }
                            MoveNos(aNos);
                        }
                        UpdateHazards(gameTime);
                        UpdateCoins(gameTime);
                        UpdateNos(gameTime);
                        break;
                    }
                case State.Crash:
                    {
                        //If the user has pressed the Space key, then resume driving
                        if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true && mPreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                        {
                            mHazards.Clear();
                            mCoins.Clear();
                            existCoin = false;
                            foreach (Coin coin in mCoins)
                            {
                                coin.isEated = false;
                                coin.Visible = true;
                            }
                            foreach (Nos pineapple in mNoss)
                            {
                                pineapple.isEated = false;
                                pineapple.Visible = true;
                            }
                            existCoin = false;
                            existNos = false;
                            if (mCarPosition.X <= 50) mCarPosition.X += 20;
                            if (mCarPosition.X >= 600) mCarPosition.X -= 20;
                            mCurrentState = State.Running;
                        }

                        break;
                    }
            }
            mPreviousKeyboardState = aCurrentKeyboardState;

            base.Update(gameTime);
        }


        private void ScrollRoad()
        {
            //Move the scrolling Road
            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                if (mRoadY[aIndex] >= this.Window.ClientBounds.Height)
                {
                    int aLastRoadIndex = aIndex;
                    for (int aCounter = 0; aCounter < mRoadY.Length; aCounter++)
                    {
                        if (mRoadY[aCounter] < mRoadY[aLastRoadIndex])
                        {
                            aLastRoadIndex = aCounter;
                        }
                    }
                    mRoadY[aIndex] = mRoadY[aLastRoadIndex] - mRoad.Height;
                }
            }

            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                mRoadY[aIndex] += mVelocityY;
            }
        }
        

        private void MoveHazard(Hazard theHazard)
        {
            theHazard.Position.Y += mVelocityY;
            if (theHazard.Position.Y > graphics.GraphicsDevice.Viewport.Height && theHazard.Visible == true)
            {
                theHazard.Visible = false;
                score += 5;
                

                mIncreaseVelocity -= 1;
                if (mIncreaseVelocity < 0)
                {
                    mIncreaseVelocity = 5;
                    //mVelocityY += 1;
                }
            }
        }

        private void MoveCoin(Coin theCoin)
        {
            theCoin.Position.Y += mVelocityY;
            if (theCoin.Position.Y > graphics.GraphicsDevice.Viewport.Height && theCoin.Visible == true)
            {
                theCoin.Visible = false;
                theCoin.isEated = true;
                existCoin = false;
            }
        }
        private void MoveNos(Nos thenos)
        {
            thenos.Position.Y += mVelocityY;
            if (thenos.Position.Y > graphics.GraphicsDevice.Viewport.Height && thenos.Visible == true)
            {
                thenos.Visible = false;
                thenos.isEated = true;
                existNos = false;
            }
        }
        private void UpdateHazards(GameTime theGameTime)
        {
            mNextHazardAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (mNextHazardAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }


                mNextHazardAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                AddHazard();
            }
        }

        private void UpdateCoins(GameTime theGameTime)
        {
            mNextAppleAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (mNextAppleAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }


                if (!existCoin)
                {
                    mNextAppleAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                    AddApple();
                    existCoin = true;
                }
            }
        }
        private void UpdateNos(GameTime theGameTime)
        {
            mNextPineappleAppearsIn -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (mNextPineappleAppearsIn < 0)
            {
                int aLowerBound = 24 - (mVelocityY * 2);
                int aUpperBound = 30 - (mVelocityY * 2);

                if (mVelocityY > 10)
                {
                    aLowerBound = 6;
                    aUpperBound = 8;
                }
                if (!existNos)
                {
                    mNextPineappleAppearsIn = (double)mRandom.Next(aLowerBound, aUpperBound) / 10;
                    AddPineapple();
                    existNos = true;
                }
            }
        }
        private void AddHazard()
        {
            int aRoadPosition = mRandom.Next(1, 8);
            int aPosition = 90 + mRandom.Next(-40, 40);
            if (aRoadPosition == 2)
            {
                aPosition = 265 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 3)
            {
                aPosition = 455 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 610 + mRandom.Next(-40, 40);
            }
            else if (aRoadPosition == 5)
            {
                aPosition = 180 + mRandom.Next(-20, 20);
            }
            else if (aRoadPosition == 6)
            {
                aPosition = 530 + mRandom.Next(-20, 20);
            }
            else if (aRoadPosition == 7)
            {
                aPosition = 360 + mRandom.Next(-20, 20);
            }
            bool aAddNewHazard = true;
            foreach (Hazard aHazard in mHazards)
            {
                if (aHazard.Visible == false)
                {
                    aAddNewHazard = false;
                    aHazard.Visible = true;
                    aHazard.Position = new Vector2(aPosition, -mHazard.Height);
                    break;
                }
            }

            if (aAddNewHazard == true)
            {
                //Add a hazard to the left side of the Road
                Hazard aHazard = new Hazard();
                aHazard.Position = new Vector2(aPosition, -mHazard.Height);

                mHazards.Add(aHazard);
            }
        }

        private void AddApple()
        {
            int aRoadPosition = mRandom.Next(1, 8);
            int aPosition = 85 - mRandom.Next(1, 11); ;
            if (aRoadPosition == 2)
            {
                aPosition = 120 + mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 3)
            {
                aPosition = 240 - mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 280 + mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 5)
            {
                aPosition = 410 - mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 6)
            {
                aPosition = 450 + mRandom.Next(1, 11);
            }
            else if (aRoadPosition == 7)
            {
                aPosition = 560 - mRandom.Next(1, 11);
            }
            bool aAddNewApple = true;
            foreach (Coin aApple in mCoins)
            {
                if (aApple.Visible == false)
                {
                    aAddNewApple = false;
                    aApple.Visible = true;
                    aApple.isEated = false;
                    aApple.Position = new Vector2(aPosition, -mCoin.Height);
                    break;
                }
            }
            if (aAddNewApple == true)
            {
                //Add a Apple to the left side of the Road
                Coin aApple = new Coin();
                aApple.Position = new Vector2(aPosition, -mCoin.Height);

                mCoins.Add(aApple);
            }
        }
        private void AddPineapple()
        {
            int aRoadPosition = mRandom.Next(1, 5);
            int aPosition = 130;
            if (aRoadPosition == 2)
            {
                aPosition = 180;
            }
            else if (aRoadPosition == 3)
            {
                aPosition = 350;
            }
            else if (aRoadPosition == 4)
            {
                aPosition = 500;
            }
            bool aAddNewPineapple = true;
            foreach (Nos aPineapple in mNoss)
            {
                if (aPineapple.Visible == false)
                {
                    aAddNewPineapple = false;
                    aPineapple.Visible = true;
                    aPineapple.isEated = false;
                    aPineapple.Position = new Vector2(aPosition, -mNos.Height);
                    break;
                }
            }
            if (aAddNewPineapple == true)
            {
                //Add a Pineapple to the left side of the Road
                Nos aPineapple = new Nos();
                aPineapple.Position = new Vector2(aPosition, -mNos.Height);

                mNoss.Add(aPineapple);
            }
        }


        private bool CheckCollision(Hazard theHazard)
        {

            BoundingBox aHazardBox = new BoundingBox(new Vector3(theHazard.Position.X, theHazard.Position.Y, 0), new Vector3(theHazard.Position.X + (mHazard.Width * .4f), theHazard.Position.Y + ((mHazard.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aHazardBox.Intersects(aCarBox) == true)
            {
                mCurrentState = State.Crash;
                mCarsRemaining -= 1;
                if (mCarsRemaining < 0)
                {
                    mCurrentState = State.GameOver;
                }
                return true;
            }

            return false;
        }
        private int CheckCoinCollision(Coin theApple)
        {

            BoundingBox aAppleBox = new BoundingBox(new Vector3(theApple.Position.X, theApple.Position.Y, 0), new Vector3(theApple.Position.X + (mCoin.Width * .4f), theApple.Position.Y + ((mCoin.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aAppleBox.Intersects(aCarBox) == true && theApple.isEated == false)
            {
                
                MoveCoin(theApple);
                theApple.Visible = false;
                theApple.isEated = true;
                existCoin = false;
                score += 20;
                //if (CarSelect == 1) if (mVelocityY >= 15) mVelocityY--;
                //if (CarSelect == 2) if (mVelocityY >= 10) mVelocityY--;
                return 1;
            }
            return 0;
        }
        private int CheckNosPosition(Nos thePineapple)
        {
            BoundingBox aPineappleBox = new BoundingBox(new Vector3(thePineapple.Position.X, thePineapple.Position.Y, 0), new Vector3(thePineapple.Position.X + (mNos.Width * .4f), thePineapple.Position.Y + ((mNos.Height - 50) * .4f), 0));
            BoundingBox aCarBox = new BoundingBox(new Vector3(mCarPosition.X, mCarPosition.Y, 0), new Vector3(mCarPosition.X + (mCar.Width * .2f), mCarPosition.Y + (mCar.Height * .2f), 0));

            if (aPineappleBox.Intersects(aCarBox) == true && thePineapple.isEated == false)
            {
                MoveNos(thePineapple);
                thePineapple.Visible = false;
                thePineapple.isEated = true;
                existNos = false;
                score += 100;
                mVelocityY++;
                return 1;
            }
            return 0;
        }

        private void ExitCountdown(GameTime theGameTime)
        {
            mExitCountDown -= theGameTime.ElapsedGameTime.TotalSeconds;
            if (mExitCountDown < 0 && mCurrentState != State.intro)
            {
                this.Exit();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(mBackground, new Rectangle(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);

            switch (mCurrentState)
            {
                case State.TitleScreen:
                    {
                        //Draw the display text for the Title screen
                        DrawTextCentered("Welcome To XNA Racing Game", 200);
                        DrawTextCentered("Press 'Space' to begin", 260);
                        //DrawTextCentered("Exit in " + ((int)mExitCountDown).ToString(), 475);
                        //ExitCountdown(gameTime);
                        break;
                    }
                case State.intro:
                    {
                        DrawTextCentered("Instructions:", 50);
                        DrawTextCentered("Collect items to increase score while avoiding obstacles.", 80);
                        DrawTextCentered("Coins will give 20 points.", 110);
                        DrawTextCentered("Nitros Oxide Systems(NOS) will give 100 points and increase speed.", 140);
                        DrawTextCentered("Player is given 5 lives, and loses one if car crashes into", 170);
                        DrawTextCentered("obstacles or drives too far off the road.", 200);
                        DrawTextCentered("===================================================", 230);
                        DrawTextCentered("Controls", 250);
                        DrawTextCentered("Use left and right arrow keys on keyboard to control car.", 280);
                        DrawTextCentered("Press M to return to main menu.", 310);
                        DrawTextCentered("===================================================", 340);
                        DrawTextCentered("Start", 370);
                        DrawTextCentered("Press A or S to select car.", 400);
                        DrawTextCentered("Car A is faster and has a higher top speed.", 430);
                        DrawTextCentered("Car S is slower and has a lower top speed.", 460);
                        break;
                    }
                default:
                    {
                        DrawRoad();
                        DrawHazards();
                        DrawCoins();
                        DrawNos();
                        if (CarSelect == 1)
                        {
                            spriteBatch.Draw(mCar, mCarPosition, new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                            spriteBatch.DrawString(mFont, "Lives:", new Vector2(100, 520), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                            for (int aCounter = 0; aCounter < mCarsRemaining; aCounter++)
                            {
                                spriteBatch.Draw(mCar, new Vector2(50 + (30 * aCounter), 550), new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.05f, SpriteEffects.None, 0);
                            }
                        }
                        else if (CarSelect == 2)
                        {
                            spriteBatch.Draw(mCar2, mCarPosition, new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
                            spriteBatch.DrawString(mFont, "Lives:", new Vector2(100, 520), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                            for (int aCounter = 0; aCounter < mCarsRemaining; aCounter++)
                            {
                                spriteBatch.Draw(mCar2, new Vector2(50 + (30 * aCounter), 550), new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, 0, new Vector2(0, 0), 0.05f, SpriteEffects.None, 0);
                            }
                        }

                        
                        spriteBatch.DrawString(mFont, "Speed: " + mVelocityY, new Vector2(100, 25), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        spriteBatch.DrawString(mFont, "Score: " + score.ToString(), new Vector2(100, 50), Color.Yellow, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                        if (mCurrentState == State.Crash)
                        {
                            DrawTextDisplayArea();
                            DrawTextCentered("Crash!", 200);
                            DrawTextCentered("Press 'Space' to continue driving.", 260);
                        }
                        else if (mCurrentState == State.GameOver)
                        {
                            DrawTextDisplayArea();
                            DrawTextCentered("Game Over!", 200);
                            spriteBatch.DrawString(mFont, "Score: " + score.ToString(), new Vector2(300, 120), Color.Yellow, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                            DrawTextCentered("Try again? Press 'A' or 'S' to select car.", 260);
                            //DrawTextCentered("Exit in " + ((int)mExitCountDown).ToString(), 400);
                            existCoin = false;
                        }

                        break;
                    }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRoad()
        {
            for (int aIndex = 0; aIndex < mRoadY.Length; aIndex++)
            {
                if (mRoadY[aIndex] > mRoad.Height * -1 && mRoadY[aIndex] <= this.Window.ClientBounds.Height)
                {
                    if (CarSelect == 1) spriteBatch.Draw(mRoad2, new Rectangle((int)((this.Window.ClientBounds.Width - mRoad.Width) / 2 - 18), mRoadY[aIndex], mRoad.Width, mRoad.Height + 5), Color.White);
                    else spriteBatch.Draw(mRoad, new Rectangle((int)((this.Window.ClientBounds.Width - mRoad.Width) / 2 - 18), mRoadY[aIndex], mRoad.Width, mRoad.Height + 5), Color.White);
                }
            }
        }

        private void DrawHazards()
        {
            foreach (Hazard aHazard in mHazards)
            {
                if (aHazard.Visible == true)
                {
                    spriteBatch.Draw(mHazard, aHazard.Position, new Rectangle(0, 0, mHazard.Width, mHazard.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawCoins()
        {
            foreach (Coin aApple in mCoins)
            {
                if (aApple.Visible == true)
                {
                    spriteBatch.Draw(mCoin, aApple.Position, new Rectangle(0, 0, mCoin.Width, mCoin.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }
        private void DrawNos()
        {
            foreach (Nos aPineapple in mNoss)
            {
                if (aPineapple.Visible == true)
                {
                    spriteBatch.Draw(mNos, aPineapple.Position, new Rectangle(0, 0, mNos.Width, mNos.Height), Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                }
            }
        }
        private void DrawTextDisplayArea()
        {
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (450 / 2));
            spriteBatch.Draw(mBackground, new Rectangle(aPositionX, 75, 450, 400), Color.White);
        }

        private void DrawTextCentered(string theDisplayText, int thePositionY)
        {
            Vector2 aSize = mFont.MeasureString(theDisplayText);
            int aPositionX = (int)((graphics.GraphicsDevice.Viewport.Width / 2) - (aSize.X / 2));

            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX, thePositionY), Color.Green, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mFont, theDisplayText, new Vector2(aPositionX + 1, thePositionY + 1), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}