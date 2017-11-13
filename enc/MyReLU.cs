using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.Engine.Network.Activation;

namespace enc
{
    class MyReLU : IActivationFunction
    {
        public void ActivationFunction(double[] d, int start, int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (d[i] < 0)
                    d[i] = 0.01*d[i];
            }
        }

        public double DerivativeFunction(double b, double a)
        {
            return b > 0 ? 1 : 0.01;
        }

        public bool HasDerivative
        {
            get { return true; }
        }

        public string[] ParamNames
        {
            get { return new string[]{}; }
        }

        public double[] Params
        {
            get { return new double[]{}; }
        }

        public object Clone()
        {
            return new MyReLU();
        }
    }
}
