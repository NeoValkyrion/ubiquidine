#pragma once
#include<string>
#include<exception>
#include "opencv2/core/core.hpp"
#include "opencv2/features2d/features2d.hpp"
#include "opencv2/nonfree/nonfree.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/calib3d/calib3d.hpp"

class MTPlateDetector
{
public:
	MTPlateDetector(std::string filename);
	~MTPlateDetector(void);

	bool detect(cv::Mat const &mat);

private:
	cv::SIFT detector;
	cv::SIFT extractor;
    std::vector<cv::KeyPoint> kp_object;
	cv::Mat object, des_object, img_object;
	cv::FlannBasedMatcher matcher;
	std::vector<std::vector<cv::DMatch > > matches;
	std::vector<cv::DMatch > good_matches;
	std::vector<cv::KeyPoint> kp_image;
	unsigned static const int min_good_matches = 25;

	void init(std::string filename) {
		object = cv::imread( filename, CV_LOAD_IMAGE_GRAYSCALE );
		if(!object.data) {
			throw std::runtime_error("Error opening file" + filename);
		}
		cv::initModule_nonfree();
		matches.reserve(100000);
		detector.detect( object, kp_object );
		extractor.compute( object, kp_object, des_object);
	}
};

