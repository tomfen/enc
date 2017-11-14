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
    class MatDataSet : IMLDataSet
    {
        static public BasicMLDataSet Convert(Mat[] images, double[][] Y)
        {
            var X = new double[images.Length][];

            for (int i = 0; i < images.Length; i++)
            {
                X[i] = new double[28 * 28];
                Marshal.Copy(images[i].Data, X[i], 0, 28 * 28);
            }

            return new BasicMLDataSet(X, Y);
        }


        private Mat[] images;
        private double[][] ideal;

        MatDataSet(Mat[] images, double[][] ideal)
        {
            this.images = images;
            this.ideal = ideal;
        }

        public IMLDataPair this[int x] => throw new NotImplementedException();

        public int IdealSize { get { return ideal[0].Length; } }

        public int InputSize { get { return images[0].Height * images[0].Width; } }

        public int Count { get { return images.Length; } }

        public bool Supervised { get { return ideal != null; } }

        public void Close()
        {
            
        }

        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IMLDataSet OpenAdditional()
        {
            throw new NotImplementedException();
        }
    }
}
