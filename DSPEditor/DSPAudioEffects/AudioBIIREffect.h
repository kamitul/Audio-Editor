#pragma once
#include <math.h>

#define BR_MAX_COEFS 120
#define PI 3.1415926

struct br_coeffs {
	double e;
	double p;
	double d[3];
};


struct br_filter {
	double e;
	double p;
	double d[3];
	double x[3];
	double y[3];
};

static br_coeffs  br_coeff_arr[BR_MAX_COEFS];


extern "C" __declspec(dllexport) void br_iir_init(double fsfilt, double gb, double Q, double fstep, short fmin) {
	int i;
	double damp;
	double wo;

	damp = sqrt(1 - pow(gb, 2)) / gb;

	for (i = 0; i < BR_MAX_COEFS; i++) {
		wo = 2 * PI*(fstep*i + fmin) / fsfilt;
		br_coeff_arr[i].e = 1 / (1 + damp * tan(wo / (Q * 2)));
		br_coeff_arr[i].p = cos(wo);
		br_coeff_arr[i].d[0] = br_coeff_arr[i].e;
		br_coeff_arr[i].d[1] = 2 * br_coeff_arr[i].e*br_coeff_arr[i].p;
		br_coeff_arr[i].d[2] = (2 * br_coeff_arr[i].e - 1);
	}
}


extern "C" __declspec(dllexport) void br_iir_setup(struct br_filter * H, int ind) {
	H->e = br_coeff_arr[ind].e;
	H->p = br_coeff_arr[ind].p;
	H->d[0] = br_coeff_arr[ind].d[0];
	H->d[1] = br_coeff_arr[ind].d[1];
	H->d[2] = br_coeff_arr[ind].d[2];
}


extern "C" __declspec(dllexport) float br_iir_filter(float yin, struct br_filter * H) {
	float yout;

	H->x[0] = H->x[1];
	H->x[1] = H->x[2];
	H->x[2] = yin;

	H->y[0] = H->y[1];
	H->y[1] = H->y[2];

	H->y[2] = H->d[0] * H->x[2] - H->d[1] * H->x[1] + H->d[0] * H->x[0] + H->d[1] * H->y[1] - H->d[2] * H->y[0];

	yout = (float)(H->y[2]);
	return yout;
}

