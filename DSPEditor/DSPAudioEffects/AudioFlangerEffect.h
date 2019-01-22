#pragma once


#include "AudioDelayEffect.h"


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
	DelayInit(2, fbv, fwv, 1);

	flanger_samp_freq = sampling;
	flanger_counter = effect_rate;
	flanger_control = 1;
	var_delay = mind;

	//User Parameters
	flanger_counter_limit = effect_rate;
	max_delay = maxd;
	min_delay = mind;
	mix_vol = 1;
	delay_step = stepd;
}


extern "C" __declspec(dllexport) float FlangerProcess(float xin) {
	float yout;

	yout = DelayTask(xin);
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

