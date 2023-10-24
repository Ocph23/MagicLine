using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MagicLine.Game;

namespace MagicLine
{
    public class ConverterBall : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (BallSize)value;
            if (data != null && data != BallSize.Empty)
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


    public class ConventerBallSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var width = System.Convert.ToDouble(parameter);

            var data = (BallSize)value;
            return data switch
            {
                BallSize.Small => (double)width*0.4,
                BallSize.Big => (double)width*0.9,
                _ => (double)0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //var width = System.Convert.ToDouble(parameter);

            //if ((double)value == 0)
            //    return BallSize.Empty;

            //if (width * 0.9 == (double)value)
            //    return BallSize.Big;

            //return BallSize.Small;

        }
    }

    public class ConventerBallColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null)
                return null;
            return Helper.ColorToGradientBall(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConventerBallVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (BallSize)value;

            if (data.Equals(BallSize.Empty))
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
