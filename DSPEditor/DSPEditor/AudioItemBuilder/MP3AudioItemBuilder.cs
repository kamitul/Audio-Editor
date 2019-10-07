using DSPEditor.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioItemBuilder
{
    public class MP3AudioItemBuilder : AudioBuilder, IAudioItemBuilder
    {
        public MP3AudioItemBuilder()
        {
            AudioItem = new AudioItem();
        }

        public void SetFullPath(string filePath)
        {
            OpenAudioFile(filePath);
        }

        public void OpenAudioFile(string filePath)
        {
            fileReader = new AudioFileReader(filePath);
            waveStream = new Mp3FileReader(filePath);

            Debug.Assert(fileReader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
            var samples = new float[fileReader.Length / 2];
            fileReader.Read(samples, 0, samples.Length / 2);
            LoadAudioItemData(filePath, samples);
        }

        private void LoadAudioItemData(string filePath, float[] samples)
        {
            audioItem.OriginalAudioBuffer = samples;
            audioItem.FilePath = filePath;
            audioItem.WaveFormat = waveStream.WaveFormat;
        }
    }
}
