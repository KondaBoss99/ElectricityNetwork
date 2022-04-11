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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    DefaultExt = "xml",
                    Filter = "XML Files|*.xml"
                };

                if (openFileDialog.ShowDialog().GetValueOrDefault())
                {
                    DrawElectricityNetworkHelper.LoadXMLContent(openFileDialog.FileName, DrawingNetworkCanvas.Width, DrawingNetworkCanvas.Height);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
