#pragma once

#include <chrono>

#define MAX_DELAY_LENGTH 44100

float buffer[MAX_DELAY_LENGTH * 4];
int offsetIndex;
int currentIndex;
int bufferLength;
int delay_length;
double feedbackLevel;
double delayLevel;

extern "C" __declspec(dllexport) void DelayInit(double _feedbackLevel, double _delayLevel) {

	int i = 0;
	for (i = 0; i < MAX_DELAY_LENGTH; i++) buffer[i] = 0;

	offsetIndex = 44100 / 2;
	currentIndex = 0;
	bufferLength = 44100;
	feedbackLevel = _feedbackLevel;
	delayLevel = _delayLevel;


}

extern "C" __declspec(dllexport) float DelayProcess(float in_sample, int *time_elapsed) {

	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();

	buffer[offsetIndex] *= (float)feedbackLevel;
	buffer[offsetIndex] += (double)in_sample * delayLevel;

	in_sample += (float)buffer[currentIndex];

	if (in_sample >= 1.0) in_sample = 0.99;
	if (in_sample <= -1.0) in_sample = -0.99;

	currentIndex = (currentIndex + 1) % bufferLength;
	offsetIndex = (offsetIndex + 1) % bufferLength;

	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::microseconds>(t2 - t1).count();

	*time_elapsed = (int)duration;

	return in_sample;
}