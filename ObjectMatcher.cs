using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;

namespace FaceTrackingBasics
{
    public static class ObjectMatcher
    {
        public static Boolean Detect(ObjectDetectee observedScene, ObjectDetectee obj)
        {
            HomographyMatrix homography = null;

            
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            // extract features from the observed image
            observedKeyPoints = observedScene.objectKeyPoints;
            Matrix<float> observedDescriptors = observedScene.objectDescriptors;
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            matcher.Add(obj.objectDescriptors);
            if (observedDescriptors == null)
            {
                return false;
            }
            indices = new Matrix<int>(observedDescriptors.Rows, k);
            using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
            {
                matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                mask = new Matrix<byte>(dist.Rows, 1);
                mask.SetValue(255);
                Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
            }

            int nonZeroCount = CvInvoke.cvCountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(obj.objectKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(obj.objectKeyPoints, observedKeyPoints, indices, mask, 2);
            }

            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = obj.objectImage.ROI;
                PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};

                using (MemStorage m1 = new MemStorage())
                using (MemStorage m2 = new MemStorage())
                {

                    Contour<PointF> objPoly = new Contour<PointF>(m1);
                    Contour<PointF> scenePoly = new Contour<PointF>(m2);
                    pts.OrderBy(p => p.X).ThenBy(p => p.Y);
                    foreach (PointF i in pts)
                    {
                        objPoly.Push(i);
                    }
                    homography.ProjectPoints(pts);
                    pts.OrderBy(p => p.X).ThenBy(p => p.Y);
                    foreach (PointF i in pts)
                    {
                        scenePoly.Push(i);
                    }
                    double ratio = scenePoly.Area / objPoly.Area;
                    Matrix<double> row = homography.GetRow(2);
                    if (!(ratio >= .25 && ratio <= 1.25))
                    {
                        return false;
                    }
                    if (Math.Abs(homography.Data[2, 0]) > .003 || Math.Abs(homography.Data[2, 1]) > .003)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
