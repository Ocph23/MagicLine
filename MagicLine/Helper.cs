using MagicLine.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLine
{
    public static class Helper
    {

        //public static RadialGradientBrush ColorToGradientBall(this System.Drawing.Color color) {

        //    var hex = System.Drawing.ColorTranslator.ToHtml(color);
        //    return new RadialGradientBrush(new GradientStopCollection() {
        //        new GradientStop(Microsoft.Maui.Graphics.Colors.WhiteSmoke,.5f)     ,
        //        new GradientStop(Microsoft.Maui.Graphics.Color.FromArgb(hex),1.0f)
        //    })
        //    { Center = new Microsoft.Maui.Graphics.Point(1.1, 1.0) };
        //}


        public static RadialGradientBrush ColorToGradientBall(string colorName)
        {
      
            var colorBall = ColorHelper.GetColor(colorName);
            return new RadialGradientBrush(new GradientStopCollection() {
                new GradientStop(Microsoft.Maui.Graphics.Color.FromArgb(colorBall.Light),.1f),
                new GradientStop(Microsoft.Maui.Graphics.Color.FromArgb(colorBall.Dark),0.5f),
                new GradientStop(Microsoft.Maui.Graphics.Color.FromArgb(colorBall.Primary),1.0f)
            })
            { Center = new Microsoft.Maui.Graphics.Point(0.4, 0.3) };
        }

    }
}
