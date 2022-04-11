using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityNetwork.Model.Models
{
    public class ElectricityNetworkContentModel
    {

        #region Constructors

        public ElectricityNetworkContentModel() { }

        #endregion

        #region Properties
        public List<PowerEntity> PowerEntities { get; set; } = new List<PowerEntity>();
        public List<LineEntity> Lines { get; set; } = new List<LineEntity>();
        public List<SubstationEntity> Substations { get; set; } = new List<SubstationEntity>();
        public List<SwitchEntity> Switches { get; set; } = new List<SwitchEntity>();
        public List<NodeEntity> Nodes { get; set; } = new List<NodeEntity>();
        public List<Point> Points { get; set; } = new List<Point>();

        #endregion

        
    }
}
