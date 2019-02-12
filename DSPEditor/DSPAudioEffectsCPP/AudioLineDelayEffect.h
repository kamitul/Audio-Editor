#pragma once

#include "math.h"

#define MAX_BUF_SIZE 64000

static double d_buffer[MAX_BUF_SIZE];


struct fract_delay {
	double d_mix;       
	short d_samples;	
	double d_fb;	    
	double d_fw;	    
	double n_fract;     
	double *rdPtr;      
	double *wrtPtr;     
};

static fract_delay del{};


extern "C" __declspec(dllexport) void LineDelayInit(double delay_samples, double dfb, double dfw, double dmix) {
	
	del.d_samples = (short)floor(delay_samples);
	del.n_fract = (delay_samples - del.d_samples); 
	
	del.d_fb = dfb;

	del.d_fw = dfw;

	del.d_mix = dmix;

	del.wrtPtr = &d_buffer[MAX_BUF_SIZE - 1];
}

extern "C" __declspec(dllexport) void DelaySetFb(double val) {
	del.d_fb = val;
}

extern "C" __declspec(dllexport) void DelaySetFw(double val) {
	del.d_fw = val;
}

extern "C" __declspec(dllexport) void DelaySetMix(double val) {
	del.d_mix = val;
}

extern "C" __declspec(dllexport) void DelaySetDelay(double n_delay) {

	del.d_samples = (short)floor(n_delay);

	del.n_fract = (n_delay - del.d_samples);
}

extern "C" __declspec(dllexport) double DelayGetFb(void) {
	return del.d_fb;
}

extern "C" __declspec(dllexport) double DelayGetFw(void) {
	return del.d_fw;
}

extern "C" __declspec(dllexport) double DelayGetMix(void) {
	return del.d_mix;
}


extern "C" __declspec(dllexport) float LineDelayTask(float xin) {
	float yout;
	double * y0;
	double * y1;
	double x1;
	double x_est;

	del.rdPtr = del.wrtPtr - (short)del.d_samples;

	if (del.rdPtr < d_buffer) {
		del.rdPtr += MAX_BUF_SIZE - 1;
	}

	y0 = del.rdPtr - 1;
	y1 = del.rdPtr;

	if (y0 < d_buffer) {
		y0 += MAX_BUF_SIZE - 1;
	}

	x_est = (*(y0)-*(y1))*del.n_fract + *(y1);

	x1 = xin + x_est * del.d_fb;

	*(del.wrtPtr) = x1;

	yout = (float)(x1 * del.d_mix + x_est * del.d_fw);

	del.wrtPtr++;

	if ((del.wrtPtr - &d_buffer[0]) > MAX_BUF_SIZE - 1) {
		del.wrtPtr = &d_buffer[0];
	}
	return yout;
}


