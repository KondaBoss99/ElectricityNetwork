using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ElectricityNetwork.Model.Models
{
    public class SwitchEntity : PowerEntity
    {
        private string status;

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }
        public override void SetColor()
        {
            var color = ((System.Windows.Media.SolidColorBrush)PEShape.Fill).Color;

            if (color == System.Windows.Media.Color.FromRgb(0, 196, 226))
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(255, 50, 29));
            }
            else
            {
                PEShape.Fill = new SolidColorBrush(Color.FromRgb(0, 196, 226));
            }
        }
    }
}
