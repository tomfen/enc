using Encog.Neural.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;

namespace enc.reuters
{
    public class MultilabelErrorFunction : IErrorFunction
    {
        const double threshold = 0.5;

        public void CalculateError(IActivationFunction af, double[] b, double[] a,
            IMLData ideal, double[] actual, double[] error, double derivShift,
            double significance)
        {

            double Yp = 0, Yn = 0;
            for(int i=0; i < ideal.Count; i++)
            {
                if (ideal[i] > threshold)
                    Yp++;
                else
                    Yn++;
            }

            for (int j = 0; j < actual.Length; j++)
            {
                double deriv = af.DerivativeFunction(b[j], a[j]);

                if (actual[j] > threshold) // is positive
                {
                    double suml = 0;
                    for (int l = 0; l < actual.Length; l++)
                    {
                        if (ideal[l] <= threshold)
                            suml += Math.Exp(-(actual[j] - actual[l]));
                    }

                    error[j] = -(1 / Yn * Yp) * suml * deriv;
                }
                else // is negative
                {
                    double sumk = 0;
                    for (int k = 0; k < actual.Length; k++)
                    {
                        if (ideal[k] > threshold)
                            sumk += Math.Exp(-(actual[k] - actual[j]));
                    }

                    error[j] = (1 / Yn * Yp) * sumk * deriv;
                }
            }
        }
    }
}
