using System;
using System.Collections.Generic;
using System.Linq;
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using Encog.Util.CSV;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.ML.Train.Strategy;
using Encog.ML.Train.Strategy.End;
using System.IO;
using Encog.Neural.Error;
using enc.Utils;

namespace enc.reuters
{
    class ReutersDemo : IExperiment
    {
        public string Command => "r";

        public string Name => "Klasyfikacja tekstu";

        public string Description => "Trenuje i testuje model dla klasyfikacji zbioru Reuters-21578.\n" +
            "-m int: maksymalny czas uczenia w minutach. Domyślnie 10. Ustaw 0, jezeli wczytujesz model do testow.\n" +
            "-l1 double: wartosc regularyzacji l1. Domyslnie 0.\n" +
            "-l2 double: wartosc regularyzacji l2. Domyslnie 0.\n" +
            "-l string: sciezka do wczytania modelu. Jezeli nie podano, to tworzony jest nowy model za pomoca kreatora. " +
            "Jezeli nie podano wartosci, to otwierany jest eksplorator.\n" +
            "-s string: sciezka do zapisu modelu. Jezeli nie podano wartosci, to otwierany jest eksplorator.";

        public void Run(Dictionary<string, string> options)
        {
            string trainingSetFilename = @"..\..\..\..\DataSets\train.csv";
            string testSetFilename = @"..\..\..\..\DataSets\test.csv";

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);
            double l1 = ExperimentOptions.getParameterDouble(options, "l1", 0);
            double l2 = ExperimentOptions.getParameterDouble(options, "l2", 0);

            int features = NumberOfFeatures(trainingSetFilename);
            Console.WriteLine(String.Format("Wczytuję {0} cech...", features));

            var format = new CSVFormat('.', ',');

            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)WinPersistence.LoadSaved(options["l"]) :
                CreateNetwork(features);

            if (network == null)
                return;

            if (minutes > 0)
            {
                CSVMLDataSet trainingSet = new CSVMLDataSet(trainingSetFilename, features, 90, true, format, false);

                var initialUpdate = options.ContainsKey("l") ? 0.001 : RPROPConst.DefaultInitialUpdate;
                var train = new ResilientPropagation(network, trainingSet, initialUpdate, RPROPConst.DefaultMaxStep)
                {
                    RType = RPROPType.iRPROPp,
                    ErrorFunction = new CrossEntropyErrorFunction(),
                    L1 = l1,
                    L2 = l2,
                };


                var improvementStop = new StopTrainingStrategy(0, 10);
                var minutesStop = new EndMinutesStrategy(minutes);
                train.AddStrategy(improvementStop);
                train.AddStrategy(minutesStop);

                int epoch = 1;
                while (!train.TrainingDone)
                {
                    train.Iteration();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
                }
            }

            var labels = Labels(trainingSetFilename);

            CSVMLDataSet testSet = new CSVMLDataSet(testSetFilename, features, 90, true, format, false);

            var breakEven = Evaluation.BreakEven(network, testSet, labels);
            var f1 = Evaluation.F1(network, testSet, labels);


            var relevantCategories = new string[11] {
                "earn", "acq", "money-fx", "grain", "crude", "trade", "interest", "ship", "wheat", "corn",
                "Micro Avg."};

            Console.WriteLine("Kategoria         BEP       F1");

            foreach (var category in relevantCategories)
            {
                Console.WriteLine(category.PadRight(15) + breakEven[category].ToString("0.0000") + "   " + f1[category].ToString("0.0000"));
            }

            if (options.ContainsKey("s"))
                WinPersistence.Save(network, options["s"]);
        }

        private BasicNetwork CreateNetwork(int features)
        {
            var dialog = new NetworkCreatorForm(features, 90);
            dialog.ShowDialog();
            return dialog.network;
        }

        private int NumberOfFeatures(string filename)
        {
            return File.ReadLines(filename).First().Split(',').Length - 90;
        }

        private string[] Labels(string filename)
        {
            string[] header = File.ReadLines(filename).First().Split(',');
            return header.Skip(header.Count() - 90).ToArray();
        }
    }
}
