using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioTremoloEffect
    {
        static double dep;
        static short counter_limit;
        static short control;
        static short mod;
        static double offset;

        public static float TremoloProcess(float xin)
        {
            float yout;
            float m;

            m = (float)(mod * dep / counter_limit);
            yout = (float)((m + offset) * xin);
            return yout;
        }

        public static void TremoloSweep()
        {
            mod += control;

            if (mod > counter_limit)
            {
                control = -1;
            }
            else if (mod < 0)
            {
                control = 1;
            }
        }

        public static void TremoloInit(short effect_rate, double depth)
        {
            dep = depth;
            control = 1;
            mod = 0;
            counter_limit = effect_rate;
            offset = 1 - dep;
        }
    }
}
