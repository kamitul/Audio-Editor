#pragma once
#include <chrono>

extern "C" void tremolo_init(short effect_rate, double tremolo_depth_value);
extern "C" double tremolo_process(double in_sample);

extern "C" __declspec(dllexport) void TremoloInitASM(short effect_rate, double depth) {
	tremolo_init(effect_rate, depth);
}

extern "C" __declspec(dllexport) float TremoloProcessASM(float in_value, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	double out_value = tremolo_process((double)in_value);

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

	return (float)out_value;
}