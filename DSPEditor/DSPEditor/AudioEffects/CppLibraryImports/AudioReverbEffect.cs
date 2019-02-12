using DSPEditor.AudioEffects.CppLibraryImports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.DSPAudioEffects
{
    class AudioReverbEffect : AudioEffect
    {
        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReverbInit(int delay, float _decay);

        [DllImport("DSPAudioEffectsCpp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReverbProcess(IntPtr tab, int length, int begin_index, int end_index, ref int time_elapsed);

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReverbInitASM(int delay, float _decay);

        [DllImport("DSPAudioEffectsASM.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReverbProcessASM(IntPtr tab, int length, int begin_index, int end_index, ref int time_elapsed);

        public static void ReverbEffectInit(int delay, float _decay)
        {
            switch(DllType)
            {
                case DllType.Cpp:
                    ReverbInit(delay, _decay);
                    break;
                case DllType.MASM:
                    ReverbInitASM(delay, _decay);
                    break;
            }
        }

        public static unsafe void AddReverbEffect(ref float[] samples, int begin_index, int end_index, ref int time_elapsed)
        {
            switch(DllType)
            {
                case DllType.Cpp:
                    fixed (float* p = samples)
                    {
                        ReverbProcess((IntPtr)p, samples.Length, begin_index, end_index, ref time_elapsed);
                    }
                    break;
                case DllType.MASM:
                    double[] output = ConvertToDoubleArray(samples);
                    fixed (double* p = output)
                    {
                        ReverbProcessASM((IntPtr)p, samples.Length, begin_index, end_index, ref time_elapsed);
                    }
                    samples = FillDoubleArray(output);
                    break;
            }
           
        }

        private static float[] FillDoubleArray(double[] samples)
        {
            float[] output = new float[samples.Length];
            for (int i = 0; i < samples.Length; i++)
                output[i] = (float)(samples[i]);
            return output;
        }

        private static unsafe double[] ConvertToDoubleArray(float[] samples)
        {
            double[] output = new double[samples.Length];
            for (int i = 0; i < samples.Length; i++)
                output[i] = samples[i];
            return output;
        }
    }
}
