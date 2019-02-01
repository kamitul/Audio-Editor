using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DSPEditor.Utility
{
    class UIUtility
    {
        public static void Bind(object dataSource, string sourcePath, FrameworkElement destinationObject, DependencyProperty dp, BindingMode bindingMode)
        {
            Bind(dataSource, sourcePath, destinationObject, dp, null, bindingMode, null);
        }

        public static void Bind(object dataSource, string sourcePath, FrameworkElement destinationObject, DependencyProperty dp, string stringFormat, BindingMode bindingMode, IValueConverter converter)
        {
            Binding binding = new Binding
            {
                Source = dataSource,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = bindingMode,
                Path = new PropertyPath(sourcePath),
                StringFormat = stringFormat,
                Converter = converter
            };
            destinationObject.SetBinding(dp, binding);
        }
    }
}
