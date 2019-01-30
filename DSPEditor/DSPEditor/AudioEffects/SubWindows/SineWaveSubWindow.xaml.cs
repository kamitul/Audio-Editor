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
using System.Windows.Shapes;

namespace DSPEditor.AudioEffects
{
    /// <summary>
    /// Interaction logic for SineWaveSubWindow.xaml
    /// </summary>
    public partial class SineWaveSubWindow : Window
    {
        public Action<int, double> SinWaveStartProcessing;

        public SineWaveSubWindow()
        {
            InitializeComponent();
        }

        private void SendParamsToProcess(object sender, RoutedEventArgs e)
        {
            if (SinWaveStartProcessing != null)
            {
                SinWaveStartProcessing(FreqRate.Value.Value, Amplitude.Value.Value);
            }
        }
    }
}
