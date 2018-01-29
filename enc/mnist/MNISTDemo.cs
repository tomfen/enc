using System;
using System.Collections.Generic;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using OpenCvSharp;
using Encog.ML.Train.Strategy;
using Encog.ML.Train.Strategy.End;
using Encog.Neural.Error;
using enc.Utils;

namespace enc.mnist
{
    class MnistDemo : IExperiment
    {
        public string Command => "m";

        public string Name => "Klasyfikacja zbioru MNIST";
        
        public string Description => "Trenuje i testuje model dla klasyfikacji zbioru MNIST.\n" +
            "-m int: maksymalny czas uczenia w minutach. Domyślnie 10. Ustaw 0, jezeli wczytujesz model do testow.\n" +
            "-deskew: jezeli podano, to pochylenie jest normalizowane.\n" +
            "-hog: jeżeli podano, to używa deskryptora HOG zamiast wartosci pikseli.\n" +
            "-l1 double: wartosc regularyzacji l1. Domyslnie 0.\n" +
            "-l2 double: wartosc regularyzacji l2. Domyslnie 0.\n" +
            "-l string: sciezka do wczytania modelu. Jezeli nie podano, to tworzony jest nowy model za pomoca kreatora. " +
            "Jezeli nie podano wartosci, to otwierany jest eksplorator.\n" +
            "-s string: sciezka do zapisu modelu. Jezeli nie podano wartosci, to otwierany jest eksplorator.";

        public void Run(Dictionary<string, string> options)
        {
            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);
            bool deskew = options.ContainsKey("deskew");
            bool useHog = options.ContainsKey("hog");
            double l1 = ExperimentOptions.getParameterDouble(options, "l1", 0);
            double l2 = ExperimentOptions.getParameterDouble(options, "l2", 0);

            BasicMLDataSet trainingSet = LoadDataSet(@"..\..\..\..\DataSets\train-images.idx3-ubyte",
                                                     @"..\..\..\..\DataSets\train-labels.idx1-ubyte", 0, 50000,
                                                     deskew, useHog, 0, 1);

            BasicMLDataSet validationSet = LoadDataSet(@"..\..\..\..\DataSets\train-images.idx3-ubyte",
                                                     @"..\..\..\..\DataSets\train-labels.idx1-ubyte", 50000, 10000,
                                                     deskew, useHog, 0, 1);

            BasicMLDataSet testSet = LoadDataSet(@"..\..\..\..\DataSets\t10k-images.idx3-ubyte",
                                                       @"..\..\..\..\DataSets\t10k-labels.idx1-ubyte", 0, 10000,
                                                       deskew, useHog, 0, 1);


            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)WinPersistence.LoadSaved(options["l"]):
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
            
            Console.WriteLine("Error rate: " + Evaluation.ErrorRate(network, testSet)*100 + "%");

            if (options.ContainsKey("s"))
                WinPersistence.Save(network, options["s"]);
        }

        private BasicNetwork CreateNetwork(int features)
        {
            var dialog = new NetworkCreatorForm(features, 10);
            dialog.ShowDialog();
            return dialog.network;
        }

        private BasicMLDataSet LoadDataSet(string imgFile, string labelsFile, int startIndex = 0, int toRead = 0,
            bool deskew = false, bool useHog = false,
            int negative = -1, int positive = 1)
        {
            Mat[] images = MnistReader.ReadImages(imgFile, startIndex, toRead);
            double[] labels = MnistReader.ReadLabels(labelsFile, startIndex, toRead);

            var ohe = new OneHotEncoder(10, 0, 1);
            var ret = new BasicMLDataSet();

            if (!useHog)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    Mat img = deskew ? ImageUtil.Deskew(images[i]) : images[i];

                    var imgVec = ImageUtil.ImgVector(img);
                    var labelVec = ohe.Transform(labels[i]);

                    ret.Add(new BasicMLDataPair(
                        new BasicMLData(imgVec, false),
                        new BasicMLData(labelVec, false)
                        ));
                }
            }
            else
            {
                HOGDescriptor hog = new HOGDescriptor(
                    winSize:    new Size(28, 28),
                    blockSize:  new Size(14, 14),
                    blockStride:new Size(7, 7),
                    cellSize:   new Size(14, 14)
                );

                for (int i = 0; i < images.Length; i++)
                {
                    var hogVec = ImageUtil.HOGVector(images[i], hog);
                    var labelVec = ohe.Transform(labels[i]);

                    ret.Add(new BasicMLDataPair(
                        new BasicMLData(hogVec, false),
                        new BasicMLData(labelVec, false)
                        ));
                }
            }
            
            return ret;
        }
    }
}
