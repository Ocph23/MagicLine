using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagicLine
{
    internal class ConventerBall : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (CellSize)value;
            if (data != null && data != CellSize.Empty)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    internal class ConventerBallSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = (StackLayout)parameter;

            var data = (CellSize)value;
            return data switch
            {
                CellSize.Small => 15,
                CellSize.Big => 1,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ConventerBallColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sysColor = (System.Drawing.Color)value;
            var hex = System.Drawing.ColorTranslator.ToHtml(sysColor);
            return new RadialGradientBrush(new GradientStopCollection() {
                new GradientStop(Microsoft.Maui.Graphics.Colors.WhiteSmoke,.5f)     ,
                new GradientStop(Microsoft.Maui.Graphics.Color.FromArgb(hex),1.0f)
            })
            {Center = new Microsoft.Maui.Graphics.Point(1.1,1.0) };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
