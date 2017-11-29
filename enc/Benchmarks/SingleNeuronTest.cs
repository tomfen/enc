using enc.Utils;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using OpenCvSharp;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc.Benchmarks
{
    class SingleNeuronTest : IExperiment
    {
        public string Command => "v";

        public string Name => "Wizualizacja przestrzeni błędu";

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

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 1));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            new RangeRandomizer(-4, 4).Randomize(network, 0);

            var algorithms = new Propagation[] {
                //new Backpropagation((BasicNetwork)network.Clone(), trainingSet, 1, 0),
                //new Backpropagation((BasicNetwork)network.Clone(), trainingSet, 1, .5),
                //new QuickPropagation((BasicNetwork)network.Clone(), trainingSet),
                //new ManhattanPropagation((BasicNetwork)network.Clone(), trainingSet, 0.3),
                //new ScaledConjugateGradient((BasicNetwork)network.Clone(), trainingSet),
                new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType= RPROPType.iRPROPm},
                new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType= RPROPType.iRPROPp},
                //new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType= RPROPType.RPROPm},
                //new ResilientPropagation((BasicNetwork)network.Clone(), trainingSet) { RType= RPROPType.RPROPp},
            };

            Point<double>[,] results = new Point<double>[epochs+1, algorithms.Length];


            for (int i = 0; i < algorithms.Length; i++)
            {
                var train = algorithms[i];
                results[0, i].X = train.Network.Flat.Weights[0];
                results[0, i].Y = train.Network.Flat.Weights[1];
                for (int epoch = 1; epoch <= epochs; epoch++)
                {
                    train.Iteration();
                    results[epoch, i].X = train.Network.Flat.Weights[0];
                    results[epoch, i].Y = train.Network.Flat.Weights[1];
                }
                
                train.FinishTraining();
            }

            new ErrorSpaceVisualizer(results, network, trainingSet, algorithms).ShowDialog();

            if (options.ContainsKey("s"))
                CSVWriter.Save(options["s"], results, algorithms);
        }
    }
}
