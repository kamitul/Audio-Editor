using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DSPEditor.AudioEffects.CppLibraryImports;

namespace DSPEditor.AudioEffects
{
    class AudioPhaserEffect : AudioEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PhaserInit(double rateParam, double widthParam);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float PhaserProcess(float inval, ref int time_elapsed);

    }
}
