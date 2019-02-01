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

namespace DSPEditor.AudioEffects.SubWindows
{
    /// <summary>
    /// Interaction logic for WahWahSubWindow.xaml
    /// </summary>
    public partial class WahWahSubWindow : Window
    {
        public Action<int, int, int, int, double> WahStartProcessing;

        public WahWahSubWindow()
        {
            InitializeComponent();

        }

        private void SendParamsToProcess(object sender, RoutedEventArgs e)
        {
            if (WahStartProcessing != null)
            {
                WahStartProcessing(EffectRateWah.Value.Value, MaxF.Value.Value, MinF.Value.Value, Q.Value.Value, GainFactor.Value.Value);
            }
        }
    }
}
