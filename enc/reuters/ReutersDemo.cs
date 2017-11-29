﻿using System;
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
            int features = 500;

            var format = new CSVFormat('.', ',');
            CSVMLDataSet trainingSet = new CSVMLDataSet(@"..\..\..\..\..\DataSets\train.csv", features, 10, true, format, false);
            CSVMLDataSet testSet = new CSVMLDataSet(@"..\..\..\..\..\DataSets\test.csv", features, 10, true, format, false);
            
            BasicNetwork network = options.ContainsKey("l") ?
                (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(options["l"])) :
                CreateNetwork(features);

            int minutes = ExperimentOptions.getParameterInt(options, "m", 10);

            var train = new ResilientPropagation(network, trainingSet)
            {
                RType = RPROPType.iRPROPp,
                //ErrorFunction = new MultilabelErrorFunction()
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
                EncogDirectoryPersistence.SaveObject(new FileInfo(options["s"]), network);
        }

        private BasicNetwork CreateNetwork(int features)
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, features));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), true, 300));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }
}
