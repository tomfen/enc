using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NEAT;
using Encog.Util.Arrayutil;

namespace enc.lander
{
    class NeuralPilot : LanderPilot
    {
        NEATNetwork network;

        NormalizedField Altitude = new NormalizedField(NormalizationAction.Normalize, "alt", 0, -10, 1, -1);
        NormalizedField Latitude = new NormalizedField(NormalizationAction.Normalize, "lat", 5, -5, 1, -1);
        NormalizedField Angle = new NormalizedField(NormalizationAction.Normalize, "ang", 5, -5, 1, -1);

        public NeuralPilot(NEATNetwork network) : base()
        {
            this.network = network;
        }
        
        public override void Process()
        {
            var input = new BasicMLData(7);
            input[0] = Lander.Vessel.LinearVelocity.Y;
            input[1] = Lander.Vessel.LinearVelocity.X;

            input[2] = Altitude.Normalize(Lander.WorldCenter.Y);
            input[3] = Latitude.Normalize(Lander.WorldCenter.X);

            input[4] = Angle.Normalize(Lander.Vessel.Rotation);
            input[5] = Lander.Vessel.AngularVelocity;

            input[6] = Lander.IsLanded() ? 1 : 0;

            IMLData output = network.Compute(input);

            Lander.ThrustLeft((float)output[0] * 2 - 1);
            Lander.ThrustRight((float)output[1] * 2 - 1);
            Lander.ThrustUp((float)output[2] * 2 - 1);

        }
    }
}
