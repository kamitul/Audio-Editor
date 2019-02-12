using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.AudioEffectProcess
{
    interface IArrayProcessable
    {
        void ProcessArray(object[] arraySamplesProcess, int beginIndex, int endIndex, ref int timeElapsed);
    }
}
