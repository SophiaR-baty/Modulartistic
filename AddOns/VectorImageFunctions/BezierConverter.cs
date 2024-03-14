using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using OpenCvSharp;
using System.Diagnostics;
using BitmapToVector;
using System.IO;

namespace VectorImageFunctions
{
    public class BezierConverter
    {
        public static Bitmap CannyExtern(string path_to_img, double threshold_low, double threshold_high)
        {
           
            using var src = new Mat(path_to_img, ImreadModes.Grayscale);
            using var dst = new Mat();

            Cv2.Canny(src, dst, threshold_low, threshold_high, 3);
            /*
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }
            */

            Bitmap bm;
            using (MemoryStream stream = new MemoryStream())
            {
                dst.WriteToStream(stream);
                bm = new Bitmap(stream);
            }

            return bm;
        }

        public static Curve TraceMat(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            using var poBitmap = PotraceBitmap.Create(width, height);
            
                
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c = bm.GetPixel(j, i);
                    if (c.R + c.G + c.B != 0)
                    {
                        poBitmap.SetBlackUnsafe(j, i);
                    }
                }
            }

            // Create options
            var param = new PotraceParam();

            // Trace using Potrace
            var traceResult = Potrace.Trace(param, poBitmap);

            
            Curve resultCurve = new Curve();
            
            for (PotracePath poPath = traceResult.Plist; poPath != null; poPath = poPath.Next)
            {
                PotraceCurve poCurve = poPath.Curve;

                for (int i = 0; i < poCurve.N; i++)
                {
                    // Console.WriteLine(poCurve.Tag[i]);
                    if (poCurve.Tag[i] == 1)
                    {
                        double p0_x;
                        double p0_y;
                        if (i == 0)
                        {
                            p0_x = poCurve.C[^1][2].X;
                            p0_y = poCurve.C[^1][2].Y;
                        }
                        else
                        {
                            p0_x = poCurve.C[i - 1][2].X;
                            p0_y = poCurve.C[i - 1][2].Y;
                        }
                        BezierSegment seg = new BezierSegment(p0_x, p0_y, poCurve.C[i][0].X, poCurve.C[i][0].Y, poCurve.C[i][1].X, poCurve.C[i][1].Y, poCurve.C[i][2].X, poCurve.C[i][2].Y);
                        resultCurve.segments.Add(seg);
                    }
                    else
                    {
                        double p0_x;
                        double p0_y;
                        if (i == 0)
                        {
                            p0_x = poCurve.C[^1][2].X;
                            p0_y = poCurve.C[^1][2].Y;
                        }
                        else
                        {
                            p0_x = poCurve.C[i - 1][2].X;
                            p0_y = poCurve.C[i - 1][2].Y;
                        }
                        CornerSegment seg = new CornerSegment(p0_x, p0_y, poCurve.C[i][1].X, poCurve.C[i][1].Y, poCurve.C[i][2].X, poCurve.C[i][2].Y);
                        resultCurve.segments.Add(seg);
                    }
                }
            }
            return resultCurve;
        }
    }
}
