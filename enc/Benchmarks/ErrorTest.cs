using Encog.Engine.Network.Activation;
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace enc.Benchmarks
{
    class ErrorTest : IExperiment
    {
        public string Command => "e";

        public string Name => "Test błędu uczenia";

        public string Description => throw new NotImplementedException();
        
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

            var algorithms = new Propagation[]
            {
                //new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType = RPROPType.RPROPp },
                //new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType = RPROPType.iRPROPp },
                new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet),
                new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) {ErrorFunction=new reuters.MultilabelErrorFunction() },
            };

            double[,] results = new double[epochs + 1, algorithms.Length];

            for (int i = 0; i < algorithms.Length; i++)
                results[0, i] = network.CalculateError(trainingSet);

            for (int i = 0; i < algorithms.Length; i++)
            {
                Console.WriteLine("Algorytm: " + algorithms[i].GetType().ToString() );

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
