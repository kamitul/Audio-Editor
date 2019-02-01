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
    /// Interaction logic for FlangerSubWindow.xaml
    /// </summary>
    public partial class FlangerSubWindow : Window
    {
        public Action<int, int, int, double, double, double> FlangerStartProcessing;

        public FlangerSubWindow()
        {
            InitializeComponent();
        }

        private void SendParamsToProcess(object sender, RoutedEventArgs e)
        {
            if (FlangerStartProcessing != null)
            {
                FlangerStartProcessing(EffectRate.Value.Value, MaxD.Value.Value, MinD.Value.Value, FeedbackVolume.Value.Value, SampleStep.Value.Value, TapMix.Value.Value);
            }
        }
    }
}
