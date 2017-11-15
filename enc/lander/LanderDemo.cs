using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace enc.lander
{
    public class LanderDemo : IExperiment
    {
        public string Command => "l";

        public string Name => "Symulacja l¹downika";

        public string Options => "";

        public string Description => "";

        public void Run(Dictionary<string, string> options)
        {
            LanderPilot pilot;

            if (options.ContainsKey("k"))
            {
                pilot = new KeyboardPilot();
            }
            else
            {
                BasicNetwork network = CreateNetwork();

                IMLTrain train = new MLMethodGeneticAlgorithm(() =>
                {
                    BasicNetwork result = CreateNetwork();
                    ((IMLResettable)result).Reset();
                    return result;
                }, new PilotScorer(), 200);
                
                for (int epoch = 1; epoch <= 30; epoch++)
                {
                    train.Iteration();
                    Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error);

                    if (Keyboard.GetState().IsKeyDown(Keys.Z))
                        break;
                }
                pilot = new NeuralPilot(network);
            }

            var sim = new LanderSimulation(pilot);
            var game = new Game1(sim);
            game.Run();
        }

        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 6};
            pattern.AddHiddenLayer(10);
            pattern.OutputNeurons = 3;
            pattern.ActivationFunction = new ActivationTANH();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}
