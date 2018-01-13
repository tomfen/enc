using OpenCvSharp;
using System;
using System.IO;

namespace enc.mnist
{
    class MnistReader
    {
        public static double[] ReadLabels(string path, int start = 0, int size = 0)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            int magic = br.ReadInt32BE();
            int items = br.ReadInt32BE();

            //pomiń pierwsze
            if (start > 0)
                br.ReadBytes(start);

            //gdzie skończyć
            int toRead = size > 0?
                size:
                items;

            double[] ret = new double[toRead];

            for (int i = 0; i < toRead; i++)
                ret[i] = br.ReadByte();

            file.Close();
            br.Close();

            return ret;
        }

        static public Mat[] ReadImages(string fileName, int start = 0, int size = 0)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            int magic = br.ReadInt32BE();
            int dim = magic & 0xff;

            int items = br.ReadInt32BE();
            int itemSize = 1;
            for (int i = 0; i < dim - 1; i++)
            {
                itemSize *= br.ReadInt32BE();
            }


            //pomiń pierwsze
            if (start > 0)
                br.ReadBytes(start * 28 * 28);

            //gdzie skończyć
            int toRead = size > 0 ?
                size :
                items;

            Mat[] ret = new Mat[toRead];

            for (int i = 0; i < toRead; i++)
            {
                ret[i] = new Mat(28, 28, MatType.CV_8UC1, br.ReadBytes(28 * 28));
            }

            file.Close();
            br.Close();

            return ret;
        }
        
    }
}
