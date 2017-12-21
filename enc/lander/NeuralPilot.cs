using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NEAT;
using Encog.Neural.Networks;
using Encog.Util.Arrayutil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Utilities;

namespace enc.lander
{
    class NeuralPilot : LanderPilot
    {
        NEATNetwork network;

        NormalizedField Altitude = new NormalizedField(NormalizationAction.Normalize, "alt", 0, -10, 0.9, -0.9);

        public NeuralPilot(NEATNetwork network) : base()
        {
            this.network = network;
        }
        
        public override void Process()
        {
            var input = new BasicMLData(7);
            input[0] = Lander.Vessel.LinearVelocity.Y;
            input[1] = Lander.Vessel.LinearVelocity.X;

            input[2] = Altitude.Normalize(Lander.Vessel.WorldCenter.Y);
            input[3] = Lander.Vessel.WorldCenter.X;

            input[4] = Lander.Vessel.Rotation;
            input[5] = Lander.Vessel.AngularVelocity;

            input[6] = Lander.IsLanded() ? 1 : -1;

            IMLData output = network.Compute(input);

            Lander.ThrustLeft((float)output[0]);
            Lander.ThrustRight((float)output[1]);
            Lander.ThrustUp((float)output[2]);

        }
    }
}
