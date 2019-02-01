using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DSPEditor.Audio
{
    [Serializable]
    public class AudioItem
    {
        private string filePath;
        private float[] processedAudioBuffer;
        private float[] originalAudioBuffer;
        private WaveFormat waveFormat;
      

        public float[] ProcessedAudioBuffer { get => processedAudioBuffer; set => processedAudioBuffer = value; }
        public string FilePath { get => filePath; set => filePath = value; }
        public float[] OriginalAudioBuffer { get => originalAudioBuffer; set => originalAudioBuffer = value; }
        public WaveFormat WaveFormat { get => waveFormat; set => waveFormat = value; }
    }
}
