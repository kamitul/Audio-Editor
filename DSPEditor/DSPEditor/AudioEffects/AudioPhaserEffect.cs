using DSPEditor.AudioItemBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DSPEditor.AudioItemBuilder.BPIIR;

namespace DSPEditor.AudioEffects
{
    class AudioPhaserEffect
    {
        private static short center_freq;
        private static short samp_freq;
        private static short counter;
        private static short counter_limit;
        private static short control;
        private static short max_freq;
        private static short min_freq;
        private static double pha_mix;
        private static short f_step;
        private static double dir_mix;
        private static bp_filter[] H;


        public static void PhaserInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, double pha_mixume, short freq_step, double dmix)
        {
            BPIIR.bp_iir_init(sampling, gainfactor, Q, freq_step, minf);

            H = new bp_filter[20];

            for(int i = 0; i < 20; ++i)
            {
                H[i] = new bp_filter();
            }

            center_freq = 0;
            samp_freq = sampling;
            counter = effect_rate;
            control = 0;
            counter_limit = effect_rate;

            min_freq = 0;
            max_freq = (short)((maxf - minf) / freq_step);

            pha_mix = pha_mixume;
            f_step = freq_step;
            dir_mix = dmix;
        }

  
        public static float PhaserProcess(float xin)
        {
            float yout;
            int i;

            yout = BPIIR.bp_iir_filter(xin, H[0]);

            for (i = 1; i < 20; i++)
            {
                yout = BPIIR.bp_iir_filter(yout, H[i]);
            }

            yout = (float)(dir_mix * xin + pha_mix * yout);

            return yout;
        }


        public static void PhaserSweep()
        {
            int i;

            if (--counter < 0)
            {
                if (control < 0)
                {
                    center_freq += f_step;

                    if (center_freq > max_freq)
                    {
                        control = 1;
                    }
                }
                else if (control > 0)
                {
                    center_freq -= f_step;

                    if (center_freq == min_freq)
                    {
                        control = 0;
                    }
                }
                for (i = 0; i < 20; i++)
                {
                    BPIIR.bp_iir_setup(H[i], center_freq);
                }
                counter = counter_limit;
            }
        }

    }
}
