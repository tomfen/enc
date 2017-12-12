using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc.cifar
{
    class CifarReader
    {
        private const int filesize = 1 + 32 * 32 * 3;
        private const int samplesPerFile = 10000;

        static public void ReadImages(String[] fileNames, out double[] labels, out Mat[] images)
        {
            images = new Mat[fileNames.Length * samplesPerFile];
            labels = new double[fileNames.Length * samplesPerFile];
            
            int sample = 0;

            foreach(string fileName in fileNames)
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                BinaryReader br = new BinaryReader(file);

                for(int i = 0; i < samplesPerFile; i++)
                {
                    labels[sample] = br.ReadByte();

                    Mat R = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                    Mat G = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));
                    Mat B = new Mat(32, 32, MatType.CV_8UC1, br.ReadBytes(32 * 32 * 1));

                    Mat img = new Mat();
                    Cv2.Merge(new Mat[3] { B, G, R }, img);
                    images[sample] = img;

                    sample++;
                }

                file.Close();
                br.Close();
            }
        }

        static public string[] ReadLabelNames(string filename)
        {
            return File.ReadAllLines(filename);
        }
    }
}
