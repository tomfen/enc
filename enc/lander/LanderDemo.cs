using Encog.Engine.Network.Activation;
using Encog.ML.EA.Train;
using Encog.Neural.NEAT;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Pattern;
using System;
using System.Collections.Generic;

namespace enc.lander
{
    public class LanderDemo : IExperiment
    {
        public string Command => "l";

        public string Name => "Symulacja l¹downika";
        
        public string Description => "Trenowanie sieci neuronowej za pomoc¹ algorytmu ewolucyjnego na przykladzie prostej gry.\n" +
            "-p int: poczatkowy rozmiar populacji. Domyslnie 500.\n" +
            "-e int: liczba iteracji, Domyslnie 30.\n" +
            "-i: jezeli podano, to pokazywane jest ka¿de polepszenie wyniku.\n" +
            "-k: jezeli podano, to ladownik jest sterowany przez klawiature.";

        public void Run(Dictionary<string, string> options)
        {
            LanderPilot pilot;

            int population = ExperimentOptions.getParameterInt(options, "p", 500);
            int epochs = ExperimentOptions.getParameterInt(options, "e", 30);
            bool showImprovements = options.ContainsKey("i");

            if (options.ContainsKey("k"))
            {
                pilot = new KeyboardPilot();
            }
            else
            {
                NEATPopulation pop = new NEATPopulation(7, 3, population)
                {
                    InitialConnectionDensity = 0.5,
                    ActivationCycles = 1
                };
                pop.Reset();
                ICalculateScore score = new PilotScorer();

                TrainEA train = NEATUtil.ConstructNEATTrainer(pop, score);
                
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
