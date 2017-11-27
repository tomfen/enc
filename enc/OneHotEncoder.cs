using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class OneHotEncoder
    {
        static public double[][] Transform(double[] Y, double negative = -1, double positive = 1)
        {
            int classes = 0;
            for (int i = 0; i < Y.Length; i++)
                if (classes < Y[i]+1)
                    classes = (int)Y[i]+1;

            double[][] ret = new double[Y.Length][];

            for (int i = 0; i < Y.Length; i++)
            {
                ret[i] = new double[classes];
                for (int j = 0; j < classes; j++)
                    ret[i][j] = j == Y[i] ? positive : negative;
            }

            return ret;
        }
    }
}
