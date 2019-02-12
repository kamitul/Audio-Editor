#pragma once

#include <chrono>

extern "C" void reverb_init(int delay, float _decay);
extern "C" void reverb_process(double * tab, int begin, int end);

extern "C" __declspec(dllexport) void ReverbInitASM(int delay, float _decay) {
	reverb_init(delay, (double)_decay);
}

extern "C" __declspec(dllexport) void ReverbProcessASM(double * tab, int lengt, int begin_index, int end_index, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	reverb_process(tab, begin_index, end_index);

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;
}