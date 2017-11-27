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
using System.Runtime.InteropServices;
using enc.mnist;

namespace enc.mnist
{
    class MnistDemo : IExperiment
    {
        public string Command => "m";

        public string Description => "";

        public string Options => "";

        public string Name => "Klasyfikacja zbioru MNIST";

        public void Run(Dictionary<string, string> options)
        {
            double[] _Y1 = MnistReader.ReadLabels(@"..\..\..\..\..\DataSets\train-labels.idx1-ubyte");
            double[] _Y2 = MnistReader.ReadLabels(@"..\..\..\..\..\DataSets\t10k-labels.idx1-ubyte");

            Mat[] X1 = MnistReader.ReadImages(@"..\..\..\..\..\DataSets\train-images.idx3-ubyte");
            Mat[] X2 = MnistReader.ReadImages(@"..\..\..\..\..\DataSets\t10k-images.idx3-ubyte");

            for (int i = 60000 - 100; i < 60000; i++)
            {
                X1[i].SaveImage(@"..\..\..\..\..\TEST\" + i + ".png");
                ImageUtil.Deskew(X1[i]).SaveImage(@"..\..\..\..\..\TEST\" + i + "D.png");
            }

            var Y1 = OneHotEncoder.Transform(_Y1);
            var Y2 = OneHotEncoder.Transform(_Y2);


            BasicMLDataSet trainingSet = MatDataSet.Convert(X1, Y1);
            BasicMLDataSet validationSet = MatDataSet.Convert(X2, Y2);

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 28 * 28));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 200));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 10));
            network.Structure.FinalizeStructure();
            network.Reset(1);

            var train = new ResilientPropagation(network, trainingSet);
            train.RType = RPROPType.iRPROPp;
            train.BatchSize = 10;

            int epoch = 1;
            while (epoch <= 1)
            {
                train.Iteration();
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "| Epoch #" + epoch++ + " Error:" + train.Error);
            }

            train.FinishTraining();



            Console.WriteLine(Evaluation.accuracy(network, validationSet));

            //Encog.Persist.EncogDirectoryPersistence.SaveObject(new FileInfo("..\\NET"), network);

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
