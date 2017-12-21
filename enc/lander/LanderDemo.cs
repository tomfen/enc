using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.EA.Train;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.NEAT;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
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

            int population = options.ContainsKey("p") ? int.Parse(options["p"]) : 100;
            int epochs = options.ContainsKey("e") ? int.Parse(options["e"]) : 30;
            bool showImprovements = options.ContainsKey("i");

            if (options.ContainsKey("k"))
            {
                pilot = new KeyboardPilot();
            }
            else
            {
                NEATPopulation pop = new NEATPopulation(6, 3, population);
                pop.Reset();
                pop.InitialConnectionDensity = 1.0; // not required, but speeds processing.
                ICalculateScore score = new PilotScorer();
                // train the neural network
                TrainEA train = NEATUtil.ConstructNEATTrainer(pop, score);
                
                NEATNetwork network = (NEATNetwork)train.CODEC.Decode(train.BestGenome);
                
                Console.WriteLine("Rozpoczynanie uczenia...");

                double best = double.MinValue;
                for (int epoch = 1; epoch <= epochs; epoch++)
                {
                    train.Iteration();
                    Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error);

                    if(train.Error > best)
                    {
                        best = train.Error;
                        
                        if (showImprovements)
                        {
                            NEATNetwork network1 = ((NEATPopulation)train.Method).BestNetwork;
                            var pilot1 = new NeuralPilot(network1);
                            var sim1 = new LanderSimulation(pilot1);
                            var game1 = new Game1(sim1);
                            game1.Run();
                        }
                    }
                }
                
                pilot = new NeuralPilot(((NEATPopulation)train.Method).BestNetwork);
            }

            var sim = new LanderSimulation(pilot);
            var game = new Game1(sim);
            game.Run();
        }

        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 6};
            pattern.AddHiddenLayer(20);
            pattern.OutputNeurons = 3;
            pattern.ActivationFunction = new ActivationTANH();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}
