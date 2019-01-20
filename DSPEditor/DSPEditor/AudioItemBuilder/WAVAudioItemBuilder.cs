using DSPEditor.Audio;
using DSPEditor.AudioEffects;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioItemBuilder
{
    public class WAVAudioItemBuilder : AudioBuilder, IAudioItemBuilder
    {


        public WAVAudioItemBuilder()
        {
            audioItem = new AudioItem();
        }

        public void SetFullPath(string filePath)
        {
            OpenWAVFile(filePath);
        }

        private void OpenWAVFile(string filePath)
        {

            //using (WaveFileReader reader = new WaveFileReader(filePath))
            //{

            //    byte[] bytesBuffer = new byte[reader.Length];
            //    int read = reader.Read(bytesBuffer, 0, bytesBuffer.Length);

            //    int samplesRequired = bytesBuffer.Length / 2;
            //    bytesBuffer = BufferHelpers.Ensure(bytesBuffer, samplesRequired);

            //    float max = -(float)System.Int16.MinValue;
            //    float[] samples = new float[bytesBuffer.Length / 2];

            //    for (int i = 0; i < samples.Length; i++)
            //    {
            //        short int16sample = System.BitConverter.ToInt16(bytesBuffer, i * 2);
            //        samples[i] = (float)int16sample / max;
            //    }

            //    audioItem.FilePath = filePath;
            //    audioItem.AudioBuffer = samples;

            //    float[] floatOutput = audioItem.AudioBuffer.Select(s => (float)s).ToArray();
            //    WaveFormat waveFormat = new WaveFormat(16000, 8, 2);
            //    using (WaveFileWriter writer = new WaveFileWriter("C:\\Users\\Kamil\\Source\\Repos\\JA_K.Tulczyjew_EdytorDzwiekow\\DSPEditor\\DSPEditor\\track1.wav", waveFormat))
            //    {
            //        writer.WriteSamples(floatOutput, 0, floatOutput.Length);
            //    }

            //}

            

            using (AudioFileReader reader = new AudioFileReader(filePath))
            {
                Debug.Assert(reader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
                var samples = new float[reader.Length/2];
                reader.Read(samples, 0, samples.Length/2);

                AudioTremoloEffect.TremoloInit(4000, 1);

                for (int i = 0; i < samples.Length; ++i)
                {
                    samples[i] = AudioTremoloEffect.TremoloProcess((float)(0.7 * samples[i]));
                    AudioTremoloEffect.TremoloSweep();
                }

                using (WaveFileWriter writer = new WaveFileWriter("C:\\Users\\Kamil\\Source\\Repos\\JA_K.Tulczyjew_EdytorDzwiekow\\DSPEditor\\DSPEditor\\track1.wav", reader.WaveFormat))
                {
                    writer.WriteSamples(samples, 0, samples.Length/2);
                }
            }
        }

        
    }
}
