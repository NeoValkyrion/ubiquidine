﻿// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FaceTrackingBasics
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Threading;
    using Microsoft.Kinect.Toolkit;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using Emgu.CV;
    using Emgu.CV.Structure;
    using WebApplication2;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;
    using Microsoft.Kinect.Toolkit.FaceTracking;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();
        private WriteableBitmap colorImageWritableBitmap;
        private byte[] colorImageData;
        private ColorImageFormat currentColorImageFormat = ColorImageFormat.Undefined;
        private static Image<Gray, Byte> objectImage;
        private static ObjectDetectee plate;
        private static ObjectDetectee waiter, glass;
        private volatile static bool thread_working = false;
        private volatile static Image<Gray, Byte> sceneImage;
        private static System.Threading.AutoResetEvent autoEvent = new AutoResetEvent(false);
        private static Mutex mut = new Mutex();
        private Thread workerThread = new Thread(MatcherThread);


        private static void MatcherThread()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                autoEvent.WaitOne();
                mut.WaitOne();
                thread_working = true;
                mut.ReleaseMutex();
                long test;

                watch.Start();
                ObjectDetectee scene = new ObjectDetectee(sceneImage);

                if (ObjectMatcher.Detect(scene, plate))
                {
                    if (plate.reportSeen(watch.ElapsedMilliseconds))
                    {
                        watch.Stop();
                        WebApplication2.Controller.emptyPlate(1, 1);
                        watch.Start();
                    }
                }
                if (ObjectMatcher.Detect(scene, glass))
                {
                    if (waiter.reportSeen(watch.ElapsedMilliseconds))
                    {
                        watch.Stop();
                        WebApplication2.Controller.emptyDrink(1, 1);
                        watch.Start();
                    }
                }
                if (ObjectMatcher.Detect(scene, waiter))
                {
                    if (waiter.reportSeen(watch.ElapsedMilliseconds))
                    {
                        watch.Stop();
                        WebApplication2.Controller.waiterCalled(1, 1);
                        watch.Start();
                    }
                }
                
                mut.WaitOne();
                thread_working = false;
                mut.ReleaseMutex();
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            workerThread.Start();
            var faceTrackingViewerBinding = new Binding("Kinect") { Source = sensorChooser };
            faceTrackingViewer.SetBinding(FaceTrackingViewer.KinectProperty, faceTrackingViewerBinding);

            this.InitializeComponent();

            plate = new ObjectDetectee("aug2.jpg");
            //objectImage = new Image<Gray, byte>("aug.jpg");
            waiter = new ObjectDetectee("aug.jpg", 5000, 3);
            glass = new ObjectDetectee("glass3.jpg");

            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;

            sensorChooser.Start();
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs kinectChangedEventArgs)
        {
            KinectSensor oldSensor = kinectChangedEventArgs.OldSensor;
            KinectSensor newSensor = kinectChangedEventArgs.NewSensor;

            if (oldSensor != null)
            {
                oldSensor.AllFramesReady -= KinectSensorOnAllFramesReady;
                oldSensor.ColorStream.Disable();
                oldSensor.DepthStream.Disable();
                oldSensor.DepthStream.Range = DepthRange.Default;
                oldSensor.SkeletonStream.Disable();
                oldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                oldSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }

            if (newSensor != null)
            {
                try
                {
                    newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    newSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    try
                    {
                        // This will throw on non Kinect For Windows devices.
                        newSensor.DepthStream.Range = DepthRange.Near;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        newSensor.DepthStream.Range = DepthRange.Default;
                        newSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }

                    newSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    newSensor.SkeletonStream.Enable();
                    newSensor.AllFramesReady += KinectSensorOnAllFramesReady;
                }
                catch (InvalidOperationException)
                {
                    // This exception can be thrown when we are trying to
                    // enable streams on a device that has gone away.  This
                    // can occur, say, in app shutdown scenarios when the sensor
                    // goes away between the time it changed status and the
                    // time we get the sensor changed notification.
                    //
                    // Behavior here is to just eat the exception and assume
                    // another notification will come along if a sensor
                    // comes back.
                }
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            sensorChooser.Stop();
            faceTrackingViewer.Dispose();
        }

        public volatile int time = 0;

        private void KinectSensorOnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            using (var colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Make a copy of the color frame for displaying.
                var haveNewFormat = this.currentColorImageFormat != colorImageFrame.Format;
                if (haveNewFormat)
                {
                    this.currentColorImageFormat = colorImageFrame.Format;
                    this.colorImageData = new byte[colorImageFrame.PixelDataLength];
                    this.colorImageWritableBitmap = new WriteableBitmap(
                        colorImageFrame.Width, colorImageFrame.Height, 96, 96, PixelFormats.Bgr32, null);
                    ColorImage.Source = this.colorImageWritableBitmap;
                }
                //Image<Bgr,Byte>  new Image<TColor, TDepth>(bitmap);
                colorImageFrame.CopyPixelDataTo(this.colorImageData);
                this.colorImageWritableBitmap.WritePixels(
                    new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height),
                    this.colorImageData,
                    colorImageFrame.Width * Bgr32BytesPerPixel,
                    0);

                Bitmap bmap = ImageToBitmap(colorImageFrame);
                mut.WaitOne();
                if (!thread_working)
                {
                    sceneImage = (new Image<Bgr, byte>(bmap)).Convert<Gray, byte>();
                    autoEvent.Set();
                }
                mut.ReleaseMutex();

                //WebApplication2.Controller.emptyPlate(1, 0);
            }
        }

        Bitmap ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, Image.Width, Image.Height),
                ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr, Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
    }
}
