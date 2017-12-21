using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.ML.Data.Specific;
using Encog.Engine.Network.Activation;
using Encog.ML.Factory;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.CSV;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.ML.Train.Strategy;
using Encog.ML.Train.Strategy.End;
using Encog.Neural.RBF;
using Encog.Neural.Rbf.Training;
using Encog.Persist;
using System.IO;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Error;
using Encog.ML.SVM;
using enc.Utils;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.MathUtil.RBF;
using Encog.Neural.Networks.Training.Propagation.SGD;
using Encog.Neural.Networks.Training.Propagation.SGD.Update;

namespace enc.reuters
{
    class ReutersDemo : IExperiment
    {
        public string Command => "r";

        public string Name => "Klasyfikacja tekstu";

        public string Description => "";

        public string Options => "";

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

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);

            var train = new ResilientPropagation(network, trainingSet)
                {
                    RType = RPROPType.iRPROPp,
                    ErrorFunction = new CrossEntropyErrorFunction(),
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

            Console.WriteLine(Evaluation.F1(network, testSet));

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
