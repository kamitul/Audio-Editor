#pragma once
#include <chrono>

static float max_value_dist;
static float min_value_dist;

extern "C" __declspec(dllexport) void DistortionInit(float max) {
	max_value_dist = max;
	min_value_dist = -max_value_dist;
}

extern "C" __declspec(dllexport) float DistortionProcess(float in_value, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	float out;

	if (in_value > max_value_dist) {
		out = max_value_dist * 1.5;
	}
	else if (in_value * 1000 < min_value_dist) {
		out = min_value_dist * 1.5;
	}
	else {
		out = in_value;
	}

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

	return out;
}