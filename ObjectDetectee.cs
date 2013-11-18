using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;

namespace FaceTrackingBasics
{
    public class ObjectDetectee
    {
        private Image<Gray, Byte> _objectImage;
        public Image<Gray, Byte> objectImage
        {
            get { return _objectImage; }
            set { _objectImage = value; }
        }

        private VectorOfKeyPoint _objectKeyPoints;
        public VectorOfKeyPoint objectKeyPoints
        {
            get { return _objectKeyPoints; }
            set { _objectKeyPoints = value; }
        }
        private Matrix<float> _objectDescriptors;
        
        public Matrix<float> objectDescriptors
        {
            get { return _objectDescriptors; }
            set { _objectDescriptors = value; }
        }

        private void extractData() {
             SURFDetector surfCPU = new SURFDetector(500, false);

            _objectKeyPoints = surfCPU.DetectKeyPointsRaw(objectImage, null);
            _objectDescriptors = surfCPU.ComputeDescriptorsRaw(objectImage, null, objectKeyPoints);
        }

        public ObjectDetectee(Image<Gray,Byte> image) {
            _objectImage = image;
            extractData();
        }

        public ObjectDetectee(string filename)
        {
            _objectImage = new Image<Gray, Byte>(filename);
            extractData();           
        }
    }
}
