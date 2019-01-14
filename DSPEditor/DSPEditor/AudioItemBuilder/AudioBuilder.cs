using DSPEditor.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioItemBuilder
{
    public abstract class AudioBuilder
    {
        private AudioItem audioItem;

        public AudioItem AudioItem { get => audioItem; set => audioItem = value; }
    }
}
