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
    public static class ConvertUnits
    {
        private static float _displayUnitsToSimUnitsRatio = 30f;
        private static float _simUnitsToDisplayUnitsRatio = 1 / _displayUnitsToSimUnitsRatio;

        public static void SetDisplayUnitToSimUnitRatio(float displayUnitsPerSimUnit)
        {
            _displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
            _simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
        }

        public static float ToDisplayUnits(float simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static float ToDisplayUnits(int simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(ref Vector2 simUnits, out Vector2 displayUnits)
        {
            Vector2.Multiply(ref simUnits, _displayUnitsToSimUnitsRatio, out displayUnits);
        }

        public static Vector3 ToDisplayUnits(Vector3 simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static Vector2 ToDisplayUnits(float x, float y)
        {
            return new Vector2(x, y) * _displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(float x, float y, out Vector2 displayUnits)
        {
            displayUnits = Vector2.Zero;
            displayUnits.X = x * _displayUnitsToSimUnitsRatio;
            displayUnits.Y = y * _displayUnitsToSimUnitsRatio;
        }

        public static float ToSimUnits(float displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static float ToSimUnits(double displayUnits)
        {
            return (float)displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static float ToSimUnits(int displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static Vector2 ToSimUnits(Vector2 displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static Vector3 ToSimUnits(Vector3 displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(ref Vector2 displayUnits, out Vector2 simUnits)
        {
            Vector2.Multiply(ref displayUnits, _simUnitsToDisplayUnitsRatio, out simUnits);
        }

        public static Vector2 ToSimUnits(float x, float y)
        {
            return new Vector2(x, y) * _simUnitsToDisplayUnitsRatio;
        }

        public static Vector2 ToSimUnits(double x, double y)
        {
            return new Vector2((float)x, (float)y) * _simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(float x, float y, out Vector2 simUnits)
        {
            simUnits = Vector2.Zero;
            simUnits.X = x * _simUnitsToDisplayUnitsRatio;
            simUnits.Y = y * _simUnitsToDisplayUnitsRatio;
        }
    }

    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private DebugView _debugView;

        public LanderSimulation landerSimulation;
        
        private float _X;
        private float _Y;

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

            // Load our font (DebugViewXNA needs it for the DebugPanel)
            font = Content.Load<SpriteFont>("font");


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

            landerSimulation.Step();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 255));

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, landerSimulation.lander.Vessel.Position.ToString(), new Vector2(0, 0), Color.Blue);
            _spriteBatch.DrawString(font, landerSimulation.lander.Vessel.Rotation.ToString(), new Vector2(0, 15), Color.Blue);
            _spriteBatch.DrawString(font, landerSimulation.lander.Vessel.AngularVelocity.ToString(), new Vector2(0, 30), Color.Green);
            _spriteBatch.DrawString(font, landerSimulation.lander.Vessel.LinearVelocity.ToString(), new Vector2(0, 45), Color.Green);

            _spriteBatch.DrawString(font, landerSimulation.maxAVel.ToString(), new Vector2(200, 0), Color.Yellow);
            _spriteBatch.DrawString(font, landerSimulation.maxYVel.ToString(), new Vector2(200, 15), Color.Yellow);

            Matrix projection = Matrix.CreateOrthographicOffCenter( 0,
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height),
                0f, 0f, 1f);

            projection.Translation = new Vector3(_X, _Y, 0);

            _debugView.RenderDebugData(ref projection);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
    }
}