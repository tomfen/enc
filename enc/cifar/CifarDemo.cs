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
                @"..\..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_1.bin",
                @"..\..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_2.bin",
                /*@"..\..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_3.bin",
                @"..\..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_4.bin",
                @"..\..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_5.bin",*/
            };

            string[] testFiles = {
                @"..\..\..\..\..\DataSets\cifar-10-batches-bin\test_batch.bin",
            };

            BasicMLDataSet trainSet = LoadDataSet(trainFiles);
            BasicMLDataSet testSet = LoadDataSet(testFiles);

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 32*32*3));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 300));
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
            double[] _Y;
            Mat[] images;

            BasicMLDataSet ret = new BasicMLDataSet();

            CifarReader.ReadImages(filenames, out _Y, out images);
            double[][] Y = OneHotEncoder.Transform(_Y, 0, 1);

            double[][] X = new double[images.Length][];

            for (int i = 0; i < images.Length; i++)
            {
                var img = new Mat();

                Cv2.CvtColor(images[i], img, ColorConversionCodes.BGR2HSV);
                


                ret.Add(new BasicMLDataPair(
                    new BasicMLData(ImageUtil.ImgVector(img)),
                    new BasicMLData(Y[i])
                    ));
            }

            return ret;
        }
    }
}
