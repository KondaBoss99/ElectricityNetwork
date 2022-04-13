using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ElectricityNetwork.Model.Models
{
    public class SubstationEntity : PowerEntity
    {
        public override void SetColor()
        {
            var color = ((System.Windows.Media.SolidColorBrush)PEShape.Fill).Color;

            if (color == System.Windows.Media.Color.FromRgb(227, 50, 130))
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(28, 205, 125));
            }
            else
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(227, 50, 130));
            }
        }
    }
}
