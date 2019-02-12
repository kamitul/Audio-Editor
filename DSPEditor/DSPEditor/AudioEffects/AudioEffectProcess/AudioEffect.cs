using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess
{
    public abstract class AudioEffect
    {
        protected Dictionary<string, object> audioEffectParameters;

        public virtual void AudioEffectInitialize<T>(params Tuple<String, T>[] parametersToInitialize)
        {
            foreach (Tuple<string, T> elem in parametersToInitialize)
            {
                audioEffectParameters.Add(elem.Item1, elem.Item2);
            }

            InitalizeConcreteAudioEffect();
        }

        protected abstract void InitalizeConcreteAudioEffect();
    }
}
