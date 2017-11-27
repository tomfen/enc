using enc.mnist;
using Encog.App.Analyst;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Wizard;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Util.CSV;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace enc.Benchmarks
{
    class BatchTest : IExperiment
    {
        public string Command => "b";

        public string Name => "Test wielkości batch";

        public string Description => throw new NotImplementedException();

        public string Options => throw new NotImplementedException();

        public void Run(Dictionary<string, string> options)
        {
            int epochs = ExperimentOptions.getParameterInt(options, "e", 50);
            double learningRate = ExperimentOptions.getParameterDouble(options, "lr", 0.0001);
            int[] batchSizes = ExperimentOptions.getParameterIntArray(options, "b", new int[] { 0, 1, 10 });

            double[,] results = new double[epochs, batchSizes.Length];

            CSVMLDataSet trainingSet = new CSVMLDataSet(@"..\..\..\..\..\DataSets\iris-norm.csv", 4, 3, true);
            
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 4));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), true, 10));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), false, 3));
            network.Structure.FinalizeStructure();
            network.Reset();

            for (int i = 0; i < batchSizes.Length; i++)
            {
                Console.WriteLine("Batch: " + batchSizes[i] );
                

                var train = new Backpropagation((BasicNetwork)network.Clone(), trainingSet)
                {
                    LearningRate = learningRate,
                    BatchSize = batchSizes[i]
                };

                Console.WriteLine(network.CalculateError(trainingSet));
                
                for (int epoch = 1; epoch<=epochs; epoch++)
                {
                    train.Iteration();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch + " Error:" + train.Error);
                    results[epoch-1, i] = train.Error;
                }

                train.FinishTraining();
                
            }

            if (options.ContainsKey("s"))
                CSVWriter.Save(options["s"], results, batchSizes);
        }
    }
}
