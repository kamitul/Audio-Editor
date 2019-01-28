using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.DSPAudioEffects
{
    class AudioReverbEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReverbInit(int delay, float _decay);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReverbProcess(IntPtr tab, int length, int begin_index, int end_index);

        public static unsafe void AddReverb(float[] samples, int begin_index, int end_index)
        {
            fixed (float* p = samples)
            {
                ReverbProcess((IntPtr)p, samples.Length, begin_index, end_index);
            }
        }
    }
}
