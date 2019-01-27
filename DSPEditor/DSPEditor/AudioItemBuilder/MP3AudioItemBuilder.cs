﻿using DSPEditor.Audio;
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
        AudioFileReader reader = null;
        WaveFileReader waveFileReader = null;

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
            reader = new AudioFileReader(filePath);
            waveFileReader = new WaveFileReader(filePath);

            Debug.Assert(reader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
            var samples = new float[reader.Length / 2];
            reader.Read(samples, 0, samples.Length / 2);

            audioItem.ProcessedAudioBuffer = samples;
            audioItem.FilePath = filePath;
            audioItem.WaveFormat = reader.WaveFormat;
            
        }

        public WaveFileReader GetFileReader()
        {
            return waveFileReader;
        }
    }
}
