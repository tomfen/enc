using System;
using System.Linq;
using VelcroPhysics.DebugViews;
using VelcroPhysics;
using MonoGame;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.DebugViews.MonoGame;
using VelcroPhysics.Factories;
using Microsoft.Xna.Framework.Input;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using VelcroPhysics.Extensions.DebugView;

namespace enc.lander
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private DebugView _debugView;

        private Texture2D flame;

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
            flame = Content.Load<Texture2D>("flame");


            _debugView = new DebugView(landerSimulation.world);
            _debugView.LoadContent(_graphics.GraphicsDevice, this.Content);
            /*_debugView.AppendFlags(
                DebugViewFlags.PerformanceGraph |
                DebugViewFlags.DebugPanel |
                DebugViewFlags.Controllers);*/
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

            _debugView.BeginCustomDraw(projection, Matrix.Identity);
            _debugView.DrawSolidCircle(landerSimulation.lander.Vessel.WorldCenter, 5, new Vector2(), Color.CadetBlue);
            _debugView.EndCustomDraw();

            _spriteBatch.DrawString(font, landerSimulation.lander.IsLanded().ToString(), new Vector2(), Color.White);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
    }
}