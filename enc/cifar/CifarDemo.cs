using enc.Utils;
using Encog.ML.Data.Basic;
using Encog.ML.Train.Strategy;
using Encog.ML.Train.Strategy.End;
using Encog.Neural.Error;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace enc.cifar
{
    class CifarDemo : IExperiment
    {
        public string Command => "c";

        public string Name => "Klasyfikacja zbioru Cifar-10";

        public string Description => "Trenuje i testuje model dla klasyfikacji zbioru CIFAR-10.\n" +
            "-m int: maksymalny czas uczenia w minutach. Domyślnie 10. Ustaw 0, jezeli wczytujesz model do testow.\n" +
            "-hog: jeżeli podano, to używa deskryptora HOG zamiast wartosci pikseli.\n" +
            "-l1 double: wartosc regularyzacji l1. Domyslnie 0.\n" + 
            "-l2 double: wartosc regularyzacji l2. Domyslnie 0.\n" +
            "-l string: sciezka do wczytania modelu. Jezeli nie podano, to tworzony jest nowy model za pomoca kreatora. " +
            "Jezeli nie podano wartosci, to otwierany jest eksplorator.\n" +
            "-s string: sciezka do zapisu modelu. Jezeli nie podano wartosci, to otwierany jest eksplorator.";

        public void Run(Dictionary<string, string> options)
        {
            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);
            bool useHog = options.ContainsKey("hog");
            double l1 = ExperimentOptions.getParameterDouble(options, "l1", 0);
            double l2 = ExperimentOptions.getParameterDouble(options, "l2", 0);

            string[] trainFiles = {
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_1.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_2.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_3.bin",
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_4.bin",
            };
            
            string[] validationFiles = {
                @"..\..\..\..\DataSets\cifar-10-batches-bin\data_batch_5.bin",
            };

            string[] testFiles = {
                @"..\..\..\..\DataSets\cifar-10-batches-bin\test_batch.bin",
            };

            BasicMLDataSet trainingSet = LoadDataSet(trainFiles, useHog);
            BasicMLDataSet validationSet = LoadDataSet(validationFiles, useHog);
            BasicMLDataSet testSet = LoadDataSet(testFiles, useHog);
            

            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)WinPersistence.LoadSaved(options["l"]) :
                CreateNetwork(trainingSet.InputSize);

            if (network == null)
                return;

            var initialUpdate = options.ContainsKey("l") ? 0.001 : RPROPConst.DefaultInitialUpdate;
            var train = new ResilientPropagation(network, trainingSet, initialUpdate, RPROPConst.DefaultMaxStep)
            {
                RType = RPROPType.iRPROPp,
                ErrorFunction = new CrossEntropyErrorFunction(),
                L1 = l1,
                L2 = l2,
            };
            
            var minutesStop = new EndMinutesStrategy(minutes);
            var earlyStop = new EarlyStoppingStrategy(validationSet, testSet);
            train.AddStrategy(minutesStop);
            train.AddStrategy(earlyStop);

            while (!train.TrainingDone)
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + train.IterationNumber + " Error:" + train.Error);
            }
            train.FinishTraining();

            Console.WriteLine(Evaluation.Accuracy(network, testSet));
            
            if (options.ContainsKey("s"))
                WinPersistence.Save(network, options["s"]);
        }

        public BasicMLDataSet LoadDataSet(string[] filenames, bool useHog)
        {
            BasicMLDataSet ret = new BasicMLDataSet();

            foreach (string filename in filenames)
            {
                var ohe = new OneHotEncoder(10, 0, 1);

                foreach (Tuple<Mat, int> pair in new CifarIterator(filename))
                {
                    var encoded = ohe.Transform(pair.Item2);
                    
                    double[] input;
                    if (useHog)
                    {
                        Mat grayImg = new Mat();
                        Cv2.CvtColor(pair.Item1, grayImg, ColorConversionCodes.BGR2GRAY);

                        var hog = new HOGDescriptor(
                            winSize:    new Size(32, 32),
                            blockSize:  new Size(16, 16),
                            blockStride:new Size(8, 8),
                            cellSize:   new Size(8, 8));
                        input = ImageUtil.HOGVector(pair.Item1, hog);
                    }
                    else
                    {
                        Mat img = new Mat();
                        Cv2.CvtColor(pair.Item1, img, ColorConversionCodes.BGR2HSV);

                        input = ImageUtil.ImgVector(img);
                    }

                    ret.Add(new BasicMLDataPair(
                        new BasicMLData(input, false),
                        new BasicMLData(encoded, false)
                        ));
                }
            }

            return ret;
        }

        private BasicNetwork CreateNetwork(int features)
        {
            var dialog = new NetworkCreatorForm(features, 10);
            dialog.ShowDialog();
            return dialog.network;
        }
    }
}
