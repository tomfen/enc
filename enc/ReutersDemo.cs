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

namespace enc
{
    class ReutersDemo
    {
        public static void run()
        {
            int features = 300;

            var format = new CSVFormat('.', ',');
            CSVMLDataSet trainingSet = new CSVMLDataSet(@"C:\Users\Tomek\Desktop\train.csv", features, 10, false, format, false);
            CSVMLDataSet testSet = new CSVMLDataSet(@"C:\Users\Tomek\Desktop\test.csv", features, 10, false, format, false);
            //BasicNetwork network = (BasicNetwork)Encog.Persist.EncogDirectoryPersistence.LoadObject(new System.IO.FileInfo(@"..\REUT"));
            
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, features));
            network.AddLayer(new BasicLayer(new MyReLU(), true, 1));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset();

            var train = new ResilientPropagation(network, trainingSet);

            train.RType = RPROPType.iRPROPp;

            var timeLimit = new EndMinutesStrategy(60);

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
