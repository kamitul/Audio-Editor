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

namespace DSPEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static ProgressBar progressBar;
        public static WaveformTimeline waveFormTimeLine;

        public MainWindow()
        {
            InitializeComponent();
            progressBar = AlgoTime;
            waveFormTimeLine = waveform;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"c:\";
            if (openFileDialog.ShowDialog() == true)
            {
                AudioItemManager.Instance.InitializeAudioBuilder(openFileDialog.FileName);
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
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

        private void ExportWAVFile(object sender, RoutedEventArgs e)
        {
            using (WaveFileWriter writer = new WaveFileWriter("C:\\Users\\Kamil\\Source\\Repos\\JA_K.Tulczyjew_EdytorDzwiekow\\DSPEditor\\DSPEditor\\track1.wav", AudioItemManager.GetAudioItem().WaveFormat))
            {
                writer.WriteSamples(AudioItemManager.GetAudioItem().ProcessedAudioBuffer, 0, AudioItemManager.GetAudioItem().ProcessedAudioBuffer.Length / 2);
            }

        }

        private void FlangerSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddFlangerEffect();
        }

        private void TremoloSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddTremoloEffect();
        }

        private void ChorusSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddChorusEffect();
        }

        private void EchoSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddDelayEffect();
        }

        private void ReverbSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddReverbEffect();
        }

        private void WahWahSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddWahWahEffect();
        }

        private void PhaserSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddPhaserEffect();
        }

        private void SineWaveSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddSineWaveEffect();
        }

        private void DistortionSample(object sender, RoutedEventArgs e)
        {
            AudioEffectManager.Instance.AddDistortionEffect();
        }

        private void SoundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void MuteSample(object sender, RoutedEventArgs e)
        {
            
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
    }
}
