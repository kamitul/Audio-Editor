using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioWahWahEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AutoWahInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, short freq_step);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float AutoWahProcess(float xin);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AutoWahSweep();
    }
}
