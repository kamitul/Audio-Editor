#pragma once

static int delayMilliseconds;
static int delaySamples;
static float decay;

extern "C" __declspec(dllexport) void ReverbInit(int delay, float _decay) {
	delayMilliseconds = delay;
	decay = _decay;
	delaySamples = (int)((float)delayMilliseconds * 44.1f); //44100 Hz sample rate
}

extern "C" __declspec(dllexport) void ReverbProcess(float * tab, int lengt, int begin_index, int end_index, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	for (int i = begin_index; i < end_index - delaySamples; i++) {

		tab[i] += tab[i];
		tab[i + delaySamples] += tab[i] * decay;
	}

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

}

