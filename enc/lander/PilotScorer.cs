using Encog.Neural.Networks.Training;
using Encog.ML;
using Encog.Neural.Networks;

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

            int stepsNotCrashed = 0;

            while (!sim.IsComplete())
            {
                sim.Step();
                stepsNotCrashed++;
            }

            return (double) stepsNotCrashed*10 - sim.maxXAbs - sim.minY
                - (sim.lander.LeftLegBroken ? 1000 : 0)
                - (sim.lander.RightLegBroken ? 1000 : 0);
        }
    }
}
