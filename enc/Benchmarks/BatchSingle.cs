using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc.Benchmarks
{
    class BatchSingle : IExperiment
    {
        

        public string Command => "v";

        public string Name => "";

        public string Description => throw new NotImplementedException();

        public string Options => throw new NotImplementedException();

        public void Run(Dictionary<string, string> options)
        {
            
            double[][] input =
            {
                new double[]{2},
                new double[]{0},
                new double[]{2},
                new double[]{2.1},
            };

            double[][] output =
            {
                new double[]{0.95},
                new double[]{0.5},
                new double[]{0.10},
                new double[]{0.099},
            };

            var trainingSet = new BasicMLDataSet(input, output);
            int epochs = ExperimentOptions.getParameterInt(options, "e", 50);
            double learningRate = ExperimentOptions.getParameterDouble(options, "lr", 0.01);

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 1));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();
            network.Flat.Weights[0] = 2;
            network.Flat.Weights[1] = 4;

            int[] batchSizes = ExperimentOptions.getParameterIntArray(options, "b", new int[] { 0, 1 });
            double[,] results = new double[epochs, 2 * batchSizes.Length];


            for (int i = 0; i < batchSizes.Length; i++)
            {
                Propagation train = new Backpropagation((BasicNetwork)network.Clone(), trainingSet)
                {
                    LearningRate = learningRate,
                    BatchSize = batchSizes[i]
                };

                for (int epoch = 1; epoch <= epochs; epoch++)
                {
                    train.Iteration();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch + " Error:" + train.Error);
                    results[epoch - 1, 2*i] = train.Network.Flat.Weights[0];
                    results[epoch - 1, 2*i+1] = train.Network.Flat.Weights[1];
                }
                
                train.FinishTraining();
            }

            if (options.ContainsKey("s"))
                CSVWriter.Save(options["s"], results, batchSizes);
        }
    }
}
