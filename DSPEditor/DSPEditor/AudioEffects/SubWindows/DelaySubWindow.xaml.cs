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
    /// Interaction logic for DelaySubWindow.xaml
    /// </summary>
    public partial class DelaySubWindow : Window
    {
        public Action<double, double> DelayStartProcessing;

        public DelaySubWindow()
        {
            InitializeComponent();
        }

        private void SendParamsToProcess(object sender, RoutedEventArgs e)
        {
            if (DelayStartProcessing != null)
            {
                DelayStartProcessing(FeedbackLevelDelay.Value.Value, DelayDecay.Value.Value);
            }
        }
    }
}
