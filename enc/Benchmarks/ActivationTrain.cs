using enc.mnist;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace enc.Benchmarks
{
    class ActivationTrain : IExperiment
    {
        public string Command => "s";

        public string Name => "Testuje czas obliczania funkcji aktywacji";
        
        public string Description => "Test czasu obliczania iteracji ze względu na funkcje aktywacji.\n" +
            "-e int: liczba iteracji, domyślnie 30.\n" +
            "-size int: liczba pierwszych próbek do wczytania.\n" +
            "-s string: jeżeli podano, to zapisuje wynik do pliku w formacie .csv.";

        public void Run(Dictionary<string, string> options)
        {
            int epochs = ExperimentOptions.getParameterInt(options, "e", 30);
            int size = ExperimentOptions.getParameterInt(options, "size", 100);

            BasicMLDataSet trainingSet = LoadDataSet(@"..\..\..\..\DataSets\train-images.idx3-ubyte",
                                                     @"..\..\..\..\DataSets\train-labels.idx1-ubyte",
                                                     size);
            var functions = new IActivationFunction[] {
                new ActivationTANH(),
                new ActivationElliottSymmetric(),
                new ActivationReLU(),
                new ActivationSmoothReLU(),
                };
            
            List<String> results = new List<string>
            {
                "Funkcja;Czas"
            };

            // Test
            Console.WriteLine("Funkcja                            Czas");
            foreach (var fun in functions)
            {

                var network = new BasicNetwork();
                network.AddLayer(new BasicLayer(null, false, 28*28));
                network.AddLayer(new BasicLayer(fun, true, 300));
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 10));
                network.Structure.FinalizeStructure();
                network.Reset();

                var train = new Backpropagation(network, trainingSet, 0.001, 0)
                {

                };

                GC.Collect();
                GC.WaitForPendingFinalizers();

                var stopwatch = new Stopwatch();

                string functionName = fun.GetType().Name;
                double elapsed;

                Console.Write(functionName.PadRight(35));

                // Nie uwzględniaj czasu inicjalizacji
                train.Iteration();

                // Test
                stopwatch.Start();
                for(int i=0; i < epochs; i++)
                {
                    train.Iteration();
                }
                stopwatch.Stop();

                train.FinishTraining();

                elapsed = stopwatch.Elapsed.TotalMilliseconds;
                Console.Write(elapsed.ToString("0.000").PadRight(10));
                Console.WriteLine();
                
                results.Add(functionName + ";" + elapsed.ToString());
            }

            if (options.ContainsKey("s"))
            {
                File.WriteAllLines(options["s"], results);
            }
        }

        private BasicMLDataSet LoadDataSet(string imgFile, string labelsFile, int size)
        {
            var ret = new BasicMLDataSet();
            var ohe = new OneHotEncoder(10, 0, 1);

            Mat[] images = MnistReader.ReadImages(imgFile);
            double[] labels = MnistReader.ReadLabels(labelsFile);
            
            for (int i = 0; i < size; i++)
            {
                ret.Add(
                    new BasicMLData(ImageUtil.ImgVector(images[i])),
                    new BasicMLData(ohe.Transform(labels[i]))
                    );
            }

            return ret;
        }
    }
}
