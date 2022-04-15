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
    /// Interaction logic for TextBlockWindow.xaml
    /// </summary>
    public partial class TextBlockWindow : Window
    {
        #region Constructors

        public TextBlockWindow()
        {
            InitializeComponent();
        }
        public TextBlockWindow(string text, int size, Color textColor)
        {
            InitializeComponent();
            textBoxText.Text = text;
            textBoxSize.Text = size.ToString();
            colorPickerTextColor.SelectedColor = textColor;
        }

        #endregion

        #region Events

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int temp;

            if (!string.IsNullOrWhiteSpace(textBoxText.Text)
                && int.TryParse(textBoxSize.Text, out temp)
                && colorPickerTextColor.SelectedColor.HasValue)
            {
                ((MainWindow)Application.Current.MainWindow).DrawTextBlock(textBoxText.Text,
                                                                     int.Parse(textBoxSize.Text),
                                                                     colorPickerTextColor.SelectedColor.Value);
                this.Close();
            }
        }

        #endregion
    }
}
