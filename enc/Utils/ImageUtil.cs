using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enc
{
    class ImageUtil
    {
        static public Mat Deskew(Mat img)
        {
            int width = img.Width;
            int height = img.Height;

            var m = img.Moments();

            if (Math.Abs(m.Mu02) < 1e-2)
                return img;

            float skew = (float)(m.Mu11 / m.Mu02);
            var M = new MatOfFloat(2, 3, new float[,] { { 1, -skew, 0.5f * width * skew }, { 0, 1, 0 } });
            return img.WarpAffine(M, new Size(width, height));
        }

        static public void HOG(Mat img)
        {
            var hog = new HOGDescriptor(
                new Size(28, 28), //winSize
                new Size(14, 14), //blocksize
                new Size(7, 7), //blockStride,
                new Size(14, 14), //cellSize,
                 9, //nbins,
                  1, //derivAper,
                 -1, //winSigma,
                  0, //histogramNormType,
                0.2, //L2HysThresh,
                  true,//gammal correction,
                  64//nlevels=64
                  );//Use signed gradients
        }
    }
}
