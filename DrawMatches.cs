﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;


namespace FaceTrackingBasics
{
        public static class DrawMatches
        {
            /// <summary>
            /// Draw the model image and observed image, the matched features and homography projection.
            /// </summary>
            /// <param name="modelImage">The model image</param>
            /// <param name="observedImage">The observed image</param>
            /// <param name="matchTime">The output total time for computing the homography matrix.</param>
            /// <returns>The model image and observed image, the matched features and homography projection.</returns>
            public static Image<Bgr, Byte> Draw(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage, out long matchTime)
            {
                Stopwatch watch;
                HomographyMatrix homography = null;

                SURFDetector surfCPU = new SURFDetector(500, false);
                VectorOfKeyPoint modelKeyPoints;
                VectorOfKeyPoint observedKeyPoints;
                Matrix<int> indices;

                Matrix<byte> mask;
                int k = 2;
                double uniquenessThreshold = 0.8;
                if (false && GpuInvoke.HasCuda)
                {
                    
                    GpuSURFDetector surfGPU = new GpuSURFDetector(surfCPU.SURFParams, 0.01f);
                    
                    using (GpuImage<Gray, Byte> gpuModelImage = new GpuImage<Gray, byte>(modelImage))
                    //extract features from the object image
                    using (GpuMat<float> gpuModelKeyPoints = surfGPU.DetectKeyPointsRaw(gpuModelImage, null))
                    using (GpuMat<float> gpuModelDescriptors = surfGPU.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                    using (GpuBruteForceMatcher<float> matcher = new GpuBruteForceMatcher<float>(DistanceType.L2))
                    {
                        watch = Stopwatch.StartNew();
                        modelKeyPoints = new VectorOfKeyPoint();
                        surfGPU.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                        

                        // extract features from the observed image
                        using (GpuImage<Gray, Byte> gpuObservedImage = new GpuImage<Gray, byte>(observedImage))
                        using (GpuMat<float> gpuObservedKeyPoints = surfGPU.DetectKeyPointsRaw(gpuObservedImage, null))
                        using (GpuMat<float> gpuObservedDescriptors = surfGPU.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                        using (GpuMat<int> gpuMatchIndices = new GpuMat<int>(gpuObservedDescriptors.Size.Height, k, 1, true))
                        using (GpuMat<float> gpuMatchDist = new GpuMat<float>(gpuObservedDescriptors.Size.Height, k, 1, true))
                        using (GpuMat<Byte> gpuMask = new GpuMat<byte>(gpuMatchIndices.Size.Height, 1, 1))
                        using (Stream stream = new Stream())
                        {
                            matcher.KnnMatchSingle(gpuObservedDescriptors, gpuModelDescriptors, gpuMatchIndices, gpuMatchDist, k, null, stream);
                            indices = new Matrix<int>(gpuMatchIndices.Size);
                            mask = new Matrix<byte>(gpuMask.Size);

                            //gpu implementation of voteForUniquess
                            using (GpuMat<float> col0 = gpuMatchDist.Col(0))
                            using (GpuMat<float> col1 = gpuMatchDist.Col(1))
                            {
                                GpuInvoke.Multiply(col1, new MCvScalar(uniquenessThreshold), col1, stream);
                                GpuInvoke.Compare(col0, col1, gpuMask, CMP_TYPE.CV_CMP_LE, stream);
                            }

                            observedKeyPoints = new VectorOfKeyPoint();
                            surfGPU.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                            //wait for the stream to complete its tasks
                            //We can perform some other CPU intesive stuffs here while we are waiting for the stream to complete.
                            stream.WaitForCompletion();

                            gpuMask.Download(mask);
                            gpuMatchIndices.Download(indices);

                            if (GpuInvoke.CountNonZero(gpuMask) >= 4)
                            {
                                int nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                                if (nonZeroCount >= 4)
                                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                            }

                            watch.Stop();
                        }
                    }
                }
                else
                {
                    //extract features from the object image
                    modelKeyPoints = surfCPU.DetectKeyPointsRaw(modelImage, null);
                    Matrix<float> modelDescriptors = surfCPU.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    observedKeyPoints = surfCPU.DetectKeyPointsRaw(observedImage, null);
                    Matrix<float> observedDescriptors = surfCPU.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
                    BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
                    matcher.Add(modelDescriptors);
                    if (observedDescriptors == null)
                    {
                        watch.Stop();
                        matchTime = watch.ElapsedMilliseconds;
                        return null;
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
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                    }

                    watch.Stop();
                }

                //Draw the matched keypoints
                Image<Bgr, Byte> result = Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);
                
                #region draw the projected region on the image
                if (homography != null)
                {  //draw a rectangle along the projected model
                    Rectangle rect = modelImage.ROI;
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
                        foreach (PointF i in pts)
                        {
                            objPoly.Push(i);
                        }
                        homography.ProjectPoints(pts);
                        foreach (PointF i in pts)
                        {
                            scenePoly.Push(i);
                        }
                        double ratio = scenePoly.Area / objPoly.Area;
                        Matrix<double> row = homography.GetRow(2);
                        if (!(ratio >= .005 && ratio <= 1.25))
                        {
                            result = null;
                        }
                        //if (Math.Abs(homography.Data[2, 0]) > .003 || Math.Abs(homography.Data[2, 1]) > .003)
                        //{
                        //    result = null;
                        //}
                        else
                        {
                            result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.Red), 5);
                       }
                    }
                }
                else
                {
                    result = null;
                }
                #endregion

                matchTime = watch.ElapsedMilliseconds;

                return result;
            }
        }

}
