using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class ActivationBenchmark
    {
        static private double[] randomArray(int size, double min, double max)
        {
            var ret = new double[size];
            var random = new Random();

            for (int i = 0; i < size; i++)
            {
                ret[i] = random.NextDouble() * (max - min) + min;
            }

            return ret;
        }

        static public void run()
        {
            IActivationFunction[] funs =
            {
                /*new ActivationBiPolar(),
                new ActivationElliottSymmetric(),
                new ActivationLOG(),*/
                new ActivationReLU(),
                new MyReLU(),
                /*new ActivationClippedLinear(),
                new ActivationLinear(),
                new ActivationRamp(),
                new ActivationSmoothReLU(),
                new ActivationTANH(),
                new ActivationStep(),*/
            };

            double[] arr = randomArray(10000000, -2.0, 2.0);

            Console.WriteLine("Pochodna:");
            foreach (var fun in funs)
            {
                double[] res = (double[])arr.Clone();

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                for (int i = 0; i < 5; i++) //kilka próbek
                    for (int j = 0; j < res.Length; j++)
                        fun.DerivativeFunction(res[i], 0);

                stopwatch.Stop();
                
                Console.WriteLine(fun.ToString().PadRight(60) + "time: " + stopwatch.Elapsed.ToString(@"ss\.ffff"));
            }


            Console.WriteLine("Wartość funkcji:");
            foreach (var fun in funs)
            {
                double[] res = (double[])arr.Clone();

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                for (int i = 0; i < 5; i++)
                    fun.ActivationFunction(res, 0, res.Length);

                stopwatch.Stop();

                Console.WriteLine(fun.ToString().PadRight(60) + "time: " + stopwatch.Elapsed.ToString(@"ss\.ffff"));
            }

            Console.ReadKey();
        }
    }
}
