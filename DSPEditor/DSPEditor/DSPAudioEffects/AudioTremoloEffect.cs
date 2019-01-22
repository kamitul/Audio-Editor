using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace DSPEditor.AudioEffects
{
    class AudioTremoloEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TremoloInit(short effect_rate, double depth);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float TremoloProcess(float xin);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TremoloSweep();
    }
}
