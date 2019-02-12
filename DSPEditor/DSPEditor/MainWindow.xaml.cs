using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using DSPEditor.Audio;
using DSPEditor.AudioManager;
using NAudio.Wave;
using WPFSoundVisualizationLib;
using DSPEditor.Utility;
using DSPEditor.AudioEffects;
using DSPEditor.AudioEffects.SubWindows;
using NAudio.Lame;

namespace DSPEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window paramsSubWindow;

        public static ProgressBar progressBar;
        public static WaveformTimeline waveFormTimeLine;
        public static DigitalClock digitalClock;
        public static TimeEditor startTime;
        public static TimeEditor stopTime;
        public static TextBlock scrollViewerText;
        public static ScrollViewer scrollViewerLog;

        private OutputLogWriter outputLogWriter = new OutputLogWriter();

        public static Action<string> MainWindowLogAction;

        public MainWindow()
        {
            InitializeComponent();
            int coreCount = GetThreadsCount();
            ThreadsValue.Maximum = coreCount;
            progressBar = AlgoTime;
            waveFormTimeLine = waveform;
            digitalClock = clockDisplay;
            startTime = repeatStartTimeEdit;
            stopTime = repeatStopTimeEdit;
            scrollViewerText = OutputLogText;
            scrollViewerLog = OutputLog;

            outputLogWriter.SubscripeToOpeningClosingAudioEvents();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.Dispose();
            Application.Current.Shutdown();  
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {

        }

        private void LoadProject(object sender, RoutedEventArgs e)
        {

        }

        #region FileOperations

        private void ExportMP3File(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            var audioItem = AudioItemManager.GetAudioItem();
            if (audioItem != null)
            {
                saveFileDialog.FileName = System.IO.Path.GetFileName(audioItem.FilePath) + "processed"; // Default file name
                saveFileDialog.DefaultExt = ".mp3";
                saveFileDialog.Filter = "MP3 files (.mp3)|*.mp3";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(filename.Remove(filename.Length - 4, 4));
                    stringBuilder.Append(".wav");

                    filename = stringBuilder.ToString();

                    stringBuilder.Clear();

                    using (WaveFileWriter writer = new WaveFileWriter(filename, AudioItemManager.GetAudioItem().WaveFormat))
                    {
                        if (AudioItemManager.GetAudioItem() != null)
                        {
                            writer.WriteSamples(AudioItemManager.GetAudioItem().AudioBuffer, 0, AudioItemManager.GetAudioItem().AudioBuffer.Length / 2);
                            writer.Dispose();
                            writer.Close();

                            filename = ConvertWAVToMp3File(filename, stringBuilder);

                            DeleteWAVFileAfterConversion(filename, stringBuilder);
                        }
                        else
                            MainWindowLogAction("Cannot export not changed file! " + filename);
                    }
                    if (MainWindowLogAction != null)
                    {
                        MainWindowLogAction("Exported edited audio file, filePath: " + filename);
                    }
                }
            }
            else
            {
                if (MainWindowLogAction != null)
                {
                    MainWindowLogAction("Cannot export empty file!");
                }
            }
        }

        private void DeleteWAVFileAfterConversion(string filename, StringBuilder stringBuilder)
        {
            stringBuilder.Clear();
            stringBuilder.Append(filename.Remove(filename.Length - 4, 4));
            stringBuilder.Append(".wav");

            filename = stringBuilder.ToString();

            File.Delete(filename);
        }

        private static string ConvertWAVToMp3File(string filename, StringBuilder stringBuilder)
        {
            using (var reader = new WaveFileReader(filename))
            {
                stringBuilder.Append(filename.Remove(filename.Length - 4, 4));
                stringBuilder.Append(".mp3");

                filename = stringBuilder.ToString();

                using (var wtr = new LameMP3FileWriter(filename, reader.WaveFormat, 128))
                {
                    reader.CopyTo(wtr);
                    reader.Dispose();
                    wtr.Dispose();
                }
            }

            return filename;
        }


        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"c:\";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName == "")
                {
                    if (MainWindowLogAction != null)
                    {
                        MainWindowLogAction("Audio file not opened!");
                    }
                }
                else
                {
                    AudioItemManager.Instance.InitializeAudioBuilder(openFileDialog.FileName);
                }
            }
            else
            {
                if (MainWindowLogAction != null)
                {
                    MainWindowLogAction("Audio file not opened!");
                }
            }

        }

        private void ExportOutputLog(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "outputlog"; // Default file name
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                OutputLogWriter.WriteOutputLogToFile(filePath);
                if (MainWindowLogAction != null)
                {
                    if(filePath != null)
                        MainWindowLogAction("Exported output log to: " + filePath);
                }
            }
        }

        private void ExportWAVFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            var audioItem = AudioItemManager.GetAudioItem();
            if (audioItem != null) {
                saveFileDialog.FileName = System.IO.Path.GetFileName(audioItem.FilePath) + "processed"; // Default file name
                saveFileDialog.DefaultExt = ".wav";
                saveFileDialog.Filter = "Wav files (.wav)|*.wav";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    using (WaveFileWriter writer = new WaveFileWriter(filename, AudioItemManager.GetAudioItem().WaveFormat))
                    {
                        if (AudioItemManager.GetAudioItem() != null)
                        {
                            writer.WriteSamples(AudioItemManager.GetAudioItem().AudioBuffer, 0, AudioItemManager.GetAudioItem().AudioBuffer.Length / 2);
                            writer.Dispose();
                            writer.Close();
                        }
                        else
                            MainWindowLogAction("Cannot export not changed file! " + filename);

                        writer.Dispose();
                        writer.Close();
                    }
                    if (MainWindowLogAction != null)
                    {
                        MainWindowLogAction("Exported edited audio file, filePath: " + filename);
                    }
                }
            }
            else
            {
                if (MainWindowLogAction != null)
                {
                    MainWindowLogAction("Cannot export empty file!");
                }
            }

        }

        #endregion

        #region Flanger
        private void FlangerSample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new FlangerSubWindow();
            ((FlangerSubWindow)paramsSubWindow).FlangerStartProcessing += ProcessFlangerSample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessFlangerSample(int effect_rate, int maxd, int mind, double fwv, double stepd, double fbv)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddFlangerEffect(effect_rate, maxd, mind, fwv, stepd, fbv);
        }

        private static int GetThreadsCount()
        {
            int coreCount = 0;
            int threadsCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                threadsCount += int.Parse(item["NumberOfLogicalProcessors"].ToString());
            }

            return coreCount * threadsCount * 4;
        }

        #endregion

        #region Tremolo
        private void TremoloSample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new TremoloSubWindow();
            ((TremoloSubWindow)paramsSubWindow).TremoloStartProcessing += ProcessTremoloSample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessTremoloSample(int effectRate, double depth)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddTremoloEffect(effectRate, depth);
        }

        #endregion

        #region Chours
        private void ChorusSample(object sender, RoutedEventArgs e)
        {
            int coreCount = GetThreadsCount();
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddChorusEffect();
        }

        #endregion

        #region Delay
        private void DelaySample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new DelaySubWindow();
            ((DelaySubWindow)paramsSubWindow).DelayStartProcessing += ProcessDelaySample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessDelaySample(double feedback_level, double delay_decay)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddDelayEffect(feedback_level, delay_decay);
        }

        #endregion

        #region Reverb
        private void ReverbSample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new ReverbSubWindow();
            ((ReverbSubWindow)paramsSubWindow).ReverbStartProcessing += ProcessReverbSample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessReverbSample(int delay, double decay)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddReverbEffect(delay, decay);
        }

        #endregion

        #region WahWah
        private void WahWahSample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new WahWahSubWindow();
            ((WahWahSubWindow)paramsSubWindow).WahStartProcessing += ProcessWahWahSample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessWahWahSample(int effect_rate, int maxf, int minf, int Q, double gainfactor)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddWahWahEffect(effect_rate, maxf, minf, Q, gainfactor);
        }

        #endregion

        #region Pahser
        private void PhaserSample(object sender, RoutedEventArgs e)
        {
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddPhaserEffect();
        }

        #endregion

        #region SineWave
        private void SineWaveSample(object sender, RoutedEventArgs e)
        {
            paramsSubWindow = new SineWaveSubWindow();
            ((SineWaveSubWindow)paramsSubWindow).SinWaveStartProcessing += ProcessSineWaveSample;
            paramsSubWindow.ShowDialog();
        }

        private void ProcessSineWaveSample(int freq, double ampl)
        {
            paramsSubWindow.Close();
            paramsSubWindow = null;
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddSineWaveEffect(freq, ampl);
        }

        #endregion

        #region Distortion
        private void DistortionSample(object sender, RoutedEventArgs e)
        {
            int coreCount = GetThreadsCount();
            if (ThreadsValue.Value > coreCount)
                ThreadsValue.Value = coreCount;
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddDistortionEffect();
        }

        #endregion

        private void MuteSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.MuteAudio();
        }

        private void StopSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.StopAudio();
        }

        private void PauseSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.PauseAudio();
        }

        private void PlaySample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.PlayAudio();
        }

        private void ChangeVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            double value = slider.Value;

            AudioItemManager.Instance.ChangeVolume(value);
        }

        private void NullText(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("ClearLog") as ContextMenu;
            cm.PlacementTarget = sender as ScrollViewer;
            cm.IsOpen = true;
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            OutputLogText.Text = "";
        }

        private void CppSelect(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.dllType = AudioEffects.CppLibraryImports.DllType.Cpp;
            if (MainWindowLogAction != null)
            {
                MainWindowLogAction("Chnaged implementation mode to C++");
            }
        }

        private void MasmSelect(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.dllType = AudioEffects.CppLibraryImports.DllType.MASM;
            if (MainWindowLogAction != null)
            {
                MainWindowLogAction("Chnaged implementation mode to MASM, architecture x64");
            }
        }
    }
}
