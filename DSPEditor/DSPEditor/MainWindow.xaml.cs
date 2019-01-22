using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

namespace DSPEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        }

        private void FlangerSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.AddFlangerEffect();
        }

        private void TremoloSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.AddTremoloEffect();
        }

        private void ChorusSample(object sender, RoutedEventArgs e)
        {
           // AudioItemManager.Instance.
        }

        private void FuzzSample(object sender, RoutedEventArgs e)
        {

        }

        private void EchoSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.AddDelayEffect();
        }

        private void ReverbSample(object sender, RoutedEventArgs e)
        {

        }

        private void WahWahSample(object sender, RoutedEventArgs e)
        {
            AudioItemManager.Instance.AddWahWahEffect();
        }

        private void PhaserSample(object sender, RoutedEventArgs e)
        {

        }

        private void SoundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void MuteSample(object sender, RoutedEventArgs e)
        {

        }

        private void StopSample(object sender, RoutedEventArgs e)
        {

        }

        private void PauseSample(object sender, RoutedEventArgs e)
        {

        }

        private void PlaySample(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
