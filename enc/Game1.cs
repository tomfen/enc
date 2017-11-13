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

namespace enc.lunar
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
        private float _timer;
        private World _world;
        private DebugView _debugView;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferHeight = 800,
                                PreferredBackBufferWidth = 480,
                                IsFullScreen = false
                            };

            Content.RootDirectory = "Content";

            // Frame rate is fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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
                _world = new World(Vector2.UnitY * 10);
            }
            else
            {
                _world.Clear();
            }

            _debugView = new DebugView(_world);
            _debugView.LoadContent(_graphics.GraphicsDevice, this.Content);
            _debugView.AppendFlags(VelcroPhysics.Extensions.DebugView.DebugViewFlags.PerformanceGraph);

            // Create and position our floor
            _floor = BodyFactory.CreateRectangle(
                _world,
                ConvertUnits.ToSimUnits(480),
                ConvertUnits.ToSimUnits(50),
                10f, bodyType:BodyType.Static);
            _floor.Position = ConvertUnits.ToSimUnits(240, 605);
            _floor.Restitution = 0.2f;
            _floor.Friction = 0.2f;
           
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // Create a random box every second
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= 1.0f)
            {
                // Reset our timer
                _timer = 0f;

                // Determine a random size for each box
                var random = new Random();
                var width = random.Next(20, 100);
                var height = random.Next(20, 100);

                // Create it and store the size in the user data
                var box = BodyFactory.CreateRectangle(
                    _world,
                    ConvertUnits.ToSimUnits(width),
                    ConvertUnits.ToSimUnits(height),
                    10f,
                    new Vector2(width, height));

                box.BodyType = BodyType.Dynamic;
                box.Restitution = 0.2f;
                box.Friction = 0.2f;

                // Randomly pick a location along the top to drop it from
                box.Position = ConvertUnits.ToSimUnits(random.Next(50, 400), 0);
            }

            // Advance all the elements in the world
            _world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));

            // Clean up any boxes that have fallen offscreen
            foreach (var box in from box in _world.BodyList
                                let pos = ConvertUnits.ToDisplayUnits(box.Position)
                                where pos.Y > _graphics.GraphicsDevice.Viewport.Height
                                select box)
            {
                _world.RemoveBody(box);
            }

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