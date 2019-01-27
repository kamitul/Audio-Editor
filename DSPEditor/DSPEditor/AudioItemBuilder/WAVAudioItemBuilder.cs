using DSPEditor.Audio;
using NAudio.Wave;
using System.Diagnostics;

namespace DSPEditor.AudioItemBuilder
{
    public class WAVAudioItemBuilder : AudioBuilder, IAudioItemBuilder
    {
        AudioFileReader reader = null;
        WaveFileReader wavReader = null;

        public WAVAudioItemBuilder()
        {
            AudioItem = new AudioItem();
        }

        public void SetFullPath(string filePath)
        {
            OpenAudioFile(filePath);
        }

        public void OpenAudioFile(string filePath)
        {
            reader = new AudioFileReader(filePath);
            wavReader = new WaveFileReader(filePath);

            Debug.Assert(reader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
            var samples = new float[reader.Length / 2];
            reader.Read(samples, 0, samples.Length / 2);

            audioItem.OriginalAudioBuffer = samples;
            audioItem.FilePath = filePath;
            audioItem.WaveFormat = reader.WaveFormat;
        }

        public WaveFileReader GetWaveStream()
        {
            return wavReader;
        }

        public string GetFilePath()
        {
            return audioItem.FilePath;
        }
    }
}
