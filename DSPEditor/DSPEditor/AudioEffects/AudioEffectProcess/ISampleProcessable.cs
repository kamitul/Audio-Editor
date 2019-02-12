using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess
{
    interface ISampleProcessable
    {
        void PorcessSample(object sampleToProcess, ref int timeElapsed);
    }
}
