using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc.cifar
{
    class CifarDemo : IExperiment
    {
        public string Command => "c";

        public string Name => "Klasyfikacja zbioru Cifar-10";

        public string Description => throw new NotImplementedException();

        public string Options => throw new NotImplementedException();

        public void Run(Dictionary<string, string> options)
        {
            string[] trainFiles = {
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_1.bin",
                /*@"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_2.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_3.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_4.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_5.bin",*/
            };

            string[] testFiles = {
                @"..\..\..\..\DataSets\cifar-10-batches-bin\test_batch.bin",
            };

            BasicMLDataSet trainSet = LoadDataSet(trainFiles);
            BasicMLDataSet testSet = LoadDataSet(testFiles);

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, false, 32*32*3));
            //network.AddLayer(new BasicLayer(new ActivationReLU(), true, 1000));
            //network.AddLayer(new BasicLayer(new ActivationLinear(), true, 800));
            network.AddLayer(new BasicLayer(new ActivationReLU(), true, 300));
            network.AddLayer(new BasicLayer(new ActivationSoftMax(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset();

            var train = new ResilientPropagation(network, trainSet)
            {
                RType = RPROPType.iRPROPp,
                //ErrorFunction = new CrossEntropyErrorFunction(),
            };
            
            int epoch = 1;
            while (epoch < 10)
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
            }
            train.FinishTraining();

            Console.WriteLine(Evaluation.Accuracy(network, testSet));
        }

        public BasicMLDataSet LoadDataSet(string[] filenames)
        {
            BasicMLDataSet ret = new BasicMLDataSet();

            foreach (string filename in filenames)
            {
                var ohe = new OneHotEncoder(10, 0, 1);

                foreach (Tuple<Mat, int> pair in new CifarIterator(filename))
                {
                    var img = new Mat();
                    var encoded = ohe.Transform(pair.Item2);

                    Cv2.CvtColor(pair.Item1, img, ColorConversionCodes.BGR2HSV);

                    ret.Add(new BasicMLDataPair(
                        new BasicMLData(ImageUtil.ImgVector(img)),
                        new BasicMLData(encoded)
                        ));
                }
            }

            return ret;
        }
    }
}
