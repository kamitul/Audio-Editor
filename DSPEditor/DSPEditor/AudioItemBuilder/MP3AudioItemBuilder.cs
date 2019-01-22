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
            audioItem = new AudioItem();
        }

        public void SetFullPath(string filePath)
        {
            OpenAudioFile(filePath);
        }

        public void OpenAudioFile(string filePath)
        {
            using (AudioFileReader reader = new AudioFileReader(filePath))
            {
                Debug.Assert(reader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
                var samples = new float[reader.Length / 2];
                reader.Read(samples, 0, samples.Length / 2);

                audioItem.AudioBuffer = samples;
                audioItem.FilePath = filePath;
            }
        }
    }
}
