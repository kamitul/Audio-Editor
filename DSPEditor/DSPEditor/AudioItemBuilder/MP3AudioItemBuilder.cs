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
        Mp3FileReader mp3FileReader = null;

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
            mp3FileReader = new Mp3FileReader(filePath);

            Debug.Assert(reader.WaveFormat.BitsPerSample != 16, "Only works with 16 bit audio");
            var samples = new float[reader.Length / 2];
            reader.Read(samples, 0, samples.Length / 2);

            audioItem.OriginalAudioBuffer = samples;
            audioItem.FilePath = filePath;
            audioItem.WaveFormat = mp3FileReader.WaveFormat;
            
        }

        public WaveStream GetFileReader()
        {
            return mp3FileReader;
        }
    }
}
