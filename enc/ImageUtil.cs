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
        static Mat deskew(Mat img)
        {
            int width = img.Width;
            int height = img.Height;

            var m = img.Moments();
            if (Math.Abs(m.Mu02) < 1e-2)
                return img;
            float skew = (float)(m.Mu11 / m.Mu02);
            var M = new MatOfFloat(2, 3, new float[,] { { 1, -skew, 0.5f * width * skew }, { 0, 1, 0 } });
            return img.WarpAffine(M, new OpenCvSharp.Size(width, height));
        }
    }
}
