#pragma once
#include "AudioBIIPEffect.h"


static short center_freq;
static short samp_freq;
static short counter;
static short counter_limit;
static short control;
static short max_freq;
static short min_freq;
static short f_step;
static struct bp_filter HWah;


extern "C" __declspec(dllexport) void  AutoWahInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, short freq_step) {

	center_freq = 0;
	samp_freq = sampling;
	counter = effect_rate;
	control = 0;

	counter_limit = effect_rate;

	min_freq = 0;
	max_freq = (maxf - minf) / freq_step;

	bp_iir_init(sampling, gainfactor, Q, freq_step, minf);
	f_step = freq_step;
}


extern "C" __declspec(dllexport) float  AutoWahProcess(float xin) {
	float yout;

	yout = bp_iir_filter(xin, &HWah);
#ifdef INPUT_UNSIGNED
	yout += 32767;
#endif

	return yout;
}


extern "C" __declspec(dllexport) void  AutoWahSweep(void) {

	if (!--counter) {
		if (!control) {
			bp_iir_setup(&HWah, (center_freq += f_step));
			if (center_freq > max_freq) {
				control = 1;
			}
		}
		else if (control) {
			bp_iir_setup(&HWah, (center_freq -= f_step));
			if (center_freq == min_freq) {
				control = 0;
			}
		}

		counter = counter_limit;
	}
		}

