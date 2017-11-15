using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;

namespace enc.lander
{
    public class LanderSimulation
    {
        public Body floor;
        public World world;
        public Lander lander;
        public LanderPilot pilot;
        private bool touchedGround;
        public float maxYVel = 0;
        public float maxAVel = 0;
        public float minY = 0;
        public float maxA = 0;
        public float maxXAbs = 0;
        private int stepsSinceTouchdown = 0;

        public bool ShouldMinimize => false;

        public bool RequireSingleThreaded => true;

        public LanderSimulation(LanderPilot pilot)
        {
            world = new World(new Vector2(0, 9.8f));

            floor = BodyFactory.CreateEdge(world, new Vector2(-10, 9), new Vector2(10, 9));
            lander = new Lander(world, new Vector2(0f, 0));

            this.pilot = pilot;
            pilot.Lander = lander;

            floor.OnCollision += Floor_OnCollision;
        }

        private void Floor_OnCollision(Fixture fixtureA, Fixture fixtureB, VelcroPhysics.Collision.ContactSystem.Contact contact)
        {
            touchedGround = true;
        }

        public void Reset()
        {
            lander.Destroy();
            lander = new Lander(world, new Vector2(0f, 0));
            pilot.Lander = lander;
            touchedGround = false;
            maxAVel = 0;
            maxYVel = 0;
        }

        public void Step()
        {
            pilot.Process();
            world.Step(1f / 60f);

            if (touchedGround)
                stepsSinceTouchdown++;

            maxYVel = Math.Max(maxYVel, lander.Vessel.LinearVelocity.Y);
            maxAVel = Math.Max(maxAVel, Math.Abs(lander.Vessel.AngularVelocity));
            minY = Math.Min(minY, lander.Vessel.Position.Y);
            maxA = Math.Max(maxA, Math.Abs(lander.Vessel.Rotation));
            maxXAbs = Math.Max(maxXAbs, Math.Abs(lander.Vessel.Position.X));
        }

        public bool IsComplete()
        {
            return (lander.fuel<=0 || stepsSinceTouchdown > 10 || lander.Vessel.WorldCenter.Y  > 20); 
        }
    }
}
