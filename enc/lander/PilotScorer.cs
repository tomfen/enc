using Encog.Neural.Networks.Training;
using Encog.ML;
using Encog.Neural.Networks;
using Microsoft.Xna.Framework;
using System;
using VelcroPhysics.Dynamics;

namespace enc.lander
{
    class PilotScorer : ICalculateScore
    {

        public bool ShouldMinimize => false;

        public bool RequireSingleThreaded => true;
        
        public double CalculateScore(IMLMethod network)
        {
            var pilot = new NeuralPilot((BasicNetwork)network);
            var sim = new LanderSimulation(pilot);
            
            int steps = 0;
            int stepsLanded = 0;
            
            while (steps < 60 * 15 && stepsLanded < 60 && !sim.lander.IsCrashed())
            {
                sim.Step();
                steps++;

                if (sim.lander.IsLanded())
                    stepsLanded++;
                else
                    stepsLanded = 0;
            }



            return sim.lander.Vessel.WorldCenter.Y < 10 ?
                sim.lander.IsCrashed() ?
                    -100000 + steps :
                    -sim.lander.damage - steps :
                float.MinValue;

        }
        
    }
}
