using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioDelayEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DelayInit(double delay_samples, double dfb, double dfw, double dmix);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float DelayTask(float xin);
    }
}
