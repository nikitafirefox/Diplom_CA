using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using SciChart.Charting.Visuals;
using SciChart.Charting3D;

namespace Diplom.Forms
{
    /// <summary>
    /// Логика взаимодействия для SaveCAWindow.xaml
    /// </summary>
    public partial class SaveCAWindow : Window
    {

        public CA_Model CA { get; set; }

        public string FileName { get; set; }

        private Axis _selectAxis;

        public SciChart3DSurface SciChart3D { get; set; }

        public SciChartSurface SciChart2D { get; set; }

        public Chart SelectionChart { get; set; }

        public Axis SelectAxis { get { return _selectAxis; } set { _selectAxis = value; SetLabel(); } }

        private int _selectIndex;

        public int SelectIndex { get { return _selectIndex; } set { _selectIndex = value; SetLabel(); } }

        public SaveCAWindow(bool style)
        {
            InitializeComponent();

            if (style)
            {
                StateLabel.Style = (Style)FindResource("LabelDark");


                s1.Style = (Style)FindResource("SeparatorDarkStyle");
                s2.Style = (Style)FindResource("SeparatorDarkStyle");
                s3.Style = (Style)FindResource("SeparatorDarkStyle");
                s4.Style = (Style)FindResource("SeparatorDarkStyle");


                CancelButton.Style = (Style)FindResource("ButtonDarkStyle");
                CSVSaveButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveBMPButton.Style = (Style)FindResource("ButtonDarkStyle");
                OpenCAStateButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveStateButton.Style = (Style)FindResource("ButtonDarkStyle");
                SaveXML.Style = (Style)FindResource("ButtonDarkStyle");
                OpenXML.Style = (Style)FindResource("ButtonDarkStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));

            }
            else
            {
                StateLabel.Style = (Style)FindResource("LabelLight");


                s1.Style = (Style)FindResource("SeparatorLightStyle");
                s2.Style = (Style)FindResource("SeparatorLightStyle");
                s3.Style = (Style)FindResource("SeparatorLightStyle");
                s4.Style = (Style)FindResource("SeparatorDarkStyle");


                CancelButton.Style = (Style)FindResource("ButtonLightStyle");
                CSVSaveButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveBMPButton.Style = (Style)FindResource("ButtonLightStyle");
                OpenCAStateButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveStateButton.Style = (Style)FindResource("ButtonLightStyle");
                SaveXML.Style = (Style)FindResource("ButtonLightStyle");
                OpenXML.Style = (Style)FindResource("ButtonLightStyle");

                grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }

        }

        private void SetLabel() {

            string s = "";

            switch (SelectAxis)
            {
                case Axis.Ox:
                    s += "Плоскость Ox;";
                    break;

                case Axis.Oy:
                    s += "Плоскость Oy;";
                    break;

                case Axis.Oz:
                    s += "Плоскость Oz;";
                    break;

            }

            s += " №" + SelectIndex;

            StateLabel.Content = s;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void SaveStateButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = FileName;
            dlg.DefaultExt = ".dat"; 
            dlg.Filter = "CA State file|*.dat";


            bool? result = dlg.ShowDialog();

            
            if (result == true)
            {
                string filename = dlg.FileName;

                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, CA);

                }
                
            }
        }

        private void OpenCAStateButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".dat";
            dlg.Filter = "CA State file|*.dat";

            bool? result = dlg.ShowDialog();

            if (result == true) {

                string filename = dlg.FileName;

                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate)) {

                    CA = (CA_Model)formatter.Deserialize(fs);

                }

                _selectIndex = CA.Length / 2;

                SetLabel();
            }

        }

        private void CSVSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = FileName;
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV |*.csv";


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                string filename = dlg.FileName;

                double[,] pollution = CA.GetPollution(_selectIndex, _selectAxis);

                string[,] content = new string[CA.Length + 2,CA.Length + 1];

                for (int i = 0; i < CA.Length + 2; i++)
                {
                    for (int j = 0; j < CA.Length + 1; j++)
                    {
                        content[i,j] = "";
                    }
                }

                content[0,0] = StateLabel.Content.ToString().Replace(";"," ").Replace("Плоскость", "Axis").Replace("№","#") + "Size CA: " + CA.Length;

                string s = "";

                switch (SelectAxis)
                {
                    case Axis.Ox:
                        s += "Y/Z";
                        break;

                    case Axis.Oy:
                        s += "X/Z";
                        break;

                    case Axis.Oz:
                        s += "X/Y" +
                            "";
                        break;

                }

                content[1,0] = s;

                for (int i = 0; i < CA.Length; i++)
                {
                    for (int j = 0; j < CA.Length; j++)
                    {
                        content[i + 2, j + 1] = pollution[i, j].ToString();
                    }
                }

                StringBuilder stringBuilder = new StringBuilder();


                for (int i = 0; i < CA.Length + 2; i++)
                {
                    string[] str = new string[CA.Length + 1];

                    for (int j = 0; j < CA.Length + 1; j++)
                    {
                        str[j] = content[i, j];
                    }
                    


                    stringBuilder.AppendLine(string.Join(";", str));
                }

                File.WriteAllText(filename, stringBuilder.ToString());



            }
        }

        private void SaveBMPButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = FileName;
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "JPEG |*.jpeg";

            bool? result = dlg.ShowDialog();


            if (result == true) {

                string filename = dlg.FileName;

                BitmapSource bitmap;

                switch (SelectionChart) {
                    default:
                    case Chart.PointChart3D:
                    case Chart.MeshChart:
                        bitmap = SciChart3D.ExportToBitmapSource();
                        break;

                    case Chart.PointChart2D:
                        bitmap = SciChart2D.ExportToBitmapSource();
                        break;

                }

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                BitmapFrame outputFrame = BitmapFrame.Create(bitmap);

                encoder.Frames.Add(outputFrame);

                using (FileStream file = File.OpenWrite(filename))
                {
                    encoder.Save(file);
                }



            }

        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = FileName;
            dlg.DefaultExt = ".xml";
            dlg.Filter = "CA State file|*.xml";


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                string filename = dlg.FileName;

                CA.ToXML(filename);

            }
        }

        private void OpenXML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "CA State file|*.xml";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {

                string filename = dlg.FileName;

                CA.OpenXML(filename);

                _selectIndex = CA.Length / 2;

                SetLabel();
            }
        }
    }
}
