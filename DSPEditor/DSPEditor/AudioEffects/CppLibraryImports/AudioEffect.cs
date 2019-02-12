using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.AudioEffects.CppLibraryImports
{
    public enum DllType
    {
        Cpp,
        MASM
    }

    public abstract class AudioEffect
    {
        private static DllType dllType;

        public static DllType DllType { get => dllType; set => dllType = value; }
    }
}
