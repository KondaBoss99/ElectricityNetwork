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
using System.Windows.Shapes;

namespace ElectricityNetwork.WPF.Views.PaternDraw
{
    /// <summary>
    /// Interaction logic for PolygonWindow.xaml
    /// </summary>
    public partial class PolygonWindow : Window
    {
        #region Fields
        private List<Point> polygonPoints;

        #endregion

        #region Properties
        public List<Point> PolygonPoints
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

        #endregion

        #region Constructors
        public PolygonWindow(List<Point> points)
        {
            PolygonPoints = points;
            InitializeComponent();
        }
        public PolygonWindow(List<Point> points, int contureLine, Color fillColor, Color borderColor)
        {
            PolygonPoints = points;
            InitializeComponent();
            textBoxContureLine.Text = contureLine.ToString();
            colorPickerFill.SelectedColor = fillColor;
            colorPickerBorder.SelectedColor = borderColor;
        }

        #endregion

        #region Events
        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int temp;
            if (int.TryParse(textBoxContureLine.Text, out temp) && colorPickerFill.SelectedColor.HasValue
                && colorPickerBorder.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawPolygon(PolygonPoints,
                                                                     int.Parse(textBoxContureLine.Text),
                                                                     colorPickerFill.SelectedColor.Value,
                                                                     colorPickerBorder.SelectedColor.Value);

                this.Close();
            }
        }

        #endregion
    }
}
