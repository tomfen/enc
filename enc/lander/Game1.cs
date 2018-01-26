using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.DebugViews.MonoGame;
using Microsoft.Xna.Framework.Input;

namespace enc.lander
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private DebugView _debugView;

        public LanderSimulation landerSimulation;
        
        private float _X;
        private float _Y;
        private float _Z = 1;

        public Game1(LanderSimulation sim)
        {
            _graphics = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferHeight = 600,
                                PreferredBackBufferWidth = 800,
                                IsFullScreen = false,
                            };
            
            Content.RootDirectory = "Content";
            
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0/60.0);

            landerSimulation = sim;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            
            font = Content.Load<SpriteFont>("font");


            _debugView = new DebugView(landerSimulation.world);
            _debugView.LoadContent(_graphics.GraphicsDevice, this.Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                landerSimulation.Reset();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _X += 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _X -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _Y -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _Y += 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                _Z -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                _Z += 0.01f;

            landerSimulation.Step();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 255));

            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height),
                0f, 0f, 1f);
            

            _spriteBatch.Begin();
            projection.Translation = new Vector3(_X, _Y, 0);

            _debugView.RenderDebugData(ref projection);

            _spriteBatch.DrawString(font, "Silnik dolny: " + landerSimulation.lander?.ThrustUpValue, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(font, "Silnik lewy:  " + landerSimulation.lander?.ThrustLeftValue, new Vector2(0, 15), Color.White);
            _spriteBatch.DrawString(font, "Silnik prawy: " + landerSimulation.lander?.ThrustRightValue, new Vector2(0, 30), Color.White);
            _spriteBatch.DrawString(font, "Paliwo: " + landerSimulation.lander?.fuel, new Vector2(150, 0), Color.White);
            _spriteBatch.DrawString(font, "Uszkodzenie: " + landerSimulation.lander?.damage, new Vector2(150, 15), Color.White);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
    }
}