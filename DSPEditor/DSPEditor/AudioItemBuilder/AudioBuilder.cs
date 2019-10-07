using DSPEditor.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioItemBuilder
{
    public abstract class AudioBuilder
    {
        protected AudioItem audioItem;
        protected AudioFileReader fileReader = default;
        protected WaveStream waveStream = default;

        public AudioItem AudioItem { get => audioItem; set => audioItem = value; }

        public WaveStream GetFileReader()
        {
            return waveStream;
        }
    }
}
