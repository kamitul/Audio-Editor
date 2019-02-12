#pragma once

#include <chrono>

extern "C" void distortion_init(double max, double min);
extern "C" double distortion_process(double in_sample);

extern "C" __declspec(dllexport) void DistortionInitASM(float max) {
	distortion_init((double)max, (double)-max);
}

extern "C" __declspec(dllexport) float DistortionProcessASM(float in_value, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	double out_value = distortion_process((double)in_value);

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

	return (float)out_value;
}