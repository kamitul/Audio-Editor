using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioItemBuilder
{
    class BPIIR
    {
        public class bp_coeffs
        {
            public double e;
            public double p;
            public double[] d = new double[3];

            public bp_coeffs()
            {

            }
        };

        public class bp_filter
        {
            public double e;
            public double p;
            public double[] d = new double[3];
            public double[] x = new double[3];
            public double[] y = new double[3];

            public bp_filter()
            {

            }
        };

        public static bp_coeffs[] bp_coeff_arr;

        public static void bp_iir_init(double fsfilt, double gb, double Q, short fstep, short fmin)
        {
            bp_coeff_arr = new bp_coeffs[120];

            int i;
            double damp;
            double wo;

            damp = gb / Math.Sqrt((1 - Math.Pow(gb, 2)));

            for (i = 0; i < 120; i++)
            {
                bp_coeff_arr[i] = new bp_coeffs();
                wo = 2 * 3.14 * (fstep * i + fmin) / fsfilt;
                bp_coeff_arr[i].e = 1 / (1 + damp * Math.Tan(wo / (Q * 2)));
                bp_coeff_arr[i].p = Math.Cos(wo);
                bp_coeff_arr[i].d[0] = (1 - bp_coeff_arr[i].e);
                bp_coeff_arr[i].d[1] = 2 * bp_coeff_arr[i].e * bp_coeff_arr[i].p;
                bp_coeff_arr[i].d[2] = (2 * bp_coeff_arr[i].e - 1);
            }
        }

        public static void bp_iir_setup(bp_filter H, int ind)
        {
            H.e = bp_coeff_arr[ind].e;
            H.p = bp_coeff_arr[ind].p;
            H.d[0] = bp_coeff_arr[ind].d[0];
            H.d[1] = bp_coeff_arr[ind].d[1];
            H.d[2] = bp_coeff_arr[ind].d[2];

        }

        public static float bp_iir_filter(float yin, bp_filter H) {
            float yout;

            H.x[0] = H.x[1];
            H.x[1] = H.x[2];
            H.x[2] = yin;

            H.y[0] = H.y[1];
            H.y[1] = H.y[2];

            H.y[2] = H.d[0] * H.x[2] - H.d[0] * H.x[0] + (H.d[1] * H.y[1]) - H.d[2] * H.y[0];

            yout = (float)H.y[2];

            return yout;
        }
    }
}
