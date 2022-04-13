using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ElectricityNetwork.WPF.Utils
{
    public static class HexColorToBrushHelper
    {
        public static Brush GetBrush(string hexColor)
        {
            BrushConverter converter = new BrushConverter();
            return (Brush)converter.ConvertFromString(hexColor);
        }
    }
}
