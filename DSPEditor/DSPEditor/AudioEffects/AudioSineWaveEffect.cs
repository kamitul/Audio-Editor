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
        private static extern float SineWaveProcess(IntPtr tab, int sampleCount, int sampleRate);

        public static unsafe void AddSineWave(float[] samples, int sampleRate)
        {
            fixed (float* p = samples)
            {
                SineWaveProcess((IntPtr)p, samples.Length, sampleRate);
            }
        }
    }
}
