using ElectricityNetwork.Model.Models;
using ElectricityNetwork.Model.Models.Enums;
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
        public static ElectricityNetworkContentModel LoadXMLContent(string path, double canvasWidth, double canvasHeight)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList lineNodeList = xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList substationNodeList = xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switchNodeList = xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeNodeList = xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            ElectricityNetworkContentModel networkModel = new ElectricityNetworkContentModel()
            {
                Width = canvasWidth,
                Height = canvasHeight,
                Lines = GenerateLines(lineNodeList),
                Substations = GenerateSubstations(substationNodeList),
                Switches = GenerateSwitches(switchNodeList),
                Nodes = GenerateNodes(nodeNodeList),
            };

            GenerateGrid(networkModel);

            return networkModel;
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
        private static void GenerateGrid(ElectricityNetworkContentModel networkModel)
        {
            double substationMinX, substationMinY, nodeMinX, nodeMinY, switchMinX, switchMinY, substationNodeMinX, substationNodeMinY;
            substationMinX = networkModel.Substations.Min((obj) => obj.X);
            nodeMinX = networkModel.Nodes.Min((obj) => obj.X);
            switchMinX = networkModel.Switches.Min((obj) => obj.X);
            substationMinY = networkModel.Substations.Min((obj) => obj.Y);
            nodeMinY = networkModel.Nodes.Min((obj) => obj.Y);
            switchMinY = networkModel.Switches.Min((obj) => obj.Y);
            substationNodeMinX = Math.Min(substationMinX, nodeMinX);
            networkModel.MinimumX = Math.Min(substationNodeMinX, switchMinX);
            substationNodeMinY = Math.Min(substationMinY, nodeMinY);
            networkModel.MinimumY = Math.Min(substationNodeMinY, switchMinY);

            double substationMaxX, substationMaxY, nodeMaxX, NodeMaxY, switchMaxX, switchMaxY, substationNodeMaxX, substationNodeMaxY, maxX, maxY;

            substationMaxX = networkModel.Substations.Max((obj) => obj.X);
            nodeMaxX = networkModel.Nodes.Max((obj) => obj.X);
            switchMaxX = networkModel.Switches.Max((obj) => obj.X);
            substationMaxY = networkModel.Substations.Max((obj) => obj.Y);
            NodeMaxY = networkModel.Nodes.Max((obj) => obj.Y);
            switchMaxY = networkModel.Switches.Max((obj) => obj.Y);
            substationNodeMaxX = Math.Min(substationMaxX, nodeMaxX);
            substationNodeMaxY = Math.Min(substationMaxY, NodeMaxY);
            maxX = Math.Max(substationNodeMaxX, switchMaxX);
            maxY = Math.Max(substationNodeMaxY, switchMaxY);
            networkModel.XParts = (networkModel.Width / 2) / (maxX - networkModel.MinimumX);
            networkModel.YParts = (networkModel.Height / 2) / (maxY - networkModel.MinimumY);

            networkModel.MainGrid = new GridModel(500 + 1, 500 + 1);
            for (int i = 0; i <= 500; i++)
                for (int j = 0; j <= 500; j++)
                    networkModel.MainGrid.BlockMatrix[i, j] = new BlockModel()
                    {
                        X = i * 10,
                        Y = j * 10,
                        BlockType = EBlockType.EMPTY,
                        BlockShape = null,
                        ApproximateX = i,
                        ApproximateY = j,
                    };

            ConvertToCanvasCoordinates(networkModel);
        }
        #endregion

        #region Methods

        private static void ConvertToCanvasCoordinates(ElectricityNetworkContentModel networkModel)
        {
            for (int i = 0; i < networkModel.Substations.Count; i++)
            {
                double x = Math.Round((networkModel.Substations[i].X - networkModel.MinimumX) * networkModel.XParts / 10) * 10;
                double y = Math.Round((networkModel.Substations[i].Y - networkModel.MinimumY) * networkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(networkModel, x, y);
                networkModel.Substations[i].X = ret.X;
                networkModel.Substations[i].Y = ret.Y;
                networkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.SUBSTATION);
            }

            for (int i = 0; i < networkModel.Nodes.Count; i++)
            {
                double x = Math.Round((networkModel.Nodes[i].X - networkModel.MinimumX) * networkModel.XParts / 10) * 10;
                double y = Math.Round((networkModel.Nodes[i].Y - networkModel.MinimumY) * networkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(networkModel, x, y);
                networkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.NODE);
                networkModel.Nodes[i].X = ret.X;
                networkModel.Nodes[i].Y = ret.Y;
            }

            for (int i = 0; i < networkModel.Switches.Count; i++)
            {
                double x = Math.Round((networkModel.Switches[i].X - networkModel.MinimumX) * networkModel.XParts / 10) * 10;
                double y = Math.Round((networkModel.Switches[i].Y - networkModel.MinimumY) * networkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(networkModel, x, y);
                networkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.SWITCH);
                networkModel.Switches[i].X = ret.X;
                networkModel.Switches[i].Y = ret.Y;
            }
        }

        private static Point GetClosestAvailablePoint(ElectricityNetworkContentModel networkModel, double x, double y)
        {
            if (!IsPointUsed(networkModel, x, y))
            {
                networkModel.Points.Add(new Point() { X = x, Y = y });

                return new Point() { X = x * 2, Y = y * 2 };
            }

            double closestX = x - 10;
            closestX = (closestX < 0) ? closestX + 10 : closestX;

            double closestY = y - 10;
            closestY = (closestY < 0) ? closestY + 10 : closestY;

            while (IsPointUsed(networkModel, closestX, closestY))
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (!IsPointUsed(networkModel, closestX, closestY))
                        {
                            networkModel.Points.Add(new Point() { X = closestX, Y = closestY });
                            return new Point() { X = closestX * 2, Y = closestY * 2 };
                        }
                        closestY = closestY + 10;
                    }
                    if (!IsPointUsed(networkModel, closestX, closestY))
                    {
                        networkModel.Points.Add(new Point() { X = closestX, Y = closestY });
                        return new Point() { X = closestX * 2, Y = closestY * 2 };
                    }
                    closestX = closestX + 10;
                    closestY = closestY - 2 * 10;
                }
            }

            networkModel.Points.Add(new Point() { X = closestX, Y = closestY });
            return new Point() { X = closestX * 2, Y = closestY * 2 };
        }


        private static bool IsPointUsed(ElectricityNetworkContentModel networkModel, double x, double y)
        {
            for (int i = 0; i < networkModel.Points.Count; i++)
                if (networkModel.Points[i].X == x && networkModel.Points[i].Y == y)
                    return true;

            return false;
        }

        #endregion
    }
}
