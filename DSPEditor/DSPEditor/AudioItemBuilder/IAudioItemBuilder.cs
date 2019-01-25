using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPEditor.Audio;

namespace DSPEditor.AudioItemBuilder
{
    public interface IAudioItemBuilder
    {
        void SetFullPath(string filePath);
        void OpenAudioFile(string filePath);
    }
}
