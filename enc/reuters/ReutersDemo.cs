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

        public string Description => "";
        
        public void Run(Dictionary<string, string> options)
        {
            string trainingSetFilename = @"..\..\..\..\DataSets\train.csv";
            string testSetFilename = @"..\..\..\..\DataSets\test.csv";

            int features = NumberOfFeatures(trainingSetFilename);
            Console.WriteLine(String.Format("Wczytano {0} cech.", features));

            var format = new CSVFormat('.', ',');
            CSVMLDataSet trainingSet = new CSVMLDataSet(trainingSetFilename, features, 10, true, format, false);
            CSVMLDataSet testSet = new CSVMLDataSet(testSetFilename, features, 10, true, format, false);

            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)WinPersistence.LoadSaved(options["l"]) :
                CreateNetwork(features);

            if (network == null)
                return;

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);
            double l1 = ExperimentOptions.getParameterDouble(options, "l1", 0);
            double l2 = ExperimentOptions.getParameterDouble(options, "l2", 0);

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
            //train.AddStrategy(new SimpleEarlyStoppingStrategy(trainingSet, 10));


            int epoch = 1;
            while (!train.TrainingDone)
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
            }

            var labels = new String[]{ "earn", "acq", "money-fx", "grain", "crude", "trade", "interest", "ship", "wheat", "corn" };
            
            var result = Evaluation.BreakEven(network, testSet, labels);

            foreach(var category in result)
            {
                Console.WriteLine(category.Key.ToString().PadRight(15) + category.Value.ToString("0.0000"));
            }

            if (options.ContainsKey("s"))
                WinPersistence.Save(network, options["s"]);
        }

        private BasicNetwork CreateNetwork(int features)
        {
            var dialog = new NetworkCreatorForm(features, 10);
            dialog.ShowDialog();
            return dialog.network;
        }

        private int NumberOfFeatures(string filename)
        {
            return File.ReadLines(filename).First().Split(',').Length - 10;
        }
    }
}
