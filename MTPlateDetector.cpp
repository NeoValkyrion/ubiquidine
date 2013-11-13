#include "MTPlateDetector.h"


MTPlateDetector::MTPlateDetector(std::string filename)
{
	init(filename);
}


MTPlateDetector::~MTPlateDetector(void)
{
}

bool MTPlateDetector::detect(cv::Mat const &color_img){
	good_matches.clear();
	kp_image.clear();
	cv::Mat image, des_image, img_matches;
	cvtColor(color_img, image, CV_RGB2GRAY);
	detector.detect( image, kp_image );
	extractor.compute( image, kp_image, des_image );

	if(!des_image.empty() && !des_object.empty()) {
			matcher.knnMatch(des_object, des_image, matches, 2);
	}

	for(int i = 0; i < cv::min(des_image.rows-1,(int) matches.size()); i++) //this loop is sensitive to segfaults
        {
            if((matches[i][0].distance < 0.8*(matches[i][1].distance)) && ((int) matches[i].size()<=2 && (int) matches[i].size()>0))
            {
                good_matches.push_back(matches[i][0]);
            }
        }

	if(good_matches.size() >= min_good_matches) {
		return true;
	} else {
		return false;
	}
}
