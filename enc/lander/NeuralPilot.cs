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
    class NeuralPilot : LanderPilot
    {
        BasicNetwork network;
        
        public NeuralPilot(BasicNetwork network) : base()
        {
            this.network = network;
        }
        
        public override void Process()
        {
            var input = new BasicMLData(6);
            input[0] = Lander.Vessel.Position.X/4;
            input[1] = Lander.Vessel.Rotation/2;
            input[2] = Lander.Vessel.LinearVelocity.X;
            input[3] = Lander.Vessel.LinearVelocity.Y;
            input[4] = Lander.Vessel.AngularVelocity;
            input[5] = (Lander.Vessel.Position.Y + 3) / 6;

            IMLData output = network.Compute(input);

            Lander.ThrustLeft((float)output[0]);
            Lander.ThrustRight((float)output[1]);
            Lander.ThrustUp((float)output[2]);

        }
    }
}
