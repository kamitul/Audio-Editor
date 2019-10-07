using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class SineWaveEffect : AudioEffect, IArrayProcessable
    {
        int sampleRate;

        public SineWaveEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void ProcessArray(object[] arraySamplesProcess, int beginIndex, int endIndex, ref int timeElapsed)
        {
            AudioSineWaveEffect.AddSineWave(Utility.Utilty.ToFloatArray(arraySamplesProcess), sampleRate, beginIndex, endIndex, ref timeElapsed);
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioSineWaveEffect.SineWaveInit (
                    (int)audioEffectParameters["Frequency"],
                    (float)audioEffectParameters["Amplitude"]
                );

            sampleRate = (int)audioEffectParameters["Sample Rate"];
        }
    }
}
