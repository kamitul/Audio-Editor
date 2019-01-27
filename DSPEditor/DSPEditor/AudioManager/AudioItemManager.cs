using DSPEditor.AudioItemBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using DSPEditor.AudioManager;
using DSPEditor.Utility;
using NAudio.Wave;

namespace DSPEditor.Audio
{
    public enum AudioType
    {
        MP3,
        WAV,
        UNDEFINED
    }

    public class AudioItemManager
    {
        private static AudioItemManager instance = null;
        private static readonly object padlock = new object();
        private static AudioUIPlayer audioUIPlayer = new AudioUIPlayer();

        private OutputLogWriter outputLogWriter = new OutputLogWriter();

        public static Action<string> WriteToOutputLog;

        private static IAudioItemBuilder audioItemBuilder;

        public static AudioItemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioItemManager();
                }
                return instance;
             
            }
        }

        public static void SetAudioItem(AudioItem value)
        {
            if (audioItemBuilder != null)
                ((AudioBuilder)audioItemBuilder).AudioItem = value;
            else
                return;

        }

        public static AudioItem GetAudioItem()
        {
            if (audioItemBuilder != null)
                return ((AudioBuilder)audioItemBuilder).AudioItem;
            else
                return null;
        }

        AudioItemManager()
        {
            outputLogWriter.SubscribeToAudioItemEvent();
        }

        private void SetFilePath(string filePath)
        {
            try
            {
                audioItemBuilder.SetFullPath(filePath);
            }
            catch (NullReferenceException)
            {

            }
        }

        private void SetAudioType(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.MP3:
                    audioItemBuilder = new MP3AudioItemBuilder();
                    break;
                case AudioType.WAV:
                    audioItemBuilder = new WAVAudioItemBuilder();
                    break;
                case AudioType.UNDEFINED:
                    audioItemBuilder = null;
                    break;
            }

        }

        public void InitializeAudioBuilder(string filePath)
        {
            AudioType audioType = CheckFileExtension(Path.GetExtension(filePath));
            SetAudioType(audioType);
            SetFilePath(filePath);
            audioUIPlayer.SetAudioPlayer(audioItemBuilder.GetFileReader(), filePath);

            if (WriteToOutputLog != null)
                WriteToOutputLog("Initalized audio file: " + Path.GetFileName(filePath));
        }


        private AudioType CheckFileExtension(string fileExtension)
        {
            switch(fileExtension)
            {
                case ".mp3":
                    return AudioType.MP3;
                case ".wav":
                    return AudioType.WAV;
                default:
                    return AudioType.UNDEFINED;
            }
        }

        public void PlayAudio()
        {
            if (audioUIPlayer != null)
            {
                if (audioUIPlayer.CanPlay)
                {
                    audioUIPlayer.Play();
                    if (WriteToOutputLog != null)
                        WriteToOutputLog("Playing audio!");
                }

            }
            else
            {
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Audio not loaded!");
            }
        }

        public void StopAudio()
        {
            if (audioUIPlayer != null)
            {
                if (audioUIPlayer.CanStop)
                {
                    audioUIPlayer.Stop();
                    if (WriteToOutputLog != null)
                        WriteToOutputLog("Stopped audio!");
                }
            }
            else
            {
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Audio not loaded!");
            }
        }

        public void PauseAudio()
        {
            if (audioUIPlayer != null)
            {
                if (audioUIPlayer.CanPause)
                {
                    audioUIPlayer.Pause();
                    if (WriteToOutputLog != null)
                        WriteToOutputLog("Paused audio!");
                }
            }
            else
            {
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Audio not loaded!");
            }
        }

        public void MuteAudio()
        {
            if (audioUIPlayer != null)
            {
                if (audioUIPlayer.CanMute)
                {
                    audioUIPlayer.Mute();
                    if (WriteToOutputLog != null)
                        WriteToOutputLog("Muted audio!");
                }

            }
            else
            {
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Audio not loaded!");
            }
        }

        public void ChangeVolume(double value)
        {
            if (audioUIPlayer != null)
            {
                audioUIPlayer.ChangeVolume(value);
            }
            else
            {
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Audio not loaded!");
            }
        }

        public void Dispose()
        {
            if (audioUIPlayer != null)
            {
                audioUIPlayer.Dispose();
                if (WriteToOutputLog != null)
                    WriteToOutputLog("Disposed AudioUIPlayer!");
            }
        }

        public TimeSpan GetBeginSpan()
        {
            return audioUIPlayer.SelectionBegin;
        }

        public TimeSpan GetEndSpan()
        {
            return audioUIPlayer.SelectionEnd;
        }

        public WaveStream GetActiveStream()
        {
            return audioUIPlayer.ActiveStream;
        }
    }
}
