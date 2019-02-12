using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class WahWahEffect : AudioEffect, ISampleProcessable
    {
        public WahWahEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void PorcessSample(object sampleToProcess, ref int timeElapsed)
        {
            AudioWahWahEffect.AutoWahProcess((float)sampleToProcess, ref timeElapsed);
            AudioWahWahEffect.AutoWahSweep();
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioWahWahEffect.AutoWahInit((short)audioEffectParameters["Effect Rate"],
                (short)audioEffectParameters["Sampling Frequency"],
                (short)audioEffectParameters["Maximum Frequency"],
                (short)audioEffectParameters["MinimumFrequency"],
                (short)audioEffectParameters["Q"],
                (double)audioEffectParameters["Gain Factor"],
                (short)audioEffectParameters["Fequency Increment"]);
        }
    }
}
