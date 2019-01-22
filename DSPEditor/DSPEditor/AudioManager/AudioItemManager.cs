using DSPEditor.AudioItemBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

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

        public void AddDelayEffect()
        {

        }

        public void AddFlangerEffect()
        {

        }

        public void AddPhaserEffect()
        {

        }

        public void AddTremoloEffect()
        {

        }

        public void AddWahWahEffect()
        {
            
        }

    }
}
