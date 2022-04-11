using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityNetwork.Model.Models.Enums
{
    public enum EBlockType
    {
        EMPTY,
        NODE,
        SWITCH,
        SUBSTATION,
        LINE,
        HLINE,
        VLINE,
        CROSS_LINE
    }
}
