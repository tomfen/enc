using Encog.Neural.Networks.Training;
using Encog.ML;
using Encog.Neural.NEAT;

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
            int stepsLanded = 0;

            float _x = 10000;
            bool _xt = false;

            while (steps < 60 * 15 && stepsLanded < 60)// && !sim.lander.IsCrashed())
            {
                sim.Step();
                steps++;

                if (sim.lander.Vessel.WorldCenter.Y > 5)
                    if (!_xt)
                        _xt = true;
                        _x = sim.lander.Vessel.WorldCenter.X;

                if (sim.lander.IsLanded())
                    stepsLanded++;
                else
                    stepsLanded = 0;
            }



            return sim.lander.Vessel.WorldCenter.Y < 10 ?
                sim.lander.IsCrashed() ?
                    -100000 + steps :
                    -sim.lander.damage + sim.lander.fuel/100:
                float.MinValue;

        }
        
    }
}
