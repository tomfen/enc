using Encog.ML.Data;
using Encog.ML.Data.Basic;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace enc.mnist
{
    class MatDataSet
    {
        static public BasicMLDataSet Convert(Mat[] images, double[][] Y)
        {
            var X = MatsToDouble(images);
            
            return new BasicMLDataSet(X, Y);
        }
        
        static public double[][] MatsToDouble(Mat[] images)
        {
            var X = new double[images.Length][];

            for (int i = 0; i < images.Length; i++)
            {
                X[i] = new double[28 * 28];
                Marshal.Copy(images[i].Data, X[i], 0, 28 * 28);

                for (int j = 0; j < X[i].Length; j++)
                    X[i][j] /= 255;
            }

            return X;
        }
    }
}
