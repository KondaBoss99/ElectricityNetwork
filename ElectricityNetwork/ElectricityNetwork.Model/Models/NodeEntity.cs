using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ElectricityNetwork.Model.Models
{
    public class NodeEntity : PowerEntity
    {
        public override void SetColor()
        {
            var color = ((System.Windows.Media.SolidColorBrush)PEShape.Fill).Color;

            if (color == System.Windows.Media.Color.FromRgb(19, 88, 162))
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(236, 167, 93));
            }
            else
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(19, 88, 162));
            }
        }
    }
}
