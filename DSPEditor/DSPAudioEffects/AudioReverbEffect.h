#pragma once

static int delayMilliseconds;
static int delaySamples;
static float decay;

extern "C" __declspec(dllexport) void ReverbInit(int delay, float _decay) {
	delayMilliseconds = delay;
	decay = _decay;
	delaySamples = (int)((float)delayMilliseconds * 44.1f); //44100 Hz sample rate
}

extern "C" __declspec(dllexport) void ReverbProcess(float * tab, int length) {

	for (int i = 0; i < length - delaySamples; i++) {

		tab[i] += tab[i];
		tab[i + delaySamples] += tab[i] * decay;
	}

}