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

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private int _ray = 5;

        public int Ray { get { return _ray; } set { _ray = value; RayTextBox.Text = value.ToString(); } }

        private string _nameCA = "newCA";

        public string NameCA { get { return _nameCA; } set { _nameCA = value; NameCATextBox.Text = value; } }

        private int _length = 100;

        public int Length { get { return _length; } set { _length = value; LengthTextBox.Text = value.ToString();} }

        private int _iteration = 10;

        public int Iteration { get { return _iteration; } set { _iteration = value; IterationCountTextBox.Text = value.ToString(); } }

        private bool _isDynamicVisual = true;

        public bool IsDynamicVisual { get { return _isDynamicVisual; } set { _isDynamicVisual = value; DynamicVisualCheckBox.IsChecked = value; } }

        private bool _isStopWatch = false;

        public bool IsStopWatch { get { return _isStopWatch; } set { _isStopWatch = value; StopWatchCheckBox.IsChecked = value; } }


        public SettingsWindow(bool style)
        {
            InitializeComponent();

            if (style)
            {
                l1.Style = (Style)FindResource("LabelDark");
                l2.Style = (Style)FindResource("LabelDark");
                l3.Style = (Style)FindResource("LabelDark");
                l4.Style = (Style)FindResource("LabelDark");

                s1.Style = (Style)FindResource("SeparatorDarkStyle");
                s2.Style = (Style)FindResource("SeparatorDarkStyle");
                s3.Style = (Style)FindResource("SeparatorDarkStyle");

                IterationCountTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                LengthTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                NameCATextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                RayTextBox.Style = (Style)FindResource("TextBoxDarkStyle");

                DynamicVisualCheckBox.Style = (Style)FindResource("CheckBoxDarkStyle");
                StopWatchCheckBox.Style = (Style)FindResource("CheckBoxDarkStyle");

                CancelButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveButton.Style = (Style)FindResource("ButtonDarkStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));

            }
            else {
                l1.Style = (Style)FindResource("LabelLight");
                l2.Style = (Style)FindResource("LabelLight");
                l3.Style = (Style)FindResource("LabelLight");
                l4.Style = (Style)FindResource("LabelLight");

                s1.Style = (Style)FindResource("SeparatorLightStyle");
                s2.Style = (Style)FindResource("SeparatorLightStyle");
                s3.Style = (Style)FindResource("SeparatorLightStyle");

                IterationCountTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                LengthTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                NameCATextBox.Style = (Style)FindResource("TextBoxLightStyle");
                RayTextBox.Style = (Style)FindResource("TextBoxLightStyle");

                DynamicVisualCheckBox.Style = (Style)FindResource("CheckBoxLightStyle");
                StopWatchCheckBox.Style = (Style)FindResource("CheckBoxLightStyle");

                CancelButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveButton.Style = (Style)FindResource("ButtonLightStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }

            LengthTextBox.TextChanged += TextBox_TextChanged;
            RayTextBox.TextChanged += TextBox_TextChanged;
            IterationCountTextBox.TextChanged += TextBox_TextChanged;
            NameCATextBox.TextChanged += TextBox_TextChanged;

            DynamicVisualCheckBox.Checked += CheckBox_Checked;
            StopWatchCheckBox.Checked += CheckBox_Checked;
            DynamicVisualCheckBox.Unchecked += CheckBox_Checked;
            StopWatchCheckBox.Unchecked += CheckBox_Checked;


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => Lock();

        private void CheckBox_Checked(object sender, RoutedEventArgs e) => Lock();


        private void Lock() {
            SaveButton.IsEnabled = ((int.TryParse(LengthTextBox.Text, out int len) && int.TryParse(IterationCountTextBox.Text, out int iter)
                && int.TryParse(RayTextBox.Text, out int ray) && (len >= 10) && (len <= 300) && (iter > 0) && (iter <= 1000) && (ray >= 0) && (ray <= 100))
                && ((ray != _ray) || (len != _length) || (iter != _iteration) || (_isDynamicVisual != DynamicVisualCheckBox.IsChecked) || (_isStopWatch != StopWatchCheckBox.IsChecked) || 
                (NameCATextBox.Text != _nameCA)));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            Length = int.Parse(LengthTextBox.Text);
            Iteration = int.Parse(IterationCountTextBox.Text);
            Ray = int.Parse(RayTextBox.Text);

            IsDynamicVisual =(bool)DynamicVisualCheckBox.IsChecked;
            IsStopWatch = (bool)StopWatchCheckBox.IsChecked;

            NameCA = NameCATextBox.Text;

            DialogResult = true;

            Close();
        }


    }
}
