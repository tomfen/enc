using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Utilities;

namespace enc.lander
{
    class NeuralPilot : ILanderPilot
    {
        BasicNetwork network;
        World world;
        private Lander _lander;

        public NeuralPilot(BasicNetwork network, World world, Lander lander)
        {
            this.network = network;
            this.world = world;
            this._lander = lander;
        }

        public Lander Lander => _lander;

        public void Process()
        {
            var input = new BasicMLData(2);
            input[0] = Lander.Vessel.Position.X;
            input[1] = Lander.Vessel.Position.Y;
            IMLData output = network.Compute(input);

            Lander.ThrustLeft((float)output[0]);
            Lander.ThrustLeft((float)output[1]);
        }
    }
}
