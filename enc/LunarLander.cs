using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using System;

namespace enc.lunar
{
    public class LunarLander
    {
        public static void run()
        {
            new Game1().Run();

            BasicNetwork network = CreateNetwork();

            IMLTrain train;

            /*if (app.Args.Length > 0 && String.Compare(app.Args[0], "anneal", true) == 0)
            {
                train = new NeuralSimulatedAnnealing(
                    network, new PilotScore(), 10, 2, 100);
            }
            else*/
            {
                train = new MLMethodGeneticAlgorithm( ()=>{
					BasicNetwork result = CreateNetwork();
					((IMLResettable)result).Reset();
					return result;
				},new PilotScore(),5000);
            }

            for (int epoch = 1; epoch <= 50; epoch++)
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error);
            }

            Console.WriteLine(@"\nHow the winning network landed:");
            network = (BasicNetwork) train.Method;
            var pilot = new NeuralPilot(network, true);
            Console.WriteLine(pilot.ScorePilot());
        }

        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 3};
            pattern.AddHiddenLayer(10);
            pattern.OutputNeurons = 1;
            pattern.ActivationFunction = new ActivationTANH();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}
