using DSPEditor.AudioItemBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using DSPEditor.AudioManager;

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


        private static IAudioItemBuilder audioItemBuilder;

        public static AudioItemManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AudioItemManager();
                    }
                    return instance;
                }
            }
        }

        internal static void SetAudioItem(AudioItem value)
        {
            ((AudioBuilder)audioItemBuilder).AudioItem = value;
        }

        internal static AudioItem GetAudioItem()
        {
            return ((AudioBuilder)audioItemBuilder).AudioItem;
        }

        AudioItemManager()
        {
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
            audioUIPlayer.SetAudioPlayer(audioItemBuilder.GetWaveStream());
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

        internal void PlayAudio()
        {
            audioUIPlayer.Play();
        }

        internal void StopAudio()
        {
            audioUIPlayer.Stop();
            
        }

        internal void PauseAudio()
        {
            audioUIPlayer.Pause();
        }
    }
}
