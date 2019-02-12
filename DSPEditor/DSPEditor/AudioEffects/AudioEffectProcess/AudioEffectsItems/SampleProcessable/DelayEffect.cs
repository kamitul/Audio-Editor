using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class DelayEffect : AudioEffect, ISampleProcessable
    {
        public DelayEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void PorcessSample(object sampleToProcess, ref int timeElapsed)
        {
            AudioDelayEffect.DelayProcess((float)sampleToProcess, ref timeElapsed);
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioDelayEffect.DelayInit((double)audioEffectParameters["Feedback Level"],
                (double)audioEffectParameters["Delay Level"]);
        }
    }
}
