using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Propagation.SGD;
using Encog.Neural.NeuralData;
using Encog.Persist;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Encog.Util.Arrayutil;
using Encog.ML.Factory;
using Encog.Util.Normalize;
using Encog.ML.Data.Versatile;
using Encog.Neural.Networks.Training.Propagation;
using System.Runtime.InteropServices;
using enc.mnist;
using Encog.ML.Train.Strategy;
using Encog.ML.Train.Strategy.End;
using Encog.Neural.Error;

namespace enc.mnist
{
    class MnistDemo : IExperiment
    {
        public string Command => "m";

        public string Description => "";

        public string Options => "";

        public string Name => "Klasyfikacja zbioru MNIST";

        public void Run(Dictionary<string, string> options)
        {
            BasicMLDataSet trainingSet = LoadDataSet(@"..\..\..\..\..\DataSets\train-images.idx3-ubyte",
                                                     @"..\..\..\..\..\DataSets\train-labels.idx1-ubyte", 0, 1);
            BasicMLDataSet validationSet = LoadDataSet(@"..\..\..\..\..\DataSets\t10k-images.idx3-ubyte",
                                                       @"..\..\..\..\..\DataSets\t10k-labels.idx1-ubyte", 0, 1);

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);

            BasicNetwork network = options.ContainsKey("l")?
                (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(options["l"])):
                CreateNetwork();

            var train = new QuickPropagation(network, trainingSet)
            {
                //RType = RPROPType.iRPROPp,
                //ErrorFunction = new CrossEntropyErrorFunction(),
                //BatchSize = 10000,
            };

            var improvementStop = new StopTrainingStrategy(0.000001, 10);
            var minutesStop = new EndMinutesStrategy(minutes);
            train.AddStrategy(improvementStop);
            train.AddStrategy(minutesStop);

            int epoch = 1;
            while (!(improvementStop.ShouldStop() || minutesStop.ShouldStop()))
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
            }
            train.FinishTraining();
            
            Console.WriteLine(Evaluation.accuracy(network, validationSet));

            if(options.ContainsKey("s"))
                EncogDirectoryPersistence.SaveObject(new FileInfo(options["s"]), network);
        }

        private BasicNetwork CreateNetwork()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 28 * 28));
            network.AddLayer(new BasicLayer(new ActivationReLU(), true, 40));
            network.AddLayer(new BasicLayer(new ActivationSoftMax(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }

        private BasicMLDataSet LoadDataSet(string img, string labels, int low=-1, int high=1)
        {
            Mat[] X = MnistReader.ReadImages(img);

            double[] _Y = MnistReader.ReadLabels(labels);
            var Y = OneHotEncoder.Transform(_Y, low, high);

            return MatDataSet.Convert(X, Y);
        }
    }
}
