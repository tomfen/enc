﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc.mnist
{
    class MnistReader
    {
        public static double[] ReadLabels(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(file);

            int magic = br.ReadInt32BE();
            int items = br.ReadInt32BE();

            double[] ret = new double[items];

            for (int i = 0; i < items; i++)
                ret[i] = br.ReadByte();

            file.Close();
            br.Close();

            return ret;
        }

        static public Mat[] ReadImages(String fileName)
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

            Mat[] ret = new Mat[items];

            for (int i = 0; i < items; i++)
            {
                var m = new Mat(28, 28, MatType.CV_8UC1, br.ReadBytes(28*28));
                var n = new Mat();
                m.ConvertTo(n, MatType.CV_64FC1);
                ret[i] = n;
            }

            file.Close();
            br.Close();

            return ret;
        }
        
    }
}