using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DSPEditor.AudioEffects.CppLibraryImports;

namespace DSPEditor.AudioEffects
{
    class AudioTremoloEffect : AudioImportEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TremoloInit(short effect_rate, double depth);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float TremoloProcess(float xin, ref int time_elapsed);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TremoloSweep();

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TremoloInitASM(short effect_rate, double depth);

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float TremoloProcessASM(float xin, ref int time_elapsed);


        public static void TremoloEffectInit(short effectRateValue, double depthValue)
        {
            switch(DllType)
            {
                case DllType.MASM:
                    TremoloInitASM(effectRateValue, depthValue);
                    break;
                case DllType.Cpp:
                    TremoloInit(effectRateValue, depthValue);
                        break;
            }
        }

        public static float TremoloEffectProcess(float inSample, ref int timeElapsed)
        {

            float outValue = inSample;

            switch (DllType)
            {
                case DllType.MASM:
                    outValue = TremoloProcessASM(inSample, ref timeElapsed);
                    break;
                case DllType.Cpp:
                    outValue = TremoloProcess(inSample, ref timeElapsed);
                    TremoloSweep();
                    break;
            }

            return outValue;
        }
    }
}
