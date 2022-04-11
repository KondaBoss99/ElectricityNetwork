using ElectricityNetwork.Model.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ElectricityNetwork.Model.Models
{
    public class BlockModel
    {
        public PowerEntity BObject { get; set; }
        public EBlockType BlockType { get; set; } = EBlockType.EMPTY;
        public Brush BrushColor { get; set; } = Brushes.Green;
        public Shape BlockShape { get; set; }
        public double X { get; set; } = -1;
        public double Y { get; set; } = -1;
        public int ApproximateX { get; set; } = -1;
        public int ApproximateY { get; set; } = -1;
    }
}
