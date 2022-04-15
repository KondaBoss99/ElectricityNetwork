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
    /// Interaction logic for EllipseWindow.xaml
    /// </summary>
    public partial class EllipseWindow : Window
    {
        #region Constructors

        public EllipseWindow()
        {
            InitializeComponent();
        }
        public EllipseWindow(int radiusX, int radiusY, int contureLine, Color fillColor, Color borderColor)
        {
            InitializeComponent();
            textBoxRadiusX.Text = radiusX.ToString();
            textBoxRadiusY.Text = radiusY.ToString();
            textBoxContureLine.Text = contureLine.ToString();
            colorPickerFill.SelectedColor = fillColor;
            colorPickerBorder.SelectedColor = borderColor;
        }

        #endregion

        #region Events

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int temp;

            if (int.TryParse(textBoxRadiusX.Text, out temp)
                && int.TryParse(textBoxRadiusY.Text, out temp)
                && int.TryParse(textBoxContureLine.Text, out temp)
                && colorPickerFill.SelectedColor.HasValue
                && colorPickerBorder.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawEllipse(int.Parse(textBoxRadiusX.Text),
                                                                     int.Parse(textBoxRadiusY.Text),
                                                                     int.Parse(textBoxContureLine.Text),
                                                                     colorPickerFill.SelectedColor.Value,
                                                                     colorPickerBorder.SelectedColor.Value);
                this.Close();
            }
        }

        #endregion
    }
}
