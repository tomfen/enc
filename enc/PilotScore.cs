using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;

namespace enc.lunar
{
    public class PilotScore : ICalculateScore
    {
        public double CalculateScore(IMLMethod network)
        {
            NeuralPilot pilot = new NeuralPilot((BasicNetwork)network, false);
            return pilot.ScorePilot();
        }


        public bool ShouldMinimize
        {
            get { return false; }
        }


        /// <inheritdoc/>
        public bool RequireSingleThreaded
        {
            get { return false; }
        }
    }
}
