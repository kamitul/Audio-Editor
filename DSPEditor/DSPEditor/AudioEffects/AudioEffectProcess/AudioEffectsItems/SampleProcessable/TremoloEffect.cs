using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    [Serializable]
    class TremoloEffect : AudioEffect, ISampleProcessable
    {
        public TremoloEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void PorcessSample(object sampleToProcess, ref int timeElapsed)
        {
            AudioTremoloEffect.TremoloEffectProcess((float)sampleToProcess, ref timeElapsed);
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioTremoloEffect.TremoloEffectInit((short)audioEffectParameters["Effect Rate"], (double)audioEffectParameters["Depth Rate"]);
        }
    }
}
