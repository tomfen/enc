using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.Networks.Training.Propagation.SGD;
using Encog.Neural.NeuralData;
using Encog.Persist;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Encog.Util.Arrayutil;
using Encog.ML.Factory;
using Encog.Util.Normalize;
using Encog.ML.Data.Versatile;
using Encog.Neural.Networks.Training.Propagation;

namespace enc
{
    class MNISTDemo
    {
        

        static public void run()
        {
            var Y12 = MNISTReader.readLabels(@"..\..\train-labels.idx1-ubyte");
            var X1 = MNISTReader.readIdx(@"..\..\train-images.idx3-ubyte");

            var Y22 = MNISTReader.readLabels(@"..\..\t10k-labels.idx1-ubyte");
            var X2 = MNISTReader.readIdx(@"..\..\t10k-images.idx3-ubyte");


            var Y1 = OneHotEncoder.transform(Y12);
            var Y2 = OneHotEncoder.transform(Y22);

            BasicMLDataSet trainingSet = new BasicMLDataSet(X1, Y1);
            BasicMLDataSet validationSet = new BasicMLDataSet(X2, Y2);

            IActivationFunction[] funs =
            {
                //new MyReLU(),
                //new ActivationSigmoid(),
                new ActivationElliott(),
                //new ActivationLOG(),
            };

            Tuple<string, string>[] trainAlgs =
            {
                //Tuple.Create(MLTrainFactory.TypeBackprop, "LR=0.0006, MM=0.3"),
                Tuple.Create(MLTrainFactory.TypeRPROP, ""),
                //Tuple.Create(MLTrainFactory.TypeSCG, ""),
                //Tuple.Create(MLTrainFactory.TypeQPROP, "LR=2.0"),
                //Tuple.Create(MLTrainFactory.TypeManhattan, "LR=0.01"),
            };

            foreach (var trainAlg in trainAlgs)
            {
                foreach (var fun in funs)
                {
                    
                    BasicNetwork network = new BasicNetwork();
                    network.AddLayer(new BasicLayer(null, true, 28 * 28));
                    network.AddLayer(new BasicLayer(fun, true, 100));
                    network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 10));
                    network.Structure.FinalizeStructure();
                    network.Reset(1);
                    
                    var train = new MLTrainFactory().Create(network, trainingSet, trainAlg.Item1, trainAlg.Item2);
                    
                    var end = DateTime.UtcNow.AddMinutes(10);
                    int epoch = 1;
                    while (epoch <= 13)
                    {
                        train.Iteration();
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
                    }

                    train.FinishTraining();



                    Console.WriteLine(Evaluation.accuracy(network, validationSet));

                    //Encog.Persist.EncogDirectoryPersistence.SaveObject(new FileInfo("..\\NET"), network);
                }
            }
            Console.ReadKey();
        }

    }

    public static class Helpers
    {
        // Note this MODIFIES THE GIVEN ARRAY then returns a reference to the modified array.
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }
    }
}
