using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPEditor.Audio;
using NAudio.Wave;

namespace DSPEditor.AudioItemBuilder
{
    public interface IAudioItemBuilder
    {
        void SetFullPath(string filePath);
        void OpenAudioFile(string filePath);
        WaveFileReader GetWaveStream();
    }
}
