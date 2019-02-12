using DSPEditor.AudioEffects.CppLibraryImports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioDistortionEffect : AudioEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DistortionInit(float max);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float DistortionProcess(float in_value, ref int time_elapsed);

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DistortionInitASM(float max);

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float DistortionProcessASM(float in_value, ref int time_elapsed);

        public static void DistortionEffectInit(float max)
        {
            switch (DllType)
            {
                case DllType.MASM:
                    DistortionInitASM(max);
                    break;
                case DllType.Cpp:
                    DistortionInit(max);
                    break;
            }
        }

        public static float DistortionEffectProcess(float in_sample, ref int time_elapsed)
        {

            float out_value = in_sample;

            switch (DllType)
            {
                case DllType.MASM:
                    out_value = DistortionProcessASM(in_sample, ref time_elapsed);
                    break;
                case DllType.Cpp:
                    out_value = DistortionProcess(in_sample, ref time_elapsed);
                    break;
            }

            return out_value;
        }
    }
}
