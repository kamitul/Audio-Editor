using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects
{
    class AudioDistortionEffect
    {
        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DistortionInit(float max);

        [DllImport("DSPAudioEffects.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float DistortionProcess(float in_value);
    }
}
