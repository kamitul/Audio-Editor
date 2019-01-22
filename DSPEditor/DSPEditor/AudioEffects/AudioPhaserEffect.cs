using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioPhaserEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PhaserInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, double pha_mixume, short freq_step, double dmix);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float PhaserProcess(float xin);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PhaserSweep();
    }
}
