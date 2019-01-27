using DSPEditor.Audio;
using DSPEditor.AudioManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPEditor.Utility
{
    class OutputLogWriter
    {
        private StringBuilder stringBuilder = new StringBuilder();
        private static string outputString = "";

        public OutputLogWriter()
        {
            
        }

        public void SubscribeToAudioEffectEvent()
        {
            AudioEffectManager.WriteToOutputLog += WriteToOutputLog;
        }

        public void SubscribeToAudioItemEvent()
        {
            AudioItemManager.WriteToOutputLog += WriteToOutputLog;
        }

        public void SubscripeToOpeningClosingAudioEvents()
        {
            MainWindow.AudioFileOpenedExported += WriteToOutputLog;
        }

        public void WriteToOutputLog(string log)
        {
            stringBuilder.Clear();
            stringBuilder.Append("---------------------------------------------------------------LOG--------------------------------------------------------------");
            stringBuilder.Append("\n");
            stringBuilder.Append("");
            stringBuilder.Append("Time : " + DateTime.Now.ToString());
            stringBuilder.Append("\n");
            stringBuilder.Append(log);
            stringBuilder.Append("\n");

            outputString += stringBuilder.ToString();

            MainWindow.scrollViewerText.Text += stringBuilder.ToString();
            MainWindow.scrollViewerLog.ScrollToEnd();
        }

        public static void WriteOutputLogToFile(string filePath)
        {
            File.WriteAllText(filePath, outputString);
        }
    }
}
