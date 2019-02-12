#pragma once


#include "AudioLineDelayEffect.h"


static short flanger_samp_freq;
static double var_delay;
static short flanger_counter;
static short flanger_counter_limit;
static short flanger_control;
static short max_delay;
static short min_delay;
static double mix_vol;
static double delay_step;

extern "C" __declspec(dllexport) void FlangerInit(short effect_rate, short sampling, short maxd, short mind, double fwv, double stepd, double fbv) {
	LineDelayInit(2, fbv, fwv, 1);

	flanger_samp_freq = sampling;
	flanger_counter = effect_rate;
	flanger_control = 1;
	var_delay = mind;

	flanger_counter_limit = effect_rate;
	max_delay = maxd;
	min_delay = mind;
	mix_vol = 1;
	delay_step = stepd;
}


extern "C" __declspec(dllexport) float FlangerProcess(float xin, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	float yout;

	yout = LineDelayTask(xin);

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

	return yout;
}


extern "C" __declspec(dllexport) void FlangerSweep(void) {
	if (!--flanger_counter) {
		var_delay += flanger_control * delay_step;

		if (var_delay > max_delay) {
			flanger_control = -1;
		}

		if (var_delay < min_delay) {
			flanger_control = 1;
		}

		DelaySetDelay(var_delay);
		flanger_control = flanger_counter_limit;
	}
}

