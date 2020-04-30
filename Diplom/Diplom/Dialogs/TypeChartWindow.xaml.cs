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
using CA;


namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для TypeChartWindow.xaml
    /// </summary>
    public partial class TypeChartWindow : Window
    {   
        public int Length { get; set; }

        private int _selectIndex = -1;

        public int SelectIndex {

            set {
                _selectIndex = value;
                IndexTextBox.Text = _selectIndex.ToString();
            }

            get { return _selectIndex; }
        }

        private byte _selectAlpha = 255;

        public byte SelectAlpha {
            set {
                _selectAlpha = value;
                AlphaTextBox.Text = _selectAlpha.ToString();
            }
            get {
                return _selectAlpha;
            }
        }

        private CA_Model.Axis _selectAxis = CA_Model.Axis.Ox;

        public CA_Model.Axis SelectAxis {
            set {
                _selectAxis = value;
                AxisMeshComboBox.SelectedIndex = (int)_selectAxis;
            }
            get {
                return _selectAxis;
            }
        }

        private int _selectSize = 1;

        public int SelectSize {
            set {
                _selectSize = value;
                SizeTextBox.Text = value.ToString();
            }
            get {
                return _selectSize;
            }
        }

        private int _selectPoint = 0;

        public int SelectPoint {
            set {
                _selectPoint = value;
                PointTypeComboBox.SelectedIndex = value;
            }
            get {
                return _selectPoint;
            }
        }

        private Color _selectColor = Color.FromRgb(128, 155, 250);

        public Color SelectColor{
            set {
                _selectColor = value;

                ColorButton.Background = new SolidColorBrush(_selectColor);
            }
            get {
                return _selectColor;
            }
        }

        private int _selectPoint2D = 0;

        public int SelectPoint2D {
            get {
                return _selectPoint2D;
            }
            set {
                _selectPoint2D = value;
                Point2DComboBox.SelectedIndex = value;
            }
        }

        private MainWindow.Chart _selectChart = 0;

        public MainWindow.Chart SelectChart {
            set {
                _selectChart = value;
                switch (value) {
                    default:
                    case MainWindow.Chart.PointChart3D:
                        Point3DRadio.IsChecked = true;
                        break;
                    case MainWindow.Chart.MeshChart:
                        MeshRadio.IsChecked = true;
                        break;
                    case MainWindow.Chart.PointChart2D:
                        Point2DRadio.IsChecked = true;
                        break;
                        
                }

            }
            get {
                return _selectChart;
            }
        }

        public TypeChartWindow(bool style)
        {
            InitializeComponent();

            if (style)
            {
                l1.Style = (Style)FindResource("LabelDark");
                l2.Style = (Style)FindResource("LabelDark");
                l3.Style = (Style)FindResource("LabelDark");
                l4.Style = (Style)FindResource("LabelDark");
                l5.Style = (Style)FindResource("LabelDark");
                l6.Style = (Style)FindResource("LabelDark");
                l7.Style = (Style)FindResource("LabelDark");

                s1.Style = (Style)FindResource("SeparatorDarkStyle");
                s2.Style = (Style)FindResource("SeparatorDarkStyle");

                SizeTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                AlphaTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                IndexTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                
                PointTypeComboBox.Style = (Style)FindResource("ComboBoxDarkStyle");
                AxisMeshComboBox.Style = (Style)FindResource("ComboBoxDarkStyle");
                Point2DComboBox.Style = (Style)FindResource("ComboBoxDarkStyle");

                MeshRadio.Style = (Style)FindResource("RadioButtonDarkStyle");
                Point2DRadio.Style = (Style)FindResource("RadioButtonDarkStyle");
                Point3DRadio.Style = (Style)FindResource("RadioButtonDarkStyle");

                CancelButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveButton.Style = (Style)FindResource("ButtonDarkStyle");
                ColorButton.Style = (Style)FindResource("ButtonDarkStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));

            }
            else
            {
                l1.Style = (Style)FindResource("LabelLight");
                l2.Style = (Style)FindResource("LabelLight");
                l3.Style = (Style)FindResource("LabelLight");
                l4.Style = (Style)FindResource("LabelLight");
                l5.Style = (Style)FindResource("LabelLight");
                l6.Style = (Style)FindResource("LabelLight");
                l7.Style = (Style)FindResource("LabelLight");

                s1.Style = (Style)FindResource("SeparatorLightStyle");
                s2.Style = (Style)FindResource("SeparatorLightStyle");

                SizeTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                AlphaTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                IndexTextBox.Style = (Style)FindResource("TextBoxLightStyle");

                PointTypeComboBox.Style = (Style)FindResource("ComboBoxLightStyle");
                AxisMeshComboBox.Style = (Style)FindResource("ComboBoxLightStyle");
                Point2DComboBox.Style = (Style)FindResource("ComboBoxLightStyle");

                MeshRadio.Style = (Style)FindResource("RadioButtonLightStyle");
                Point2DRadio.Style = (Style)FindResource("RadioButtonLightStyle");
                Point3DRadio.Style = (Style)FindResource("RadioButtonLightStyle");

                CancelButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveButton.Style = (Style)FindResource("ButtonLightStyle");
                ColorButton.Style = (Style)FindResource("ButtonLightStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }

            MeshRadio.Checked += MeshRadio_Checked;
            Point3DRadio.Checked += Point3DRadio_Checked;
            Point2DRadio.Checked += Point2DRadio_Checked;

            AlphaTextBox.TextChanged += Box_TextChanged;
            SizeTextBox.TextChanged += Box_TextChanged;
            IndexTextBox.TextChanged += Box_TextChanged;

            AxisMeshComboBox.SelectionChanged += ComboBox_SelectionChanged;
            PointTypeComboBox.SelectionChanged += ComboBox_SelectionChanged;
            Point2DComboBox.SelectionChanged += ComboBox_SelectionChanged;


        }


        private void Lock() {
            SaveButton.IsEnabled = ((byte.TryParse(AlphaTextBox.Text, out byte selectAlpha) && (int.TryParse(SizeTextBox.Text, out int selectSize))
                && (int.TryParse(IndexTextBox.Text, out int selectIndex))) && ((selectAlpha >= 0) && (selectAlpha <= 255) && (selectSize > 0) && (selectSize < 6)
                && (selectIndex >= 0) && (selectIndex < Length)));
        }

        
        private void Point3DRadio_Checked(object sender, RoutedEventArgs e)
        {
            _selectChart = MainWindow.Chart.PointChart3D;
        }

        private void MeshRadio_Checked(object sender, RoutedEventArgs e)
        {
            _selectChart = MainWindow.Chart.MeshChart;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.Color = System.Drawing.Color.FromArgb(_selectColor.R, _selectColor.G, _selectColor.B);

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

                _selectColor = Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                ColorButton.Background = new SolidColorBrush(_selectColor);
            }
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lock();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lock();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _selectAlpha = byte.Parse(AlphaTextBox.Text);
            _selectSize = int.Parse(SizeTextBox.Text);
            _selectIndex = int.Parse(IndexTextBox.Text);

            _selectPoint = PointTypeComboBox.SelectedIndex;
            _selectAxis = (CA_Model.Axis)AxisMeshComboBox.SelectedIndex;
            _selectPoint2D = Point2DComboBox.SelectedIndex;
          
            Close();


        }

        private void Point2DRadio_Checked(object sender, RoutedEventArgs e)
        {
            _selectChart = MainWindow.Chart.PointChart2D;
        }
    }
}
