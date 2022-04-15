using ElectricityNetwork.Model.Models;
using ElectricityNetwork.Model.Models.Enums;
using ElectricityNetwork.WPF.Utils;
using ElectricityNetwork.WPF.Views.PaternDraw;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectricityNetwork.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private ElectricityNetworkModel electricityNetworkModel = new ElectricityNetworkModel();

        private EPattern selectedElement;
        private System.Windows.Point currentPoint;
        private List<System.Windows.Point> polygonPoints;
        public UIElement elementToDelete;
        private List<UIElement> undoElements;
        private List<UIElement> redoElements;

        #endregion

        #region Properties
        public EPattern SelectedElement
        {
            get
            {
                return selectedElement;
            }
            set
            {
                switch (selectedElement)
                {
                    case EPattern.Ellipse:
                        menuItemEllipse.Background = Brushes.Transparent;
                        break;
                    case EPattern.Polygon:
                        menuItemPolygon.Background = Brushes.Transparent;
                        break;
                    default:
                        break;
                }

                selectedElement = value;

                switch (selectedElement)
                {
                    case EPattern.Ellipse:
                        menuItemEllipse.Background = Brushes.DarkGray;
                        break;
                    case EPattern.Polygon:
                        menuItemPolygon.Background = Brushes.DarkGray;
                        break;
                }
            }
        }
        public System.Windows.Point CurrentPoint
        {
            get
            {
                return currentPoint;
            }
            set
            {
                currentPoint = value;
            }
        }
        public List<System.Windows.Point> PolygonPoints
        {
            get
            {
                return polygonPoints;
            }
            set
            {
                polygonPoints = value;
            }
        }
        public UIElement ElementToDelete
        {
            get
            {
                return elementToDelete;
            }
            set
            {
                elementToDelete = value;
            }
        }
        public List<UIElement> UndoElements
        {
            get
            {
                return undoElements;
            }
            set
            {
                undoElements = value;
            }
        }
        public List<UIElement> RedoElements
        {
            get
            {
                return redoElements;
            }
            set
            {
                redoElements = value;
            }
        }

        #endregion

        #region Constructors
        public MainWindow()
        {
            InitDrawingLists();
            InitializeComponent();
        }

        #endregion

        #region Initialize Lists
        private void InitDrawingLists()
        {
            PolygonPoints = new List<System.Windows.Point>();
            UndoElements = new List<UIElement>();
            RedoElements = new List<UIElement>();
        }

        #endregion

        #region Methods
        public void DrawEllipse(int radiusX, int radiusY, int contureLine, Color fillColor, Color borderColor)
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(borderColor),
                StrokeThickness = contureLine
            };

            Canvas.SetTop(ellipse, CurrentPoint.Y);
            Canvas.SetLeft(ellipse, CurrentPoint.X);

            ellipse.Width = radiusX;
            ellipse.Height = radiusY;

            DrawingNetworkCanvas.Children.Add(ellipse);

            ellipse.MouseLeftButtonDown += new MouseButtonEventHandler(EditEllipse);
        }
        public void DrawPolygon(List<System.Windows.Point> points, int contureLine, Color fillColor, Color borderColor)
        {
            Polygon polygon = new Polygon
            {
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(borderColor),
                StrokeThickness = contureLine
            };

            foreach (System.Windows.Point p in points)
                polygon.Points.Add(p);

            DrawingNetworkCanvas.Children.Add(polygon);

            PolygonPoints.Clear();
            polygon.MouseLeftButtonDown += new MouseButtonEventHandler(EditPolygon);
        }
        private void EditEllipse(object sender, MouseButtonEventArgs e)
        {
            ElementToDelete = sender as UIElement;
            Ellipse el = (Ellipse)ElementToDelete;

            currentPoint.Y = Canvas.GetTop(ElementToDelete);
            currentPoint.X = Canvas.GetLeft(ElementToDelete);

            EllipseWindow ellipse = new EllipseWindow((int)el.Width, (int)el.Height, (int)el.StrokeThickness, ((SolidColorBrush)el.Fill).Color, ((SolidColorBrush)el.Stroke).Color);
            ellipse.Show();

            DrawingNetworkCanvas.Children.Remove(ElementToDelete);
            ElementToDelete = null;
        }
        private void EditPolygon(object sender, MouseButtonEventArgs e)
        {
            ElementToDelete = sender as UIElement;

            Polygon p = (Polygon)ElementToDelete;

            currentPoint.Y = Canvas.GetTop(ElementToDelete);
            currentPoint.X = Canvas.GetLeft(ElementToDelete);

            PolygonWindow polygon = new PolygonWindow(PolygonPoints, (int)p.StrokeThickness, ((SolidColorBrush)p.Fill).Color, ((SolidColorBrush)p.Stroke).Color);
            polygon.Show();

            DrawingNetworkCanvas.Children.Remove(ElementToDelete);
            ElementToDelete = null;
        }

        #endregion

        #region Events

        private void LoadXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    DefaultExt = "xml",
                    Filter = "XML Files|*.xml"
                };

                if (openFileDialog.ShowDialog().GetValueOrDefault())
                {
                    this.DrawingNetworkCanvas.Children.Clear();

                    this.electricityNetworkModel = DrawElectricityNetworkHelper.LoadAndParseXML(openFileDialog.FileName, this.DrawingNetworkCanvas.Width, this.DrawingNetworkCanvas.Height);

                    LoadedXMLFileName.Text = openFileDialog.SafeFileName;
                    DrawElementsOnCanvasBtn.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DrawElementsOnCanvasBtn_Click(object sender, RoutedEventArgs e)
        {
            DrawElectricityNetworkHelper.DrawElectricityNetworkElements(this.electricityNetworkModel, this.DrawingNetworkCanvas);
        }
        
        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            SelectedElement = EPattern.Ellipse;
        }
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            SelectedElement = EPattern.Polygon;
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (DrawingNetworkCanvas.Children.Count > 1)
            {
                RedoElements.Add((DrawingNetworkCanvas.Children[DrawingNetworkCanvas.Children.Count - 1]));
                DrawingNetworkCanvas.Children.RemoveAt(DrawingNetworkCanvas.Children.Count - 1);
            }
            else if (UndoElements.Count() > 1)
            {
                foreach (UIElement item in UndoElements)
                    DrawingNetworkCanvas.Children.Add(item);

                UndoElements.Clear();
            }
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (RedoElements.Count > 0)
            {
                DrawingNetworkCanvas.Children.Add(RedoElements[RedoElements.Count() - 1]);
                RedoElements.RemoveAt(RedoElements.Count() - 1);
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (DrawingNetworkCanvas.Children.Count > 0)
            {
                foreach (UIElement item in DrawingNetworkCanvas.Children)
                    UndoElements.Add(item);

                DrawingNetworkCanvas.Children.Clear();
            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedElement == EPattern.Polygon && PolygonPoints.Count >= 3)
            {
                PolygonWindow polygonWindow = new PolygonWindow(PolygonPoints);
                polygonWindow.Show();
            }
        }
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentPoint = e.GetPosition(DrawingNetworkCanvas);

            switch (selectedElement)
            {
                case EPattern.Ellipse:
                    EllipseWindow ellipseWindow = new EllipseWindow();
                    ellipseWindow.Show();
                    break;
                case EPattern.Polygon:
                    PolygonPoints.Add(CurrentPoint);
                    break;
            }
        }

        #endregion
    }
}