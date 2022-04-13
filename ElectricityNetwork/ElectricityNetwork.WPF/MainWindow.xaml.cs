using ElectricityNetwork.Model.Models;
using ElectricityNetwork.WPF.Utils;
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

namespace ElectricityNetwork.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DrawElectricityNetworkHelper mainWindowInputOutput = new DrawElectricityNetworkHelper();
        private string path = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "XML Files|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    mainWindowInputOutput = new DrawElectricityNetworkHelper();
                    GridCanvas.Children.Clear();
                    path = openFileDialog.FileName;
                    mainWindowInputOutput.LoadAndParseXML(path);
                    mainWindowInputOutput.ScaleCanvas(GridCanvas.Width, GridCanvas.Height);
                    mainWindowInputOutput.ConvertToCanvasCoordinates();
                    mainWindowInputOutput.DrawElements(this.GridCanvas);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error", "Invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }
    }
}
