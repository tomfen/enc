using Encog.Neural.Networks.Training;
using Encog.ML;
using Encog.Neural.NEAT;
using System;

namespace enc.lander
{
    class PilotScorer : ICalculateScore
    {

        public bool ShouldMinimize => false;

        public bool RequireSingleThreaded => true;
        
        public double CalculateScore(IMLMethod network)
        {
            var pilot = new NeuralPilot((NEATNetwork)network);
            var sim = new LanderSimulation(pilot);
            
            int steps = 0;

            double r = 0;
            double minY = 0;

            while (steps < 60 * 15)
            {
                sim.Step();
                steps++;

                r += Math.Abs(sim.lander.Vessel.Rotation);

                minY = Math.Min(0, sim.lander.WorldCenter.Y - 10);
            }



            return sim.lander.WorldCenter.Y < 10 ?
                    - r/10 - sim.lander.damage:
                    float.MinValue;

        }
        
    }
}
