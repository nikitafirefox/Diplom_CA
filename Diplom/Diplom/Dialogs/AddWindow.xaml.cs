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
    /// Логика взаимодействия для AddPollutionWindow.xaml
    /// </summary>

    public enum AdditionType {Let = 0, Pollution };

    public partial class AddWindow : Window
    {

        #region properties

        public int Length { get; set; }

        public int XStart { private set; get; }

        public int YStart { private set; get; }

        public int ZStart { private set; get; }

        public int XEnd { private set; get; }

        public int YEnd { private set; get; }

        public int ZEnd { private set; get; }

        public int Frequency { private set; get; }

        public bool Started { private set; get; }

        public AdditionType AdditionType { private set; get; }

        #endregion

        public AddWindow(bool style)
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
                s3.Style = (Style)FindResource("SeparatorDarkStyle");
                s4.Style = (Style)FindResource("SeparatorDarkStyle");
                s5.Style = (Style)FindResource("SeparatorDarkStyle");


                XStartTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                XEndTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                YStartTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                YEndTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                ZStartTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                ZEndTextBox.Style = (Style)FindResource("TextBoxDarkStyle");
                FrequencyTextBox.Style = (Style)FindResource("TextBoxDarkStyle");

                StartPollutionCheckBox.Style = (Style)FindResource("CheckBoxDarkStyle");

                LetRadioButton.Style = (Style)FindResource("RadioButtonDarkStyle");
                PollutionRadioButton.Style = (Style)FindResource("RadioButtonDarkStyle");

                CancelButton.Style = (Style)FindResource("ButtonDarkStyle");
                AddButton.Style = (Style)FindResource("ButtonDarkStyle");

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
                s3.Style = (Style)FindResource("SeparatorLightStyle");
                s4.Style = (Style)FindResource("SeparatorLightStyle");
                s5.Style = (Style)FindResource("SeparatorLightStyle");

                XStartTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                XEndTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                YStartTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                YEndTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                ZStartTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                ZEndTextBox.Style = (Style)FindResource("TextBoxLightStyle");
                FrequencyTextBox.Style = (Style)FindResource("TextBoxLightStyle");

                StartPollutionCheckBox.Style = (Style)FindResource("CheckBoxLightStyle");

                LetRadioButton.Style = (Style)FindResource("RadioButtonLightStyle");
                PollutionRadioButton.Style = (Style)FindResource("RadioButtonLightStyle");

                CancelButton.Style = (Style)FindResource("ButtonLightStyle");
                AddButton.Style = (Style)FindResource("ButtonLightStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }

            XStartTextBox.TextChanged += TextBoxs_TextChanged;
            YStartTextBox.TextChanged += TextBoxs_TextChanged;
            ZStartTextBox.TextChanged += TextBoxs_TextChanged;

            XEndTextBox.TextChanged += TextBoxs_TextChanged;
            YEndTextBox.TextChanged += TextBoxs_TextChanged;
            ZEndTextBox.TextChanged += TextBoxs_TextChanged;
            FrequencyTextBox.TextChanged += TextBoxs_TextChanged;

            StartPollutionCheckBox.Checked += CheckBox_Checked;
            StartPollutionCheckBox.Unchecked += CheckBox_Checked;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TextBoxs_TextChanged(object sender, TextChangedEventArgs e) => Lock();

        private void CheckBox_Checked(object sender, RoutedEventArgs e) => Lock();

        private void Lock()
        {
            AddButton.IsEnabled = ((int.TryParse(XStartTextBox.Text, out int xStart) && int.TryParse(YStartTextBox.Text, out int yStart)
                && int.TryParse(ZStartTextBox.Text, out int zStart) && int.TryParse(XEndTextBox.Text, out int xEnd)
                && int.TryParse(YEndTextBox.Text, out int yEnd) && int.TryParse(ZEndTextBox.Text, out int zEnd) && int.TryParse(FrequencyTextBox.Text, out int frequency))
                && ((xStart >= 0) && (xEnd < Length) && (xStart <= xEnd)) && ((yStart >= 0) && (yEnd < Length) && (yStart <= yEnd))
                && ((zStart >= 0) && (zEnd < Length) && (zStart <= zEnd)) && frequency >= 0);

            if (int.TryParse(FrequencyTextBox.Text, out frequency) && frequency > 0)
            {
                StartPollutionCheckBox.IsEnabled = true;
            }
            else
            {
                StartPollutionCheckBox.IsEnabled = false;
                StartPollutionCheckBox.IsChecked = true;
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            XStart = int.Parse(XStartTextBox.Text);
            YStart = int.Parse(YStartTextBox.Text);
            ZStart = int.Parse(ZStartTextBox.Text);

            XEnd = int.Parse(XEndTextBox.Text);
            YEnd = int.Parse(YEndTextBox.Text);
            ZEnd = int.Parse(ZEndTextBox.Text);

            Frequency = int.Parse(FrequencyTextBox.Text);
            Started = StartPollutionCheckBox.IsChecked == true;

            if (PollutionRadioButton.IsChecked == true)
            {
                AdditionType = AdditionType.Pollution;
            }
            else
            {
                AdditionType = AdditionType.Let;
            }

            Close();

        }

        private void PollutionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FrequencyTextBox.IsEnabled = true;
        }

        private void LetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FrequencyTextBox.IsEnabled = false;
            FrequencyTextBox.Text = "0";
        }
    }
}
