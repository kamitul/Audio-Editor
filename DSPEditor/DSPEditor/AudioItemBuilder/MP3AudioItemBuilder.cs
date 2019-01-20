using DSPEditor.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
            OpenWAVFile(filePath);
        }

        private void OpenWAVFile(string filePath)
        {
            using (Mp3FileReader reader = new Mp3FileReader(filePath))
            {
                byte[] bytesBuffer = new byte[reader.Length];
                int read = reader.Read(bytesBuffer, 0, bytesBuffer.Length);
                var floatSamples = new double[read / 2];
                for (int sampleIndex = 0; sampleIndex < read / 2; sampleIndex++)
                {
                    var intSampleValue = BitConverter.ToInt16(bytesBuffer, sampleIndex * 2);
                    floatSamples[sampleIndex] = intSampleValue / 32768.0;
                }

                audioItem.FilePath = filePath;
                //audioItem.AudioBuffer = floatSamples;
            }
        }
    }
}
