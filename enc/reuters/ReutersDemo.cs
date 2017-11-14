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

namespace enc
{
    class ReutersDemo : IExperiment
    {
        public string Command => "r";

        public string Name => "Klasyfikacja tekstu";

        public string Description => "";

        public string Options => "";

        public void Run(Dictionary<string, string> options)
        {
            int features = 300;

            var format = new CSVFormat('.', ',');
            CSVMLDataSet trainingSet = new CSVMLDataSet(@"C:\Users\Tomek\Desktop\train.csv", features, 10, false, format, false);
            CSVMLDataSet testSet = new CSVMLDataSet(@"C:\Users\Tomek\Desktop\test.csv", features, 10, false, format, false);
            //BasicNetwork network = (BasicNetwork)Encog.Persist.EncogDirectoryPersistence.LoadObject(new System.IO.FileInfo(@"..\REUT"));
            
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, features));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), true, 500));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset();

            var train = new ResilientPropagation(network, trainingSet)
            {
                RType = RPROPType.iRPROPp
            };



            var timeLimit = new EndMinutesStrategy(10);

            train.AddStrategy(timeLimit);

            int epoch = 1;
            while (!timeLimit.ShouldStop())
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
            }
            
            train.FinishTraining();
            Encog.Persist.EncogDirectoryPersistence.SaveObject(new System.IO.FileInfo(@"..\REUT2"), network);

            Console.WriteLine(Evaluation.F1(network, testSet));
        }
    }
}
