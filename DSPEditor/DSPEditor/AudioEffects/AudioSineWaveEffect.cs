using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioSineWaveEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SineWaveInit(int freq, float amp);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SineWaveProcess(IntPtr tab, int sampleCount, int sampleRate, int begin_index, int end_index);

        public static unsafe void AddSineWave(float[] samples, int sampleRate, int begin_index, int end_index)
        {
            fixed (float* p = samples)
            {
                SineWaveProcess((IntPtr)p, samples.Length, sampleRate, begin_index, end_index);
            }
        }
    }
}
