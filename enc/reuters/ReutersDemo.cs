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
            int features = 10000; //20708

            var format = new CSVFormat('.', ',');
            CSVMLDataSet trainingSet = new CSVMLDataSet(@"..\..\..\..\..\DataSets\train.csv", features, 10, true, format, false);
            CSVMLDataSet testSet = new CSVMLDataSet(@"..\..\..\..\..\DataSets\test.csv", features, 10, true, format, false);

            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)WinPersistence.LoadSaved(options["l"]) :
                CreateNetwork(features);

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);

            Propagation train = options.ContainsKey("a") ?
                (Propagation)new ResilientPropagation(network, trainingSet)
                {
                    RType = RPROPType.iRPROPp,
                    ErrorFunction = new CrossEntropyErrorFunction(),
                } :
                new QuickPropagation(network, trainingSet);
            Console.WriteLine(train);


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
    }
}
