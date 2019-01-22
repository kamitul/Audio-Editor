#pragma once


static double dep;
static short tremolo_counter_limit;
static short tremolo_control;
static short mod;
static double offset;

extern "C" __declspec(dllexport) void TremoloInit(short effect_rate, double depth) {
	dep = depth;
	tremolo_control = 1;
	mod = 0;
	tremolo_counter_limit = effect_rate;
	offset = 1 - dep;
}

extern "C" __declspec(dllexport) float TremoloProcess(float xin) {
	float yout;
	float m;

	m = (float)(mod*dep / tremolo_counter_limit);
	yout = (float)((m + offset)*xin);
	return yout;
}

extern "C" __declspec(dllexport) void TremoloSweep(void) {

	mod += tremolo_control;

	if (mod > tremolo_counter_limit) {
		tremolo_control = -1;
	}
	else if (!mod) {
		tremolo_control = 1;
	}
}
