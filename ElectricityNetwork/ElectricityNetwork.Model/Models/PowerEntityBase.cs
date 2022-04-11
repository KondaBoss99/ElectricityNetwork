using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ElectricityNetwork.Model.Models
{
    public class PowerEntityBase
    {
        public Ellipse PEShape { get; set; }

        public virtual void SetColor() { }

        public void OnClick(object sender, EventArgs e)
        {
            PEShape.Fill = Brushes.Yellow;
        }
    }
}
