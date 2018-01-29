using Encog.Engine.Network.Activation;
using Encog.ML.Data.Specific;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace enc.Benchmarks
{
    class ErrorTest : IExperiment
    {
        public string Command => "e";

        public string Name => "Wykres błędu uczenia";

        public string Description => "Wykreśla wykres błędu od iteracji na przykładzie zbioru Iris.\n" +
            "-e int: liczba iteracji, domyślnie 50.\n" + 
            "-s string: jeżeli podano, to zapisuje wynik do pliku w formacie .csv.";
        
        public void Run(Dictionary<string, string> options)
        {
            int epochs = ExperimentOptions.getParameterInt(options, "e", 50);
            
            CSVMLDataSet trainingSet = new CSVMLDataSet(@"..\..\..\..\DataSets\iris-norm.csv", 4, 3, true);
            
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 4));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 10));
            network.AddLayer(new BasicLayer(new ActivationTANH(), false, 3));
            network.Structure.FinalizeStructure();
            network.Reset();

            var algorithms = new BasicTraining[]
            {
                new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType = RPROPType.iRPROPp },
                new QuickPropagation((BasicNetwork)network.Clone(), trainingSet),
                new Backpropagation((BasicNetwork)network.Clone(), trainingSet, 0.01, 0),
                new Backpropagation((BasicNetwork)network.Clone(), trainingSet, 0.005, 0),
            };

            double[,] results = new double[epochs + 1, algorithms.Length];

            for (int i = 0; i < algorithms.Length; i++)
                results[0, i] = network.CalculateError(trainingSet);

            for (int i = 0; i < algorithms.Length; i++)
            {
                Console.WriteLine("Algorytm: " + algorithms[i].GetType().Name );

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int epoch = 1; epoch<=epochs; epoch++)
                {
                    algorithms[i].Iteration();
                    results[epoch, i] = algorithms[i].Error;
                }

                algorithms[i].FinishTraining();

                stopwatch.Stop();

                Console.WriteLine("Czas: " + stopwatch.ElapsedMilliseconds + "ms");
            }

            new ErrorGraph(results, algorithms).ShowDialog();

            if (options.ContainsKey("s"))
                CSVWriter.Save(options["s"], results, algorithms);
        }
    }
}
