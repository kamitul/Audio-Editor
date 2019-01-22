#pragma once
#include "AudioBIIREffect.h"

#define PH_STAGES 20


static short phaser_center_freq;    /*Center frequency counter*/
static short phaser_samp_freq;      /*Sampling frequency*/
static short phaser_counter;        /*Smaple counter*/
static short phaser_counter_limit;  /*Smaple counter limit*/
static short phaser_control;        /*LFO Control*/
static short phaser_max_freq;       /*Maximum notch center frequency*/
static short phaser_min_freq;       /*Minimum notch center frequency*/
static double phaser_pha_mix;       /*Filtered signal mix*/
static short phaser_f_step;         /*Sweep frequency step*/
static double phaser_dir_mix;       /*Direct signal mix*/
static br_filter HPhaser[PH_STAGES]; /*Array of notch filters stages*/


extern "C" __declspec(dllexport) void PhaserInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, double pha_mixume, short freq_step, double dmix) {
	br_iir_init(sampling, gainfactor, Q, freq_step, minf);

	phaser_center_freq = 0;
	phaser_samp_freq = sampling;
	phaser_counter = effect_rate;
	phaser_control = 0;
	phaser_counter_limit = effect_rate;

	phaser_min_freq = 0;
	phaser_max_freq = (maxf - minf) / freq_step;

	phaser_pha_mix = pha_mixume;
	phaser_f_step = freq_step;
	phaser_dir_mix = dmix;
}


extern "C" __declspec(dllexport) float PhaserProcess(float xin) {
	float yout;
	int i;

	yout = br_iir_filter(xin, &HPhaser[0]);

	for (i = 1; i < PH_STAGES; i++) {
		yout = br_iir_filter(yout, &HPhaser[i]);
	}

	yout = (float)(phaser_dir_mix * xin + phaser_pha_mix * yout);

	return yout;
}


extern "C" __declspec(dllexport) void PhaserSweep(void) {
	int i;

	if (!--phaser_counter) {
		if (!phaser_control) {
			phaser_center_freq += phaser_f_step;

			if (phaser_center_freq > phaser_max_freq) {
				phaser_control = 1;
			}
		}
		else if (phaser_control) {
			phaser_center_freq -= phaser_f_step;

			if (phaser_center_freq == phaser_min_freq) {
				phaser_control = 0;
			}
		}
		for (i = 0; i < PH_STAGES; i++) {
			br_iir_setup(&HPhaser[i], phaser_center_freq);
		}
		phaser_counter = phaser_counter_limit;
	}
}

