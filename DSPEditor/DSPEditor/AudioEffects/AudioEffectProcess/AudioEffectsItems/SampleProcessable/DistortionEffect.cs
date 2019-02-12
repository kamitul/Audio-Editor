using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class DistortionEffect : AudioEffect, ISampleProcessable
    {
        public DistortionEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void PorcessSample(object sampleToProcess, ref int timeElapsed)
        {
            AudioDistortionEffect.DistortionEffectProcess((float)sampleToProcess, ref timeElapsed);
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioDistortionEffect.DistortionEffectInit((float)audioEffectParameters["Maxmimum Value"]);
        }
    }
}
