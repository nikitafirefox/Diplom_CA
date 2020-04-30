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

namespace Diplom.Forms
{
    /// <summary>
    /// Логика взаимодействия для WindWindow.xaml
    /// </summary>
    public partial class WindWindow : Window
    {
        public double MaxWind { get; set; }

        private double _width;

        public double Wind { get { return _width; } set { _width = value; WindTextBox.Text = _width.ToString().Replace(",","."); } }

        private double _angel;

        public double Angel { get { return _angel; } set { _angel = value; AngelTextBox.Text = _angel.ToString().Replace(",", "."); } }

        public WindWindow(bool style)
        {
            InitializeComponent();

            if (style)
            {
                l1.Style = (Style)FindResource("LabelDark");
                l2.Style = (Style)FindResource("LabelDark");
                

                s1.Style = (Style)FindResource("SeparatorDarkStyle");


                AngelTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                WindTextBox.Style = (Style)FindResource("TextBoxDarkStyle");

                CancelButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveButton.Style = (Style)FindResource("ButtonDarkStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));

            }
            else
            {
                l1.Style = (Style)FindResource("LabelLight");
                l2.Style = (Style)FindResource("LabelLight");

                s1.Style = (Style)FindResource("SeparatorLightStyle");

                WindTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                AngelTextBox.Style = (Style)FindResource("TextBoxLightStyle");


                CancelButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveButton.Style = (Style)FindResource("ButtonLightStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }


            AngelTextBox.TextChanged += TextChanged;
            WindTextBox.TextChanged += TextChanged;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Wind = double.Parse(WindTextBox.Text);
            Angel = double.Parse(AngelTextBox.Text);

            DialogResult = true;

            Close();

        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveButton.IsEnabled = double.TryParse(WindTextBox.Text, out double wind) && double.TryParse(AngelTextBox.Text, out double angel) && (wind >= 0) && (wind <= MaxWind);
        }
    }
}
