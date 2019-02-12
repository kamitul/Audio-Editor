using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DSPEditor.AudioEffects.CppLibraryImports;

namespace DSPEditor.AudioEffects
{
    class AudioFlangerEffect : AudioImportEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FlangerInit(short effect_rate, short sampling, short maxd, short mind, double fwv, double stepd, double fbv);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float FlangerProcess(float xin, ref int time_elapsed);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FlangerSweep();
    }
}
