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
    public class DrawElectricityNetworkHelper
    {
        public readonly List<PowerEntity> Resources;
        private List<LineEntity> lines = new List<LineEntity>();
        private List<SubstationEntity> substations = new List<SubstationEntity>();
        private List<SwitchEntity> switches = new List<SwitchEntity>();
        private List<NodeEntity> nodes = new List<NodeEntity>();
        private List<Point> utilizedPoints = new List<Point>();
        private GridModel mainGrid;

        private double xParts;
        private double yParts;
        private double minimumX;
        private double minimumY;

        public DrawElectricityNetworkHelper()
        {
            Resources = new List<PowerEntity>();
        }

        public void LoadAndParseXML(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList lineNodeList = xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity");
            XmlNodeList substationNodeList = xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switchNodeList = xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeNodeList = xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            lines = GenerateLines(lineNodeList);
            substations = GenerateSubstations(substationNodeList);
            switches = GenerateSwitches(switchNodeList);
            nodes = GenerateNodes(nodeNodeList);
        }

        private List<LineEntity> GenerateLines(XmlNodeList lineNodeList)
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

        private List<SubstationEntity> GenerateSubstations(XmlNodeList substationNodeList)
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

        private List<SwitchEntity> GenerateSwitches(XmlNodeList switchNodeList)
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

        private List<NodeEntity> GenerateNodes(XmlNodeList nodeNodeList)
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

        public void ScaleCanvas(double width, double height)
        {
            double substationMinX, substationMinY, nodeMinX, nodeMinY, switchMinX, switchMinY, substationNodeMinX, substationNodeMinY;
            substationMinX = substations.Min((obj) => obj.X);
            nodeMinX = nodes.Min((obj) => obj.X);
            switchMinX = switches.Min((obj) => obj.X);
            substationMinY = substations.Min((obj) => obj.Y);
            nodeMinY = nodes.Min((obj) => obj.Y);
            switchMinY = switches.Min((obj) => obj.Y);
            substationNodeMinX = Math.Min(substationMinX, nodeMinX);
            minimumX = Math.Min(substationNodeMinX, switchMinX);
            substationNodeMinY = Math.Min(substationMinY, nodeMinY);
            minimumY = Math.Min(substationNodeMinY, switchMinY);

            double substationMaxX, substationMaxY, nodeMaxX, NodeMaxY, switchMaxX, switchMaxY, substationNodeMaxX, substationNodeMaxY, maxX, maxY;

            substationMaxX = substations.Max((obj) => obj.X);
            nodeMaxX = nodes.Max((obj) => obj.X);
            switchMaxX = switches.Max((obj) => obj.X);
            substationMaxY = substations.Max((obj) => obj.Y);
            NodeMaxY = nodes.Max((obj) => obj.Y);
            switchMaxY = switches.Max((obj) => obj.Y);
            substationNodeMaxX = Math.Min(substationMaxX, nodeMaxX);
            substationNodeMaxY = Math.Min(substationMaxY, NodeMaxY);
            maxX = Math.Max(substationNodeMaxX, switchMaxX);
            maxY = Math.Max(substationNodeMaxY, switchMaxY);
            xParts = (width / 2) / (maxX - minimumX);
            yParts = (height / 2) / (maxY - minimumY);

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            mainGrid = new GridModel(500+1, 500+1);
            for (int i = 0; i <= 500; i++)
                for (int j = 0; j <= 500; j++)
                    mainGrid.BlockMatrix[i, j] = new BlockModel()
                    {
                        X = i * 10,
                        Y = j * 10,
                        BlockType = EBlockType.EMPTY,
                        BlockShape = null,
                        ApproximateX = i,
                        ApproximateY = j,
                    };
        }

        public void ConvertToCanvasCoordinates()
        {
            for (int i = 0; i < substations.Count; i++)
            {
                double x = Math.Round((substations[i].X - minimumX) * xParts / 10) * 10;
                double y = Math.Round((substations[i].Y - minimumY) * yParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(x, y);
                substations[i].X = ret.X;
                substations[i].Y = ret.Y;
                mainGrid.Add(ret.X, ret.Y, EBlockType.SUBSTATION);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                double x = Math.Round((nodes[i].X - minimumX) * xParts / 10) * 10;
                double y = Math.Round((nodes[i].Y - minimumY) * yParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(x, y);
                mainGrid.Add(ret.X, ret.Y, EBlockType.NODE);
                nodes[i].X = ret.X;
                nodes[i].Y = ret.Y;
            }

            for (int i = 0; i < switches.Count; i++)
            {
                double x = Math.Round((switches[i].X - minimumX) * xParts / 10) * 10;
                double y = Math.Round((switches[i].Y - minimumY) * yParts / 10) * 10;
                Point ret = GetClosestAvailablePoint(x, y);
                mainGrid.Add(ret.X, ret.Y, EBlockType.SWITCH);
                switches[i].X = ret.X;
                switches[i].Y = ret.Y;
            }
        }


        public Point GetClosestAvailablePoint(double x, double y)
        {
            if (!IsPointUsed(x, y))
            {
                utilizedPoints.Add(new Point() { X = x, Y = y } );

                return new Point() { X = x * 2, Y = y * 2 };
            }

            double closestX = x - 10;
            closestX = (closestX < 0) ? closestX + 10 : closestX;

            double closestY = y - 10;
            closestY = (closestY < 0) ? closestY + 10 : closestY;

            while (IsPointUsed(closestX, closestY))
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (!IsPointUsed(closestX, closestY))
                        {
                            utilizedPoints.Add(new Point() { X = closestX, Y = closestY } );
                            return new Point() { X = closestX * 2, Y = closestY * 2 };
                        }
                        closestY = closestY + 10;
                    }
                    if (!IsPointUsed(closestX, closestY))
                    {
                        utilizedPoints.Add(new Point() { X = closestX, Y = closestY } );
                        return new Point() { X = closestX * 2, Y = closestY * 2 };
                    }
                    closestX = closestX + 10;
                    closestY = closestY - 2 * 10;
                }
            }

            utilizedPoints.Add(new Point() { X = closestX, Y = closestY });
            return new Point() { X = closestX * 2, Y = closestY * 2 };
        }


        public bool IsPointUsed(double x, double y)
        {
            for (int i = 0; i < utilizedPoints.Count; i++)
                if (utilizedPoints[i].X == x && utilizedPoints[i].Y == y)
                    return true;

            return false;
        }

        public void DrawElements(Canvas mainCanvas)
        {
            DrawLines(mainCanvas);
            DrawSubstations(mainCanvas);
            DrawNodes(mainCanvas);
            DrawSwitches(mainCanvas);
            DrawCrossMarks(mainCanvas);
        }

        public void DrawSubstations(Canvas drawingCanvas)
        {
            for (int i = 0; i < substations.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#E33282") };
                shape.ToolTip = "Substation: \n" + "ID:" + substations[i].Id + "\nName: " + substations[i].Name;
                Canvas.SetLeft(shape, substations[i].X + 2);
                Canvas.SetTop(shape, substations[i].Y + 2);
                substations[i].PEShape = shape;
                Resources.Add(substations[i]);
                drawingCanvas.Children.Add(substations[i].PEShape);
            }
        }

        public void DrawNodes(Canvas drawingCanvas)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#1358A2") };
                shape.ToolTip = "Node: \n" + "ID:" + nodes[i].Id + "\nName: " + nodes[i].Name;
                Canvas.SetLeft(shape, nodes[i].X + 2);
                Canvas.SetTop(shape, nodes[i].Y + 2);
                nodes[i].PEShape = shape;
                Resources.Add(nodes[i]);
                drawingCanvas.Children.Add(nodes[i].PEShape);
            }
        }

        public void DrawSwitches(Canvas drawingCanvas)
        {
            for (int i = 0; i < switches.Count; i++)
            {
                Ellipse shape = new Ellipse() { Height = 6, Width = 6, Stroke = Brushes.Black, Fill = HexColorToBrushHelper.GetBrush("#00C4E2") };
                shape.ToolTip = "Switch: \n" + "ID:" + switches[i].Id + "\nName: " + switches[i].Name + "\nStatus: " + switches[i].Status;
                Canvas.SetLeft(shape, switches[i].X + 2);
                Canvas.SetTop(shape, switches[i].Y + 2);
                switches[i].PEShape = shape;
                Resources.Add(switches[i]);
                drawingCanvas.Children.Add(switches[i].PEShape);
            }
        }

        public void DrawLines(Canvas drawingCanvas)
        {
            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> connectedCoords = GetCoordinates();

            foreach (Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity> ent in connectedCoords)
            {
                PowerEntity startEntity = ent.Item5, endEntity = ent.Item6;
                double x1 = ent.Item1, y1 = ent.Item2, x2 = ent.Item3, y2 = ent.Item4;
                LineEntity conline = ent.Item7;

                if ((x1 == 0 || x2 == 0 || y1 == 0 || y2 == 0) || (x1 == x2 && y1 == y2))
                    continue;

                List<BlockModel> lineBlocks = mainGrid.BFSCreateLine(x1, y1, x2, y2, false);

                if (lineBlocks.Count < 2)
                    lineBlocks = mainGrid.BFSCreateLine(x1, y1, x2, y2, true);

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
                            mainGrid.AddLine(lineBlocks[i].X, lineBlocks[i].Y, lineType);
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


        List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> GetCoordinates()
        {
            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> coordinates = new List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>>();
            foreach (LineEntity ent in lines)
            {
                PowerEntity startEntity = new PowerEntity();
                PowerEntity endEntity = new PowerEntity();

                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                for (int i = 0; i < substations.Count; i++)
                {
                    if (substations[i].Id == ent.FirstEnd)
                    {
                        x1 = substations[i].X;
                        y1 = substations[i].Y;
                        startEntity = substations[i];
                    }
                    if (substations[i].Id == ent.SecondEnd)
                    {
                        x2 = substations[i].X;
                        y2 = substations[i].Y;
                        endEntity = substations[i];
                    }
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Id == ent.FirstEnd)
                    {
                        x1 = nodes[i].X;
                        y1 = nodes[i].Y;
                        startEntity = nodes[i];
                    }
                    if (nodes[i].Id == ent.SecondEnd)
                    {
                        x2 = nodes[i].X;
                        y2 = nodes[i].Y;
                        endEntity = nodes[i];
                    }
                }

                for (int i = 0; i < switches.Count; i++)
                {
                    if (switches[i].Id == ent.FirstEnd)
                    {
                        x1 = switches[i].X;
                        y1 = switches[i].Y;
                        startEntity = switches[i];
                    }
                    if (switches[i].Id == ent.SecondEnd)
                    {
                        x2 = switches[i].X;
                        y2 = switches[i].Y;
                        endEntity = switches[i];
                    }
                }

                coordinates.Add(new Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>(x1, y1, x2, y2, startEntity, endEntity, ent));
            }

            return coordinates.OrderBy(tup => ((tup.Item1 - tup.Item3) * (tup.Item1 - tup.Item3) + (tup.Item2 - tup.Item4) * (tup.Item2 - tup.Item4))).ToList();
        }

        void DrawCrossMarks(Canvas drawingCanvas)
        {
            foreach (BlockModel block in mainGrid.BlockMatrix)
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

        public void SetElementColors(object sender, EventArgs e)
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
    }

}
