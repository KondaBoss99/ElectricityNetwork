using ElectricityNetwork.Model.Models;
using ElectricityNetwork.Model.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace ElectricityNetwork.WPF.Utils
{
    public static class DrawElectricityNetworkHelper
    {
        #region Load XML
        public static ElectricityNetworkModel LoadAndParseXML(string path, double width, double height)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList lineNodeList = xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList substationNodeList = xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switchNodeList = xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeNodeList = xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            ElectricityNetworkModel electricityNetworkModel = new ElectricityNetworkModel
            {
                Lines = GenerateElectricityNetworkLines(lineNodeList),
                Substations = GenerateElectricityNetworkSubstations(substationNodeList),
                Switches = GenerateElectricityNetworkSwitches(switchNodeList),
                Nodes = GenerateElectricityNetworkNodes(nodeNodeList),
            };

            InitializeGrid(electricityNetworkModel, width, height);

            return electricityNetworkModel;
        }

        #endregion

        #region Generate Lists
        private static List<LineEntity> GenerateElectricityNetworkLines(XmlNodeList lineNodeList)
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
        private static List<SubstationEntity> GenerateElectricityNetworkSubstations(XmlNodeList substationNodeList)
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
        private static List<SwitchEntity> GenerateElectricityNetworkSwitches(XmlNodeList switchNodeList)
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
        private static List<NodeEntity> GenerateElectricityNetworkNodes(XmlNodeList nodeNodeList)
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

        #region Initializer Methods
        private static void InitializeGrid(ElectricityNetworkModel electricityNetworkModel, double width, double height)
        {
            ScaleCanvas(electricityNetworkModel, width, height);
            GenerateGrid(electricityNetworkModel);
            ConvertToCanvasCoordinates(electricityNetworkModel);
        }
        private static void ScaleCanvas(ElectricityNetworkModel electricityNetworkModel, double width, double height)
        {
            double substationMinX, substationMinY, nodeMinX, nodeMinY, switchMinX, switchMinY, substationNodeMinX, substationNodeMinY;
            substationMinX = electricityNetworkModel.Substations.Min((obj) => obj.X);
            nodeMinX = electricityNetworkModel.Nodes.Min((obj) => obj.X);
            switchMinX = electricityNetworkModel.Switches.Min((obj) => obj.X);
            substationMinY = electricityNetworkModel.Substations.Min((obj) => obj.Y);
            nodeMinY = electricityNetworkModel.Nodes.Min((obj) => obj.Y);
            switchMinY = electricityNetworkModel.Switches.Min((obj) => obj.Y);
            substationNodeMinX = Math.Min(substationMinX, nodeMinX);
            electricityNetworkModel.MinimumX = Math.Min(substationNodeMinX, switchMinX);
            substationNodeMinY = Math.Min(substationMinY, nodeMinY);
            electricityNetworkModel.MinimumY = Math.Min(substationNodeMinY, switchMinY);

            double substationMaxX, substationMaxY, nodeMaxX, NodeMaxY, switchMaxX, switchMaxY, substationNodeMaxX, substationNodeMaxY, maxX, maxY;

            substationMaxX = electricityNetworkModel.Substations.Max((obj) => obj.X);
            nodeMaxX = electricityNetworkModel.Nodes.Max((obj) => obj.X);
            switchMaxX = electricityNetworkModel.Switches.Max((obj) => obj.X);
            substationMaxY = electricityNetworkModel.Substations.Max((obj) => obj.Y);
            NodeMaxY = electricityNetworkModel.Nodes.Max((obj) => obj.Y);
            switchMaxY = electricityNetworkModel.Switches.Max((obj) => obj.Y);
            substationNodeMaxX = Math.Min(substationMaxX, nodeMaxX);
            substationNodeMaxY = Math.Min(substationMaxY, NodeMaxY);
            maxX = Math.Max(substationNodeMaxX, switchMaxX);
            maxY = Math.Max(substationNodeMaxY, switchMaxY);
            electricityNetworkModel.XParts = (width / 2) / (maxX - electricityNetworkModel.MinimumX);
            electricityNetworkModel.YParts = (height / 2) / (maxY - electricityNetworkModel.MinimumY);
        }
        private static void GenerateGrid(ElectricityNetworkModel electricityNetworkModel)
        {
            electricityNetworkModel.MainGrid = new GridModel(500 + 1, 500 + 1);
            for (int i = 0; i <= 500; i++)
                for (int j = 0; j <= 500; j++)
                    electricityNetworkModel.MainGrid.BlockMatrix[i, j] = new BlockModel()
                    {
                        X = i * 10,
                        Y = j * 10,
                        BlockType = EBlockType.EMPTY,
                        BlockShape = null,
                        ApproximateX = i,
                        ApproximateY = j,
                    };
        }
        private static void ConvertToCanvasCoordinates(ElectricityNetworkModel electricityNetworkModel)
        {
            for (int i = 0; i < electricityNetworkModel.Substations.Count; i++)
            {
                double x = Math.Round((electricityNetworkModel.Substations[i].X - electricityNetworkModel.MinimumX) * electricityNetworkModel.XParts / 10) * 10;
                double y = Math.Round((electricityNetworkModel.Substations[i].Y - electricityNetworkModel.MinimumY) * electricityNetworkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(electricityNetworkModel, x, y);
                electricityNetworkModel.Substations[i].X = ret.X;
                electricityNetworkModel.Substations[i].Y = ret.Y;
                electricityNetworkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.SUBSTATION);
            }

            for (int i = 0; i < electricityNetworkModel.Nodes.Count; i++)
            {
                double x = Math.Round((electricityNetworkModel.Nodes[i].X - electricityNetworkModel.MinimumX) * electricityNetworkModel.XParts / 10) * 10;
                double y = Math.Round((electricityNetworkModel.Nodes[i].Y - electricityNetworkModel.MinimumY) * electricityNetworkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(electricityNetworkModel, x, y);
                electricityNetworkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.NODE);
                electricityNetworkModel.Nodes[i].X = ret.X;
                electricityNetworkModel.Nodes[i].Y = ret.Y;
            }

            for (int i = 0; i < electricityNetworkModel.Switches.Count; i++)
            {
                double x = Math.Round((electricityNetworkModel.Switches[i].X - electricityNetworkModel.MinimumX) * electricityNetworkModel.XParts / 10) * 10;
                double y = Math.Round((electricityNetworkModel.Switches[i].Y - electricityNetworkModel.MinimumY) * electricityNetworkModel.YParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(electricityNetworkModel, x, y);
                electricityNetworkModel.MainGrid.Add(ret.X, ret.Y, EBlockType.SWITCH);
                electricityNetworkModel.Switches[i].X = ret.X;
                electricityNetworkModel.Switches[i].Y = ret.Y;
            }
        }

        #endregion
        
        #region Drawing Methods
        public static void DrawElectricityNetworkElements(ElectricityNetworkModel electricityNetworkModel, Canvas mainCanvas)
        {
            DrawElectricityNetworkLines(electricityNetworkModel, mainCanvas);
            DrawElectricityNetworkSubstations(electricityNetworkModel, mainCanvas);
            DrawElectricityNetworkNodes(electricityNetworkModel, mainCanvas);
            DrawElectricityNetworkSwitches(electricityNetworkModel, mainCanvas);
            DrawElectricityNetworkLineIntersections(electricityNetworkModel, mainCanvas);
        }
        private static void DrawElectricityNetworkSubstations(ElectricityNetworkModel electricityNetworkModel, Canvas drawingCanvas)
        {
            for (int i = 0; i < electricityNetworkModel.Substations.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#E33282") };
                shape.ToolTip = "Substation: \n" + "ID:" + electricityNetworkModel.Substations[i].Id + "\nName: " + electricityNetworkModel.Substations[i].Name;
                Canvas.SetLeft(shape, electricityNetworkModel.Substations[i].X + 2);
                Canvas.SetTop(shape, electricityNetworkModel.Substations[i].Y + 2);
                electricityNetworkModel.Substations[i].PEShape = shape;
                electricityNetworkModel.PowerEntities.Add(electricityNetworkModel.Substations[i]);
                drawingCanvas.Children.Add(electricityNetworkModel.Substations[i].PEShape);
            }
        }
        private static void DrawElectricityNetworkNodes(ElectricityNetworkModel electricityNetworkModel, Canvas drawingCanvas)
        {
            for (int i = 0; i < electricityNetworkModel.Nodes.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#1358A2") };
                shape.ToolTip = "Node: \n" + "ID:" + electricityNetworkModel.Nodes[i].Id + "\nName: " + electricityNetworkModel.Nodes[i].Name;
                Canvas.SetLeft(shape, electricityNetworkModel.Nodes[i].X + 2);
                Canvas.SetTop(shape, electricityNetworkModel.Nodes[i].Y + 2);
                electricityNetworkModel.Nodes[i].PEShape = shape;
                electricityNetworkModel.PowerEntities.Add(electricityNetworkModel.Nodes[i]);
                drawingCanvas.Children.Add(electricityNetworkModel.Nodes[i].PEShape);
            }
        }
        private static void DrawElectricityNetworkSwitches(ElectricityNetworkModel electricityNetworkModel, Canvas drawingCanvas)
        {
            for (int i = 0; i < electricityNetworkModel.Switches.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#00C4E2") };
                shape.ToolTip = "Switch: \n" + "ID:" + electricityNetworkModel.Switches[i].Id + "\nName: " + electricityNetworkModel.Switches[i].Name + "\nStatus: " + electricityNetworkModel.Switches[i].Status;
                Canvas.SetLeft(shape, electricityNetworkModel.Switches[i].X + 2);
                Canvas.SetTop(shape, electricityNetworkModel.Switches[i].Y + 2);
                electricityNetworkModel.Switches[i].PEShape = shape;
                electricityNetworkModel.PowerEntities.Add(electricityNetworkModel.Switches[i]);
                drawingCanvas.Children.Add(electricityNetworkModel.Switches[i].PEShape);
            }
        }
        private static void DrawElectricityNetworkLines(ElectricityNetworkModel electricityNetworkModel, Canvas drawingCanvas)
        {
            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> connectedCoords = GetCoordinates(electricityNetworkModel);

            foreach (Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity> ent in connectedCoords)
            {
                PowerEntity startEntity = ent.Item5, endEntity = ent.Item6;
                double x1 = ent.Item1, y1 = ent.Item2, x2 = ent.Item3, y2 = ent.Item4;
                LineEntity conline = ent.Item7;

                if ((x1 == 0 || x2 == 0 || y1 == 0 || y2 == 0) || (x1 == x2 && y1 == y2))
                    continue;

                List<BlockModel> lineBlocks = electricityNetworkModel.MainGrid.BFSCreateLine(x1, y1, x2, y2, false);

                if (lineBlocks.Count < 2)
                    lineBlocks = electricityNetworkModel.MainGrid.BFSCreateLine(x1, y1, x2, y2, true);

                Polyline angleLine = new Polyline();
                angleLine.Stroke = new SolidColorBrush(Colors.Black);
                angleLine.StrokeThickness = 1.5;

                for (int i = 0; i < lineBlocks.Count; i++)
                {
                    EBlockType lineType = EBlockType.EMPTY;

                    if (i < lineBlocks.Count - 1)
                    {
                        if (lineBlocks[i].X != lineBlocks[i + 1].X)
                            lineType = EBlockType.HLINE;
                        else if (lineBlocks[i].Y != lineBlocks[i + 1].Y)
                            lineType = EBlockType.VLINE;

                        if (lineType != EBlockType.EMPTY)
                            electricityNetworkModel.MainGrid.AddLine(lineBlocks[i].X, lineBlocks[i].Y, lineType);
                    }
                    System.Windows.Point linePoint = new System.Windows.Point(lineBlocks[i].X + 5, lineBlocks[i].Y + 5);
                    angleLine.Points.Add(linePoint);
                }

                angleLine.MouseRightButtonDown += SetElementColors;
                angleLine.MouseRightButtonDown += endEntity.OnClick;
                angleLine.MouseRightButtonDown += startEntity.OnClick;

                angleLine.ToolTip = "Power line\n" + "ID: " + conline.Id + "\nName: " + conline.Name + "\nTyle: " + conline.LineType + "\nConductor material: " + conline.ConductorMaterial + "\nUnderground: " + conline.IsUnderground.ToString();
                drawingCanvas.Children.Add(angleLine);
            }
        }
        private static void DrawElectricityNetworkLineIntersections(ElectricityNetworkModel electricityNetworkModel, Canvas drawingCanvas)
        {
            foreach (BlockModel block in electricityNetworkModel.MainGrid.BlockMatrix)
            {
                if (block.BlockType == EBlockType.CROSS_LINE)
                {
                    Rectangle tempRectangle = new Rectangle { Width = 5, Height = 5, Fill = HexColorToBrushHelper.GetBrush("#686868") };
                    Ellipse tempEllipse = new Ellipse() { Width = 5, Height = 5, Fill = Brushes.Black };
                    Canvas.SetLeft(tempRectangle, block.X + 2.5);
                    Canvas.SetTop(tempRectangle, block.Y + 2.5);
                    drawingCanvas.Children.Add(tempRectangle);
                }
            }
        }

        #endregion

        #region Helper Methods
        private static Point GetClosestAvailablePoint(ElectricityNetworkModel electricityNetworkModel, double x, double y)
        {
            if (!IsPointUsed(electricityNetworkModel, x, y))
            {
                electricityNetworkModel.UtilizedPoints.Add(new Point() { X = x, Y = y });

                return new Point() { X = x * 2, Y = y * 2 };
            }

            double closestX = x - 10;
            closestX = (closestX < 0) ? closestX + 10 : closestX;

            double closestY = y - 10;
            closestY = (closestY < 0) ? closestY + 10 : closestY;

            while (IsPointUsed(electricityNetworkModel, closestX, closestY))
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (!IsPointUsed(electricityNetworkModel, closestX, closestY))
                        {
                            electricityNetworkModel.UtilizedPoints.Add(new Point() { X = closestX, Y = closestY });
                            return new Point() { X = closestX * 2, Y = closestY * 2 };
                        }
                        closestY = closestY + 10;
                    }
                    if (!IsPointUsed(electricityNetworkModel, closestX, closestY))
                    {
                        electricityNetworkModel.UtilizedPoints.Add(new Point() { X = closestX, Y = closestY });
                        return new Point() { X = closestX * 2, Y = closestY * 2 };
                    }
                    closestX = closestX + 10;
                    closestY = closestY - 2 * 10;
                }
            }

            electricityNetworkModel.UtilizedPoints.Add(new Point() { X = closestX, Y = closestY });
            return new Point() { X = closestX * 2, Y = closestY * 2 };
        }
        private static bool IsPointUsed(ElectricityNetworkModel electricityNetworkModel, double x, double y)
        {
            for (int i = 0; i < electricityNetworkModel.UtilizedPoints.Count; i++)
                if (electricityNetworkModel.UtilizedPoints[i].X == x && electricityNetworkModel.UtilizedPoints[i].Y == y)
                    return true;

            return false;
        }
        private static List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> GetCoordinates(ElectricityNetworkModel electricityNetworkModel)
        {
            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> coordinates = new List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>>();
            foreach (LineEntity ent in electricityNetworkModel.Lines)
            {
                PowerEntity startEntity = new PowerEntity();
                PowerEntity endEntity = new PowerEntity();

                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                for (int i = 0; i < electricityNetworkModel.Substations.Count; i++)
                {
                    if (electricityNetworkModel.Substations[i].Id == ent.FirstEnd)
                    {
                        x1 = electricityNetworkModel.Substations[i].X;
                        y1 = electricityNetworkModel.Substations[i].Y;
                        startEntity = electricityNetworkModel.Substations[i];
                    }
                    if (electricityNetworkModel.Substations[i].Id == ent.SecondEnd)
                    {
                        x2 = electricityNetworkModel.Substations[i].X;
                        y2 = electricityNetworkModel.Substations[i].Y;
                        endEntity = electricityNetworkModel.Substations[i];
                    }
                }

                for (int i = 0; i < electricityNetworkModel.Nodes.Count; i++)
                {
                    if (electricityNetworkModel.Nodes[i].Id == ent.FirstEnd)
                    {
                        x1 = electricityNetworkModel.Nodes[i].X;
                        y1 = electricityNetworkModel.Nodes[i].Y;
                        startEntity = electricityNetworkModel.Nodes[i];
                    }
                    if (electricityNetworkModel.Nodes[i].Id == ent.SecondEnd)
                    {
                        x2 = electricityNetworkModel.Nodes[i].X;
                        y2 = electricityNetworkModel.Nodes[i].Y;
                        endEntity = electricityNetworkModel.Nodes[i];
                    }
                }

                for (int i = 0; i < electricityNetworkModel.Switches.Count; i++)
                {
                    if (electricityNetworkModel.Switches[i].Id == ent.FirstEnd)
                    {
                        x1 = electricityNetworkModel.Switches[i].X;
                        y1 = electricityNetworkModel.Switches[i].Y;
                        startEntity = electricityNetworkModel.Switches[i];
                    }
                    if (electricityNetworkModel.Switches[i].Id == ent.SecondEnd)
                    {
                        x2 = electricityNetworkModel.Switches[i].X;
                        y2 = electricityNetworkModel.Switches[i].Y;
                        endEntity = electricityNetworkModel.Switches[i];
                    }
                }

                coordinates.Add(new Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>(x1, y1, x2, y2, startEntity, endEntity, ent));
            }

            return coordinates.OrderBy(tup => ((tup.Item1 - tup.Item3) * (tup.Item1 - tup.Item3) + (tup.Item2 - tup.Item4) * (tup.Item2 - tup.Item4))).ToList();
        }

        #endregion

        #region Event Methods
        public static void SetElementColors(object sender, EventArgs e)
        {
            Polyline angleLine = (Polyline)sender;

            if (angleLine != null)
            {
                var color = ((System.Windows.Media.SolidColorBrush)angleLine.Stroke).Color;
                if (color == System.Windows.Media.Color.FromArgb(255, 0, 0, 0))
                {
                    angleLine.Stroke = new SolidColorBrush(Color.FromRgb(0, 155, 0));
                }
                else
                {
                    angleLine.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
        }

        #endregion
    }
}
