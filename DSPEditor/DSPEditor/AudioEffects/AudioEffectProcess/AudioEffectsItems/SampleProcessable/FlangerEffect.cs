using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class FlangerEffect : AudioEffect, ISampleProcessable
    {
        public FlangerEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void PorcessSample(object sampleToProcess, ref int timeElapsed)
        {
            AudioFlangerEffect.FlangerProcess((float)sampleToProcess, ref timeElapsed);
            AudioFlangerEffect.FlangerSweep();
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioFlangerEffect.FlangerInit((short)audioEffectParameters["Effect Rate"],
                (short)audioEffectParameters["Sampling Rate"],
                (short)audioEffectParameters["MaxmiumD"],
                (short)audioEffectParameters["MinimumD"],
                (double)audioEffectParameters["FWV"],
                (double)audioEffectParameters["Step delay"],
                (double)audioEffectParameters["FBV"]);
        }
    }
}
