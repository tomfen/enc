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

namespace enc.lander
{
    public static class ConvertUnits
    {
        private static float _displayUnitsToSimUnitsRatio = 100f;
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
        private Body _floor;
        private SpriteBatch _spriteBatch;
        private World _world;
        private DebugView _debugView;
        private Lander lander;

        public ILanderPilot Pilot;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferHeight = 800,
                                PreferredBackBufferWidth = 600,
                                IsFullScreen = false,
                            };
            
            Content.RootDirectory = "Content";
            
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0/60.0);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            // Load our font (DebugViewXNA needs it for the DebugPanel)
            Content.Load<SpriteFont>("font");

            // Create our World with a gravity of vertical units
            if (_world == null)
            {
                _world = new World(new Vector2(0, 2));
            }
            else
            {
                _world.Clear();
            }

            _debugView = new DebugView(_world);
            _debugView.LoadContent(_graphics.GraphicsDevice, this.Content);
            _debugView.AppendFlags(
                VelcroPhysics.Extensions.DebugView.DebugViewFlags.PerformanceGraph |
                VelcroPhysics.Extensions.DebugView.DebugViewFlags.DebugPanel |
                VelcroPhysics.Extensions.DebugView.DebugViewFlags.Controllers);
            
            _floor = BodyFactory.CreateEdge(_world, new Vector2(0, 5), new Vector2(5, 5));

            lander = new Lander(_world, new Vector2(1.5f, 0));

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 2));
            network.Structure.FinalizeStructure();
            network.Reset();

            Pilot = new NeuralPilot(network, _world, lander);//KeyboardPilot(lander);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                lander.Destroy();
                lander = new Lander(_world, new Vector2(1.5f, 0));
            }

            Pilot.Process();
            _world.Step(1f / 60f);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 0, 0, 255));

            _spriteBatch.Begin();

            var projection = Matrix.CreateOrthographicOffCenter(
                0f,
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height), 0f, 0f,
                1f);
            _debugView.RenderDebugData(ref projection);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        
    }
}