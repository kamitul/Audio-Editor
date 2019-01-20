using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DSPEditor.Audio
{
    [Serializable]
    public struct AudioItem
    {
        private string filePath;
        private float[] audioBuffer;

        public float[] AudioBuffer { get => audioBuffer; set => audioBuffer = value; }
        public string FilePath { get => filePath; set => filePath = value; }
    }
}
