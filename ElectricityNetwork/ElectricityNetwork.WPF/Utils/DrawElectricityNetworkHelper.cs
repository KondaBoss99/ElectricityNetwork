using ElectricityNetwork.Model.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ElectricityNetwork.WPF.Utils
{
    public static class DrawElectricityNetworkHelper
    {
        public static void LoadXMLContent(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList lineNodeList = xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList substationNodeList = xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switchNodeList = xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeNodeList = xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            ElectricityNetworkContentModel networkModel = new ElectricityNetworkContentModel()
            {
                Lines = GenerateLines(lineNodeList),
                Substations = GenerateSubstations(substationNodeList),
                Switches = GenerateSwitches(switchNodeList),
                Nodes = GenerateNodes(nodeNodeList)
            };


        }


        #region Generate Elements

        private static List<LineEntity> GenerateLines(XmlNodeList lineNodeList)
        {
            List<LineEntity> linesList = new List<LineEntity>();

            for (int i = 0; i < lineNodeList.Count; i++)
            {
                LineEntity l = new LineEntity()
                {
                    ConductorMaterial = lineNodeList[i].SelectSingleNode("ConductorMaterial").InnerText,
                    FirstEnd = long.Parse(lineNodeList[i].SelectSingleNode("FirstEnd").InnerText),
                    Id = long.Parse(lineNodeList[i].SelectSingleNode("Id").InnerText),
                    IsUnderground = bool.Parse(lineNodeList[i].SelectSingleNode("IsUnderground").InnerText),
                    LineType = lineNodeList[i].SelectSingleNode("LineType").InnerText,
                    Name = lineNodeList[i].SelectSingleNode("Name").InnerText,
                    R = float.Parse(lineNodeList[i].SelectSingleNode("R").InnerText),
                    SecondEnd = long.Parse(lineNodeList[i].SelectSingleNode("SecondEnd").InnerText),
                    ThermalConstantHeat = long.Parse(lineNodeList[i].SelectSingleNode("ThermalConstantHeat").InnerText)
                };

                bool foundOpositeLine = false;

                for (int j = 0; j < linesList.Count; j++)
                    if ((linesList[j].FirstEnd == l.SecondEnd && linesList[j].SecondEnd == l.FirstEnd) || (linesList[j].FirstEnd == l.FirstEnd && linesList[j].SecondEnd == l.SecondEnd))
                        foundOpositeLine = true;

                if (foundOpositeLine == false)
                    linesList.Add(l);
            }
            return linesList;
        }

        private static List<SubstationEntity> GenerateSubstations(XmlNodeList substationNodeList)
        {
            List<SubstationEntity> substationsList = new List<SubstationEntity>();

            for (int i = 0; i < substationNodeList.Count; i++)
            {
                SubstationEntity s = new SubstationEntity()
                {
                    Id = long.Parse(substationNodeList[i].SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture),
                    Name = substationNodeList[i].SelectSingleNode("Name").InnerText,
                    X = double.Parse(substationNodeList[i].SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture),
                    Y = double.Parse(substationNodeList[i].SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture)
                };

                LatitudeLongitudeHelper.ToLatitudeLongidute(s.X, s.Y, 34, out double x, out double y);
                s.X = x;
                s.Y = y;
                substationsList.Add(s);
            }
            return substationsList;
        }

        private static List<SwitchEntity> GenerateSwitches(XmlNodeList switchNodeList)
        {
            List<SwitchEntity> switchesList = new List<SwitchEntity>();

            for (int i = 0; i < switchNodeList.Count; i++)
            {
                SwitchEntity s = new SwitchEntity()
                {
                    Id = long.Parse(switchNodeList[i].SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture),
                    Name = switchNodeList[i].SelectSingleNode("Name").InnerText,
                    Status = switchNodeList[i].SelectSingleNode("Status").InnerText,
                    X = double.Parse(switchNodeList[i].SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture),
                    Y = double.Parse(switchNodeList[i].SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture)
                };

                LatitudeLongitudeHelper.ToLatitudeLongidute(s.X, s.Y, 34, out double x, out double y);
                s.X = x;
                s.Y = y;
                switchesList.Add(s);
            }
            return switchesList;
        }

        private static List<NodeEntity> GenerateNodes(XmlNodeList nodeNodeList)
        {
            List<NodeEntity> nodesList = new List<NodeEntity>();

            for (int i = 0; i < nodeNodeList.Count; i++)
            {
                NodeEntity n = new NodeEntity()
                {
                    Id = long.Parse(nodeNodeList[i].SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture),
                    Name = nodeNodeList[i].SelectSingleNode("Name").InnerText,
                    X = double.Parse(nodeNodeList[i].SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture),
                    Y = double.Parse(nodeNodeList[i].SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture)
                };

                LatitudeLongitudeHelper.ToLatitudeLongidute(n.X, n.Y, 34, out double x, out double y);
                n.X = x;
                n.Y = y;
                nodesList.Add(n);
            }
            return nodesList;
        }

        #endregion
    }
}
