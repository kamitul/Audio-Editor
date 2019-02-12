using DSPEditor.DSPAudioEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess.AudioEffectsItems
{
    class ReverbEffect : AudioEffect, IArrayProcessable
    {
        public ReverbEffect()
        {
            audioEffectParameters = new Dictionary<string, object>();
        }

        public void ProcessArray(object[] arraySamplesProcess, int beginIndex, int endIndex, ref int timeElapsed)
        {
            float[] samples = Utility.Utilty.ToFloatArray(arraySamplesProcess);
            AudioReverbEffect.AddReverbEffect(ref samples, beginIndex, endIndex, ref timeElapsed);
        }

        protected override void InitalizeConcreteAudioEffect()
        {
            AudioReverbEffect.ReverbEffectInit((int)audioEffectParameters["Delay"],
                (float)audioEffectParameters["Decay"]);
        }
    }
}
