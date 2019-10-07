using DSPEditor.Audio;
using NAudio.Wave;
using System.Diagnostics;

namespace DSPEditor.AudioItemBuilder
{
    public class WAVAudioItemBuilder : AudioBuilder, IAudioItemBuilder
    {
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
            fileReader = new AudioFileReader(filePath);
            waveStream = new WaveFileReader(filePath);

            Debug.Assert(fileReader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
            var samples = new float[fileReader.Length / 2];
            fileReader.Read(samples, 0, samples.Length / 2);
            LoadAudioItem(filePath, samples);
        }

        private void LoadAudioItem(string filePath, float[] samples)
        {
            audioItem.OriginalAudioBuffer = samples;
            audioItem.FilePath = filePath;
            audioItem.WaveFormat = fileReader.WaveFormat;
        }

    }
}
