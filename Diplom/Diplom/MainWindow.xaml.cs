using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting3D;
using SciChart.Charting3D.Axis;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.Modifiers;
using SciChart.Charting3D.PointMarkers;
using SciChart.Charting3D.RenderableSeries;
using SciChart.Data.Model;
using System.Diagnostics;
using SciChart.Charting.Visuals;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals.PointMarkers;
using System.IO;
using CA;
using System.Runtime.Serialization.Formatters.Binary;
using Diplom.Forms;
using SciChart.Charting;
using System.Xml;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Chart {PointChart3D = 0, MeshChart, PointChart2D};

        private int _iteration = 10;

        private string _savePathCA;

        private bool _saveCA = true;

        private bool _styleType = true;

        private CA_Model.Axis _selectAxis = CA_Model.Axis.Ox;

        private int _selectIndex = 50;

        private Color _selectColor = Color.FromRgb(128, 155, 250);

        private byte _selectAlpha = 255;

        private BasePointMarker3D _selectPointMarker = new CubePointMarker3D()
        {
            Size = 3,
        };

        private BasePointMarker _selectPointMarker2D = new SquarePointMarker();

        private Chart _selectChart = Chart.PointChart3D;

        private GradientColorPalette _gradientColor = new GradientColorPalette();

        private SciChart3DSurface SciChart3D { get; set; }

        private SciChartSurface SciChart2D { get; set; } 

        private ManualResetEvent _manualResetEvent = new ManualResetEvent(true);
        
        private Thread _mathThread;

        private bool _isDynamicPaint = true;

        private bool _isStopWatch = true;

        private Stopwatch _stopWatch = new Stopwatch();

        private CA_Model CA { get; set; }

        private bool GetUpdating { get; set;}

        public MainWindow()
        {
            InitializeComponent();

            StyleButton.Checked += StyleButtonToggle;
            StyleButton.Unchecked += StyleButtonToggle;

            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(139, 0, 0), 1));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0.8));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(255, 255, 0), 0.5));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(173, 255, 47), 0.2));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(0, 255, 255), 0.09));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(0, 0, 255), 0.05));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(29, 44, 107), 0.009));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(53, 29, 107), 0.001));
            _gradientColor.GradientStops.Add(new GradientStop(Color.FromRgb(92, 29, 107), 0));

            SciChart3D = new SciChart3DSurface
            {
                ChartModifier = new ModifierGroup3D(new MouseWheelZoomModifier3D(), new OrbitModifier3D())
            };



            SciChart2D = new SciChartSurface()
            {
                ChartModifier = new ModifierGroup(new MouseWheelZoomModifier(), new ZoomPanModifier())
            };

            string pathXML = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\AppConfig.xml");
            _savePathCA = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\AppCA.dat");

            if (!File.Exists(pathXML))
            {
                string dataDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }


                FileStream fs = new FileStream(pathXML, FileMode.Create);
                XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode)
                {
                    Formatting = Formatting.Indented
                };
                xmlOut.WriteStartDocument();
                xmlOut.WriteStartElement("root");
                xmlOut.WriteEndElement();
                xmlOut.WriteEndDocument();
                xmlOut.Close();
                fs.Close();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(pathXML);
                XmlElement xroot = xmlDocument.DocumentElement;

                XmlElement element = xmlDocument.CreateElement("style");
                element.InnerText = "Dark";
                xroot.AppendChild(element);

                element = xmlDocument.CreateElement("saveProperty");
                element.InnerText = _saveCA.ToString();
                xroot.AppendChild(element);

                xmlDocument.Save(pathXML);

                CA = new CA_Model();

                SaveCA();

            }
            else {

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(pathXML);
                XmlElement xroot = xmlDocument.DocumentElement;

                
                try
                {
                    _saveCA = bool.Parse(xroot.SelectSingleNode("saveProperty").InnerText);
                }
                catch
                {
                    _saveCA = true;
                }

                try
                {
                    _styleType = !(xroot.SelectSingleNode("style").InnerText == "Light");
                }
                catch
                {
                    _styleType = true;
                }

                if (_saveCA)
                {
                    OpenCA();
                }
                else {
                    CA = new CA_Model();
                }

            }

            _selectIndex = CA.Length / 2;

            SetAxis();


            AddCharting();

            Paint();
          
        }

        private void AddCharting() {
            Grid1.Children.Clear();

            switch (_selectChart) {
                case Chart.PointChart3D:
                case Chart.MeshChart:
                    Grid1.Children.Add(SciChart3D);
                    break;

                case Chart.PointChart2D:
                    Grid1.Children.Add(SciChart2D);
                    break;
            }
            
        }

        private void Paint() {

            switch (_selectChart) {

                case (int)Chart.PointChart3D:

                    var grath_3D = new XyzDataSeries3D<int>();

                    for (int x = 0; x < CA.Length; x++)
                    {
                        for (int y = 0; y < CA.Length; y++)
                        {
                            for (int z = 0; z < CA.Length; z++)
                            {
                                if (CA[x, y, z])
                                {
                                    grath_3D.Append(x, y, z);
                                }
                            }
                        }
                    }

                    var scatterSeries3D = new ScatterRenderableSeries3D
                    {
                        DataSeries = grath_3D,
                        PointMarker = _selectPointMarker

                    };

                    scatterSeries3D.PointMarker.Fill = Color.FromArgb(_selectAlpha, _selectColor.R, _selectColor.G, _selectColor.B);

                    SciChart3D.RenderableSeries.Clear();

                    SciChart3D.RenderableSeries.Add(scatterSeries3D);

                    SetName("Клеточный автомат; Итерация =" + CA.Iterator);

                    break;

                case Chart.MeshChart:
                    var meshGrath_3D = new UniformGridDataSeries3D<double>(CA.Length, CA.Length);



                    double[,] pollution = CA.GetPollution(_selectIndex, _selectAxis);
                    
                    for (int i = 0; i < CA.Length; i++)
                    {
                        for (int j = 0; j < CA.Length; j++)
                        {
                            meshGrath_3D[i, j] = pollution[i, j];

                        }
                    }




                    var meshRenderableSeries = new SurfaceMeshRenderableSeries3D() {
                        MeshColorPalette = _gradientColor,
                        DrawMeshAs = DrawMeshAs.SolidWithContours,
                        DataSeries = meshGrath_3D
                    };
                 
                 



                    SciChart3D.RenderableSeries.Clear();
                     SciChart3D.RenderableSeries.Add(meshRenderableSeries);

                    

                    SetName("Осреднение по плоскости " + CA_Model.GetNameAxis(_selectAxis) + "; Слой = "+ _selectIndex + "; Итерация =" + CA.Iterator);

                    break;

                case Chart.PointChart2D:

                    var dataGraph_2D = new XyDataSeries<int, int>();

                    for (int i = 0; i < CA.Length; i++)
                    {
                        for (int j = 0; j < CA.Length; j++)
                        {
                            if (CA.GetPointPollution(_selectAxis, _selectIndex, i, j)) {

                                dataGraph_2D.Append(i, j);

                            }

                        }

                    }

                    var xyScatter = new XyScatterRenderableSeries()
                    {
                        DataSeries = dataGraph_2D,
                        PointMarker = _selectPointMarker2D   
                    };

                    xyScatter.PointMarker.Fill = Color.FromArgb(_selectAlpha, _selectColor.R, _selectColor.G, _selectColor.B);

                    SciChart2D.RenderableSeries.Clear();
                    SciChart2D.RenderableSeries.Add(xyScatter);

                    SetName("Клеточный автомат; Плоскость " + CA_Model.GetNameAxis(_selectAxis)+"; Слой = " + _selectIndex  + "; Итерация =" + CA.Iterator);

                    break;

            }

            
        

            GetUpdating = false;
        }

        private void SetAxis() {
            switch (_selectChart) {
                case Chart.PointChart3D:
                        SetAxis3D();
                    break;

                case Chart.MeshChart:

                    string titel3D_1 = "";
                    string titel3D_2 = "";

                    switch (_selectAxis) {
                            case (int)CA_Model.Axis.Ox:
                                titel3D_1 = "Y";
                                titel3D_2 = "Z";
                            break;

                        case CA_Model.Axis.Oy:
                                titel3D_1 = "X";
                                titel3D_2 = "Z";
                            break;


                        case CA_Model.Axis.Oz:
                            titel3D_1 = "X";
                            titel3D_2 = "Y";
                        break;

                    }

                    SetMeshAxis3D(titel3D_1, "Pollution", titel3D_2);

                    break;

                case Chart.PointChart2D:

                    string titel2D_1 = "";
                    string titel2D_2 = "";

                    switch (_selectAxis)
                    {
                        case CA_Model.Axis.Ox:
                            titel2D_1 = "Y";
                            titel2D_2 = "Z";
                            break;

                        case CA_Model.Axis.Oy:
                            titel2D_1 = "X";
                            titel2D_2 = "Z";
                            break;


                        case CA_Model.Axis.Oz:
                            titel2D_1 = "X";
                            titel2D_2 = "Y";
                            break;

                    }

                    SetAxis2D(titel2D_1, titel2D_2);

                    break;
            }
        }

        private void SetAxis3D() {
            SciChart3D.XAxis = new NumericAxis3D() { AxisTitle = "X", VisibleRange = new DoubleRange(0, CA.Length-1) };
            SciChart3D.YAxis = new NumericAxis3D() { AxisTitle = "Y", VisibleRange = new DoubleRange(0, CA.Length-1) };
            SciChart3D.ZAxis = new NumericAxis3D() { AxisTitle = "Z", VisibleRange = new DoubleRange(0, CA.Length-1) };
        }

        private void SetAxis2D(string title1, string title2) {
            SciChart2D.XAxis = new NumericAxis { AxisTitle = title1, VisibleRange = new DoubleRange(0, CA.Length - 1), VisibleRangeLimit = new DoubleRange(0, CA.Length - 1) };
            SciChart2D.YAxis = new NumericAxis { AxisTitle = title2, VisibleRange = new DoubleRange(0, CA.Length - 1), VisibleRangeLimit = new DoubleRange(0, CA.Length - 1) };
        }

        private void SetMeshAxis3D(string titelX,string titleY, string titleZ)
        {
            SciChart3D.XAxis = new NumericAxis3D() { AxisTitle = titelX, VisibleRange = new DoubleRange(0, CA.Length - 1) };
            SciChart3D.YAxis = new NumericAxis3D() { AxisTitle = titleY, VisibleRange = new DoubleRange(0,1)};
            SciChart3D.ZAxis = new NumericAxis3D() { AxisTitle = titleZ, VisibleRange = new DoubleRange(0, CA.Length - 1) };
        }

        private void SetName(string title) {
            switch (_selectChart) {

                case Chart.MeshChart:
                case Chart.PointChart3D:
                    SciChart3D.ChartTitle = title;
                    break;
                case Chart.PointChart2D:
                    SciChart2D.ChartTitle = title;
                    break;

            }
            
        }

        private void NextStep(object k) {
            _stopWatch.Restart();
            NextStep((int)k);
        }

        private void NextStep(int k) {
            for (int i = 0; i < k; i++)
            {
                NextStep();
            }
            _stopWatch.Stop();
            Dispatcher.BeginInvoke((Action)(()=>{
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                PauseButton.IsEnabled = false;
                SettingsButton.IsEnabled = true;
                AddPollutionButton.IsEnabled = true;
                ChartTypButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                WindButton.IsEnabled = true;

                if (!_isDynamicPaint) Paint();
                if (_isStopWatch) MessageBox.Show("Время работы "+_stopWatch.ElapsedMilliseconds + " мс","Затраченное время");

                SaveCA();

            }));
        }

        private void NextStep() {
            CA.Next();

            if (_isDynamicPaint)
            {
                GetUpdating = true;

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Paint();
                }));
            }
            else {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    switch (_selectChart) {
                        case Chart.PointChart3D:
                                SetName("Клеточный автомат; Итерация =" + CA.Iterator);
                            break;
                        case Chart.MeshChart:
                                SetName("Осреднение по оси " + CA_Model.GetNameAxis(_selectAxis) + "; Слой = " + _selectIndex + "; Итерация =" + CA.Iterator);
                            break;
                        case Chart.PointChart2D:
                                SetName("Клеточный автомат; Разрез по оси " + CA_Model.GetNameAxis(_selectAxis) + "; Слой = " + _selectIndex + "; Итерация =" + CA.Iterator);
                            break;
                    }

                    
                    
                }));
            }

            while (GetUpdating) {}
            _manualResetEvent.WaitOne();

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            StopButton.IsEnabled = true;
            SettingsButton.IsEnabled = false;
            AddPollutionButton.IsEnabled = false;
            ChartTypButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            WindButton.IsEnabled = false;


            if (_mathThread == null) {
                ThreadStarted();
            }else if (_mathThread.IsAlive)
            {
                _manualResetEvent.Set();
            }
            else
            {
                ThreadStarted();   
            }

        }

        private void ThreadStarted() {
            _manualResetEvent.Set();
            _mathThread = new Thread(new ParameterizedThreadStart(NextStep)) { Name = "Метод Next",IsBackground = true};
            _mathThread.Start(_iteration);
        }

        private void ThreadStoped() {

            if (_mathThread != null) {

                if (_mathThread.IsAlive) {

                    _mathThread.Abort();

                    _mathThread.Join();

                }

            }

            WindButton.IsEnabled = true;
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            SettingsButton.IsEnabled = true;
            AddPollutionButton.IsEnabled = true;
            ChartTypButton.IsEnabled = true;
            SaveButton.IsEnabled = true;

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _manualResetEvent.Reset();

            if (!_isDynamicPaint) {
                Paint();
            }

            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            SettingsButton.IsEnabled = true;
            AddPollutionButton.IsEnabled = true;
            ChartTypButton.IsEnabled = true;
            SaveButton.IsEnabled = true;
            WindButton.IsEnabled = true;

            SaveCA();

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStoped();

            Paint();

            SaveCA();

            
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow(_styleType) {
                Length = CA.Length,
                Iteration = _iteration,
                Ray = CA.Ray,
                IsDynamicVisual = _isDynamicPaint,
                IsStopWatch = _isStopWatch,
                NameCA = CA.Name
            };

            if (settings.ShowDialog() == true) {

                if (settings.Length != CA.Length) {

                    ThreadStoped();

                    CA.Length = settings.Length;
                    _selectIndex = CA.Length / 2;
                    SetAxis();
                    Paint();
                    SaveCA();
                }

                if ((_selectChart == Chart.MeshChart) && (CA.Ray != settings.Ray))
                {
                    CA.Ray = settings.Ray;
                    Paint();
                }
                else {
                    CA.Ray = settings.Ray;
                }



                if (_iteration != settings.Iteration){

                    if (_iteration > settings.Iteration) {
                        ThreadStoped();

                    }
 
                    _iteration = settings.Iteration;
                }


                _isDynamicPaint = settings.IsDynamicVisual;
                _isStopWatch = settings.IsStopWatch;

                
            }
        }

        private void AddPollutionButton_Click(object sender, RoutedEventArgs e)
        {
            AddPollutionWindow pollutionWindow = new AddPollutionWindow(_styleType) { Length = CA.Length };

            if (pollutionWindow.ShowDialog() == true) {

                CA.AddPollution(pollutionWindow.XStart, pollutionWindow.XEnd, 
                    pollutionWindow.YStart, pollutionWindow.YEnd, 
                    pollutionWindow.ZStart, pollutionWindow.ZEnd);

                Paint();

                SaveCA();

            }
        }

        private void RemoveCAButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStoped();

            CA.Remove();

            Paint();

            SaveCA();

        }

        private void ChartTypButton_Click(object sender, RoutedEventArgs e)
        {
            TypeChartWindow chartWindow = new TypeChartWindow(_styleType) {
                Length = CA.Length,
                SelectAlpha = _selectAlpha,
                SelectAxis = _selectAxis,
                SelectColor = _selectColor,
                SelectPoint = GetIndexPointType(_selectPointMarker),
                SelectSize = (int)_selectPointMarker.Size,
                SelectIndex = _selectIndex,
                SelectChart = _selectChart,
                SelectPoint2D = GetIndexPoint2DType(_selectPointMarker2D)
            };

            if (chartWindow.ShowDialog() == true) {

                _selectAlpha = chartWindow.SelectAlpha;
                if (((_selectChart == Chart.PointChart2D) && (chartWindow.SelectChart < Chart.PointChart2D)) || ((chartWindow.SelectChart == Chart.PointChart2D) && (_selectChart < Chart.PointChart2D)))
                {
                    _selectChart = chartWindow.SelectChart;
                    AddCharting();
                }
                else
                {
                    _selectChart = chartWindow.SelectChart;
                }
                _selectColor = chartWindow.SelectColor;
                _selectIndex = chartWindow.SelectIndex;
                _selectAxis = chartWindow.SelectAxis;
                _selectPointMarker = GetPoint(chartWindow.SelectPoint, chartWindow.SelectSize);
                _selectPointMarker2D = GetPoint2D(chartWindow.SelectPoint2D);

                SetAxis();
                Paint();

            }
        }

        public static int GetIndexPointType(BasePointMarker3D point) {
            if (point is SpherePointMarker3D)
            {
                return 4;
            }
            else if (point is CubePointMarker3D)
            {
                return 3;
            }
            else if (point is QuadPointMarker3D)
            {
                return 2;
            }
            else if (point is EllipsePointMarker3D)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public static BasePointMarker3D GetPoint(int index, int size) {
            if (index == 4)
            {
                return new SpherePointMarker3D() {Size = size};
            }
            else if (index == 3)
            {
                return new CubePointMarker3D() { Size = size }; 
            }
            else if (index == 2)
            {
                return new QuadPointMarker3D() { Size = size };
            }
            else if (index == 1)
            {
                return new EllipsePointMarker3D() { Size = size };
            }
            else
            {
                return new PixelPointMarker3D() { Size = size };
            }
        }

        public static int GetIndexPoint2DType(BasePointMarker point) {
            if (point is SquarePointMarker)
            {
                return 1;
            }
            else {
                return 0;
            }

        }

        public static BasePointMarker GetPoint2D(int index) {
            if (index == 1)
            {
                return new SquarePointMarker();
            }
            else {
                return new EllipsePointMarker();
            }
        }

        private void ResetCAButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStoped();

            CA.Reset();

            Paint();

            SaveCA();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCAWindow saveCAWindow = new SaveCAWindow(_styleType)
            {
                CA = CA, SelectAxis = _selectAxis,
                SelectIndex = _selectIndex,
                SelectionChart = _selectChart,
                SciChart2D = SciChart2D,
                SciChart3D = SciChart3D,
                FileName = CA.Name

            };

            saveCAWindow.ShowDialog();

            CA = saveCAWindow.CA;

            _selectIndex = saveCAWindow.SelectIndex;

            SetAxis();

            Paint();

        }

        private void StyleButtonToggle(object sender, RoutedEventArgs e)
        {
            _styleType = (bool)StyleButton.IsChecked;

            if ((bool)StyleButton.IsChecked)
            {

                ThemeManager.SetTheme(SciChart3D, "SciChartv4Dark");
                ThemeManager.SetTheme(SciChart2D, "SciChartv4Dark");
                Grid1.Background = new SolidColorBrush(Color.FromArgb(255,28,28,30));
                windowGrid.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));
                stackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 30));
            }
            else {
                ThemeManager.SetTheme(SciChart3D, "BrightSpark");
                ThemeManager.SetTheme(SciChart2D, "BrightSpark");
                Grid1.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
                windowGrid.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
                stackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 249, 249, 249));
            }
        }

        private void SaveCA()
        {
            if (_saveCA)
            {
                string dataDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(_savePathCA, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, CA);

                }
            }

        }

        private void OpenCA()
        {
            if (File.Exists(_savePathCA))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(_savePathCA, FileMode.OpenOrCreate))
                {

                    CA = (CA_Model)formatter.Deserialize(fs);

                }
            }
            else {
                CA = new CA_Model();
                SaveCA();
            }
        }

        private void WindButton_Click(object sender, RoutedEventArgs e)
        {
            WindWindow windWindow = new WindWindow(_styleType) { MaxWind = CA.MaxWind,Wind = CA.Wind,Angel = CA.Angel};

            if (windWindow.ShowDialog() == true)
            {

                CA.Wind = windWindow.Wind;
                CA.Angel = windWindow.Angel;

                SaveCA();

            }
        }
    }
}
