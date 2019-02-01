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
    /// Interaction logic for ReverbSubWindow.xaml
    /// </summary>
    public partial class ReverbSubWindow : Window
    {
        public Action<int, double> ReverbStartProcessing;

        public ReverbSubWindow()
        {
            InitializeComponent();
        }

        private void SendParamsToProcess(object sender, RoutedEventArgs e)
        {
            if (ReverbStartProcessing != null)
            {
                ReverbStartProcessing(DelayRate.Value.Value, Decay.Value.Value);
            }
        }
    }
}
