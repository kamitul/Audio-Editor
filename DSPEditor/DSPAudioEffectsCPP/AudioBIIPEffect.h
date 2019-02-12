#pragma once
#include <math.h>

#define BP_MAX_COEFS 120
#define PI 3.1415926


struct  bp_coeffs  {
	double e;
	double p;
	double d[3];
};

struct bp_filter {
	double e;
	double p;
	double d[3];
	double x[3];
	double y[3];
};


bp_coeffs bp_coeff_arr[BP_MAX_COEFS];


extern "C" __declspec(dllexport) void bp_iir_init(double fsfilt, double gb, double Q, short fstep, short fmin) {
	int i;
	double damp;
	double wo;

	damp = gb / sqrt(1 - pow(gb, 2));

	for (i = 0; i < BP_MAX_COEFS; i++) {
		wo = 2 * PI*(fstep*i + fmin) / fsfilt;
		bp_coeff_arr[i].e = 1 / (1 + damp * tan(wo / (Q * 2)));
		bp_coeff_arr[i].p = cos(wo);
		bp_coeff_arr[i].d[0] = (1 - bp_coeff_arr[i].e);
		bp_coeff_arr[i].d[1] = 2 * bp_coeff_arr[i].e*bp_coeff_arr[i].p;
		bp_coeff_arr[i].d[2] = (2 * bp_coeff_arr[i].e - 1);
	}
}


extern "C" __declspec(dllexport) void bp_iir_setup(struct bp_filter * H, int ind) {
	H->e = bp_coeff_arr[ind].e;
	H->p = bp_coeff_arr[ind].p;
	H->d[0] = bp_coeff_arr[ind].d[0];
	H->d[1] = bp_coeff_arr[ind].d[1];
	H->d[2] = bp_coeff_arr[ind].d[2];
}


extern "C" __declspec(dllexport) float bp_iir_filter(float yin, struct bp_filter * H) {
	float yout;

	H->x[0] = H->x[1];
	H->x[1] = H->x[2];
	H->x[2] = yin;

	H->y[0] = H->y[1];
	H->y[1] = H->y[2];

	H->y[2] = H->d[0] * H->x[2] - H->d[0] * H->x[0] + (H->d[1] * H->y[1]) - H->d[2] * H->y[0];

	yout = (float)(H->y[2]);

	return yout;
}
