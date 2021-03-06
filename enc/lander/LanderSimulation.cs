﻿using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;

namespace enc.lander
{
    public class LanderSimulation
    {
        public Body floor;
        public World world;

        private float LanderTilt = -1.0f;

        public Lander lander;
        public LanderPilot pilot;
        
        public LanderSimulation(LanderPilot pilot)
        {
            world = new World(new Vector2(0, 9.8f));

            floor = BodyFactory.CreateEdge(world, new Vector2(-100, 9), new Vector2(100, 9));
            
            this.pilot = pilot;

            Reset();
        }
        
        public void Reset()
        {
            if(lander != null)
                lander.Destroy();
            lander = new Lander(world, new Vector2(0f, -5), LanderTilt);

            pilot.Lander = lander;
        }

        public void Step()
        {
            pilot.Process();
            world.Step(1f / 60f);
        }
        
    }
}
