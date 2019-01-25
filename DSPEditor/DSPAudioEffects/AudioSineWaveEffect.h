#pragma once

#include <math.h>

static int sample = 0;
static int frequency;
static int amplitude;

extern "C" __declspec(dllexport) void SineWaveInit(int freq, int amp) {

	frequency = freq;
	amplitude = amp;    
}

extern "C" __declspec(dllexport) void SineWaveProcess(float* buffer, int sampleCount, int sampleRate) {

	int offset = sampleCount / 2;
	for (int n = 0; n < sampleCount - offset; n++)
	{
		buffer[n + offset] = (float)(amplitude * sin((2 * 3.14 * sample * frequency) / sampleRate));
		sample++;
		if (sample >= sampleRate) sample = 0;
	}
}