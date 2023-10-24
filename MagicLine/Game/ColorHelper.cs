using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MagicLine.Game
{
    public class ColorHelper
    {
        static  List<BallColor> ballColors = new List<BallColor>()
        {
            new BallColor{Name="Red", Primary="#FF2424", Dark="#FF8585", Light="#FFC2C2" },
            new BallColor{Name="Maroon", Primary="#64001E", Dark="#CC003D", Light="#FF3370" },  //maroon
            new BallColor{Name="Blue", Primary="#235789", Dark="#4D91D1", Light="#9EC3E6" },   //blue
            new BallColor{Name="Green", Primary="#218F00", Dark="#53FF1F", Light="#A1FF85" },   //green
            new BallColor{Name="BlueAqua", Primary="#00B8B8", Dark="#1FFFFF", Light="#85FFFF" },
            new BallColor{Name="Yellow", Primary="#A0A300", Dark="#FBFF0A", Light="#FDFF99" },
            new BallColor{Name="Purple", Primary="#9102DE", Dark="#BE49FD", Light="#E2AEFE" },     //purple
            //new BallColor{Name="Golden", Primary="#A95C0F", Dark="#ED8F31", Light="#F5C28E" },  //golden
            //new BallColor{Name="Pink", Primary="#F415AD", Dark="#F86CCC", Light="#FA9EDD" },  //pink
        };


        public static string[] GetColorNames()
        {
            return ballColors.Select(x => x.Name).ToArray();
        }

        public static BallColor GetColor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
              return  new BallColor { Name = "Empty", Primary = "#43ff6442", Dark = "#43ff6442", Light = "#43ff6442" };  
            }
            return ballColors.Where(x=>x.Name==name).FirstOrDefault();
        }


    }
}
