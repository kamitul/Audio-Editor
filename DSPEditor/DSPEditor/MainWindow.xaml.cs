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

namespace DSPEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static ProgressBar progressBar;
        public static WaveformTimeline waveFormTimeLine;
        public static DigitalClock digitalClock;
        public static TimeEditor startTime;
        public static TimeEditor stopTime;
        public static TextBlock scrollViewerText;
        public static ScrollViewer scrollViewerLog;

        private OutputLogWriter outputLogWriter = new OutputLogWriter();

        public static Action<string> AudioFileOpenedExported;

        public MainWindow()
        {
            InitializeComponent();
            progressBar = AlgoTime;
            waveFormTimeLine = waveform;
            digitalClock = clockDisplay;
            startTime = repeatStartTimeEdit;
            stopTime = repeatStopTimeEdit;
            scrollViewerText = OutputLogText;
            scrollViewerLog = OutputLog;

            outputLogWriter.SubscripeToOpeningClosingAudioEvents();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"c:\";
            if (openFileDialog.ShowDialog() == true)
            {
                if(openFileDialog.FileName == "")
                {
                    if (AudioFileOpenedExported != null)
                    {
                        AudioFileOpenedExported("Audio file not opened!");
                    }
                }
                else
                {
                    AudioItemManager.Instance.InitializeAudioBuilder(openFileDialog.FileName);
                }
            }
            else
            {
                if (AudioFileOpenedExported != null)
                {
                    AudioFileOpenedExported("Audio file not opened!");
                }
            }
           
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

        private void ExportMP3File(object sender, RoutedEventArgs e)
        {

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
                if (AudioFileOpenedExported != null)
                {
                    if(filePath != null)
                        AudioFileOpenedExported("Exported output log to: " + filePath);
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
                        writer.WriteSamples(AudioItemManager.GetAudioItem().ProcessedAudioBuffer, 0, AudioItemManager.GetAudioItem().ProcessedAudioBuffer.Length / 2);
                    }
                    if (AudioFileOpenedExported != null)
                    {
                        AudioFileOpenedExported("Exported edited audio file, filePath: " + filename);
                    }
                }
            }
            else
            {
                if (AudioFileOpenedExported != null)
                {
                    AudioFileOpenedExported("Cannot export empty file!");
                }
            }

        }

        private void FlangerSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddFlangerEffect();
        }

        private void TremoloSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddTremoloEffect();
        }

        private void ChorusSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddChorusEffect();
        }

        private void DelaySample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddDelayEffect();
        }

        private void ReverbSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddReverbEffect();
        }

        private void WahWahSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddWahWahEffect();
        }

        private void PhaserSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddPhaserEffect();
        }

        private void SineWaveSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddSineWaveEffect();
        }

        private void DistortionSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.SetThreadValue(ThreadsValue.Value);
            AudioEffectManager.Instance.AddDistortionEffect();
        }

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
    }
}
