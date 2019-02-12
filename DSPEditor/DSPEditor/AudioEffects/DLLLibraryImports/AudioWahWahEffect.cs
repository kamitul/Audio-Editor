using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DSPEditor.AudioEffects.CppLibraryImports;

namespace DSPEditor.AudioEffects
{
    class AudioWahWahEffect : AudioImportEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AutoWahInit(short effect_rate, short sampling, short maxf, short minf, short Q, double gainfactor, short freq_step);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float AutoWahProcess(float xin, ref int time_elapsed);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AutoWahSweep();
    }
}
