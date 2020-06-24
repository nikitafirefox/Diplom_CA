using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace  CA
{

    [Serializable]
    public class CA_Model
    {
        public enum Axis { Ox = 0, Oy, Oz }

        public enum ThreadDispatcherType {Auto = 0, Async, Sync }

        protected ThreadDispatcherType _selectThreadDispatcher = ThreadDispatcherType.Auto;

        protected double _wind = 0;

        public double MaxWind { get; private set; } = 20;

        public double Angel { get; set; } = 0;

        public double Wind {
            get { return _wind; }
            set {
                if (value < 0)
                {
                    _wind = 0;
                }
                else if (value > MaxWind)
                {
                    _wind = MaxWind;
                }
                else {
                    _wind = value;
                }
            }
        }

        protected int _countThread;

        public static string GetNameAxis(Axis i) {

            switch (i) {
                case Axis.Ox:
                    return "Ox";

                case Axis.Oy:
                    return "Oy";

                case Axis.Oz:
                    return "Oz";

                default:
                    return "";
            }
        }

        private int _length;

        public string Name { get; set; } = "New_CA";

        public int Length {
            set {

                _length = value;
                Iterator = 0;
                _pollutions.Clear();
                if (value < MaxWind) {
                    MaxWind = value / 2;
                }
                Auto = new bool[Length, Length, Length];

            }
            get {
                return _length;
            } }

        public int Iterator { private set; get; }

        private List<Pollution> _pollutions = new List<Pollution>();

        private double _pAxis = 1.0 / 3.0;

        private double _pDirection = 1.0 / 3.0;

        public double PAxis {
            get
            {
                return _pAxis;
            }
            set {
                if (value > 1) value = 1.0 / 3.0;
                if (value < 0) value = 1.0 / 3.0;
                if (value > 0.5) value = 1.0 / 3.0;

                _pAxis = value;
            }
        }

        public double Pdirection {
            get { return _pDirection; }

            set {
                if (value > 1) value = 1.0 / 3.0;
                if (value < 0) value = 1.0 / 3.0;
                if (value > 0.5) value = 1.0 / 3.0;

                _pDirection = value;
            }
        }

        private Random _randomAxis = new Random();

        private Random _randomDirection = new Random();

        private Random _randomWind = new Random();

        public int Ray { set; get; }

        private bool[,,] Auto { set; get; }

        public bool this[int x, int y, int z] {
            get {
                return Auto[x, y, z];
            }
        }

        public CA_Model() {

            string pathXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\CAConfig.xml");
            if (!File.Exists(pathXML))
            {
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

               

                Length = 100;
                Ray = 5;
                _countThread = Environment.ProcessorCount;
                AddPollution(25, 75, 25, 75, 25, 75);

                ToXML(pathXML);

            }
            else {

                if (!OpenXML(pathXML)) {
                    ToXML(pathXML);
                }

            }
        }

        public bool OpenXML(string pathXML)
        {

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(pathXML);
            XmlElement xroot = xmlDocument.DocumentElement;

            bool error = false;

            string iteratorValue = "";

            try
            {
                iteratorValue = xroot.SelectSingleNode("iterator").InnerText;
            }
            catch {
                Length = 100;
                Ray = 5;
                _countThread = Environment.ProcessorCount;
                AddPollution(25, 75, 25, 75, 25, 75);
                _pAxis = 1.0 / 3.0;
                _pDirection = 1.0 / 3.0;
                Name = "New_CA";
                _selectThreadDispatcher = ThreadDispatcherType.Auto;
                MaxWind = 20;
                _wind = 0;
                Angel = 0;

                return false;
            }

            string threadDispatcherValue = "";

            try
            {
                threadDispatcherValue = xroot.SelectSingleNode("threadDispatcher").InnerText;
                if (threadDispatcherValue == "Sync")
                {
                    _selectThreadDispatcher = ThreadDispatcherType.Sync;
                }
                else if (threadDispatcherValue == "Async")
                {
                    _selectThreadDispatcher = ThreadDispatcherType.Async;
                }
                else {
                    _selectThreadDispatcher = ThreadDispatcherType.Auto;
                }
            }
            catch {
                _selectThreadDispatcher = ThreadDispatcherType.Auto;
            }

            string windValue = "";

            try
            {
                windValue = xroot.SelectSingleNode("wind").InnerText;
            }
            catch
            {
                error = true;
                windValue = (0).ToString();
            }

            string angelValue = "";

            try
            {
                angelValue = xroot.SelectSingleNode("angel").InnerText;
            }
            catch
            {
                error = true;
                angelValue = (0).ToString();
            }

            string maxWindValue = "";

            try
            {
                maxWindValue = xroot.SelectSingleNode("maxWind").InnerText;
            }
            catch
            {
                error = true;
                maxWindValue = (20).ToString();
            }

            try {
                Name = xroot.SelectSingleNode("name").InnerText;
            }
            catch {
                Name = "NewCA";
                error = true;
            }

            string pAxisValue = "";

            try
            {
                pAxisValue = xroot.SelectSingleNode("pAxis").InnerText;
            }
            catch {
                pAxisValue = (1.0 / 3.0).ToString();
                error = true;
            }

            string pDirectionValue = "";

            try
            {
                pDirectionValue = xroot.SelectSingleNode("pDirection").InnerText;
            }
            catch
            {
                pDirectionValue = (1.0 / 3.0).ToString();
                error = true;
            }

            string countThreadValue;

            try {
                countThreadValue = xroot.SelectSingleNode("countThread").InnerText;
            } catch {
                countThreadValue = (2).ToString();
            }


            string lengthValue = "";
            try
            {
                lengthValue = xroot.SelectSingleNode("length").InnerText;
            }
            catch {
                lengthValue = (100).ToString();
            }



            string rayValue = "";

            try
            {
                rayValue = xroot.SelectSingleNode("ray").InnerText;
            }
            catch {
                rayValue = (5).ToString();
            }




            if (int.TryParse(iteratorValue, out int count))
            {
                Iterator = count;
            }
            else
            {
                Length = 100;
                Ray = 5;
                _countThread = Environment.ProcessorCount;
                AddPollution(25, 75, 25, 75, 25, 75);
                _pAxis = 1.0 / 3.0;
                _pDirection = 1.0 / 3.0;
                Name = "New_CA";
                _selectThreadDispatcher = ThreadDispatcherType.Auto;
                MaxWind = 20;
                _wind = 0;
                Angel = 0;

                return false;
            }

            

            if (double.TryParse(pAxisValue, out double p))
            {
                _pAxis = p;
            }
            else
            {
                error = true;
                _pAxis = 1.0 / 3.0;
            }

            if (double.TryParse(pDirectionValue, out p))
            {
                _pDirection = p;
            }
            else
            {
                error = true;
                _pDirection = 1.0 / 3.0;
            }

            if (int.TryParse(countThreadValue, out count))
            {
                _countThread = count > Environment.ProcessorCount ? Environment.ProcessorCount : count;
            }
            else
            {
                error = true;
                _countThread = Environment.ProcessorCount;
            }

            if (int.TryParse(lengthValue, out count))
            {
                Length = count;
            }
            else
            {
                error = true;
                Length = 100;
            }

            if (double.TryParse(maxWindValue, out p))
            {
                if (MaxWind > 0)
                {
                    MaxWind = p < Length ? p : Length / 2;
                }
                else {
                    MaxWind = 0;
                }
            }
            else
            {
                error = true;
                MaxWind = 20 < Length ? 20 : Length / 2;
            }

            if (double.TryParse(windValue, out p))
            {
                Wind = p; 
            }
            else
            {
                error = true;
                Wind = 0;
            }

            if (double.TryParse(angelValue, out p))
            {
                Angel = p;
            }
            else
            {
                error = true;
                Angel = 0;
            }

            if (int.TryParse(rayValue, out count))
            {
                Ray = count;
            }
            else
            {
                error = true;
                Ray = 5;
            }

            XmlNode xmlNode;

            try
            {
                xmlNode = xroot.GetElementsByTagName("pollutions").Item(0);
            }
            catch {
                error = true;
                return !error;
            }

            foreach (XmlNode x in xmlNode.ChildNodes)
            {
                string xString1, xString2, yString1, yString2, zString1, zString2;

                try
                {
                    xString1 = x.Attributes.GetNamedItem("x1").Value;
                }
                catch {
                    xString1 = "";
                }

                try
                {
                    xString2 = x.Attributes.GetNamedItem("x2").Value;
                }
                catch
                {
                    xString2 = "";
                }

                try
                {
                    yString1 = x.Attributes.GetNamedItem("y1").Value;
                }
                catch
                {
                    yString1 = "";
                }

                try
                {
                    yString2 = x.Attributes.GetNamedItem("y2").Value;
                }
                catch
                {
                    yString2 = "";
                }

                try
                {
                    zString1 = x.Attributes.GetNamedItem("z1").Value;
                }
                catch
                {
                    zString1 = "";
                }

                try
                {
                    zString2 = x.Attributes.GetNamedItem("z2").Value;
                }
                catch
                {
                    zString2 = "";
                }

                int x1, x2, y1, y2, z1, z2;

                if (int.TryParse(xString1, out count))
                {
                    x1 = count < Length ? count : 0;
                }
                else
                {
                    error = true;
                    x1 = 25 < Length ? count : 0;
                }

                if (int.TryParse(xString2, out count))
                {
                    if (count < Length)
                    {
                        x2 = count >= x1 ? count : Length - 1;
                    }
                    else
                    {
                        x2 = Length - 1;
                    }

                }
                else
                {
                    error = true;

                    if (75 < Length)
                    {
                        x2 = 75 >= x1 ? 75 : Length - 1;
                    }
                    else
                    {
                        x2 = Length - 1;
                    }
                }

                if (int.TryParse(yString1, out count))
                {
                    y1 = count < Length ? count : 0;
                }
                else
                {
                    error = true;
                    y1 = 25 < Length ? 25 : 0;
                }

                if (int.TryParse(yString2, out count))
                {
                    if (count < Length)
                    {
                        y2 = count >= y1 ? count : Length - 1;
                    }
                    else
                    {
                        y2 = Length - 1;
                    }

                }
                else
                {
                    error = true;

                    if (75 < Length)
                    {
                        y2 = 75 >= y1 ? 75 : Length - 1;
                    }
                    else
                    {
                        y2 = Length - 1;
                    }
                }

                if (int.TryParse(zString1, out count))
                {
                    z1 = count < Length ? count : 0;
                }
                else
                {
                    error = true;
                    z1 = 25 < Length ? 25 : 0;
                }

                if (int.TryParse(zString2, out count))
                {
                    if (count < Length)
                    {
                        z2 = count >= z1 ? count : Length - 1;
                    }
                    else
                    {
                        z2 = Length - 1;
                    }

                }
                else
                {
                    error = true;

                    if (75 < Length)
                    {
                        z2 = 75 >= z1 ? 75 : Length - 1;
                    }
                    else
                    {
                        z2 = Length - 1;
                    }
                }

                AddPollution(x1, x2, y1, y2, z1, z2);

            }

            return !error;


        } 

        public void ToXML(string pathXML) {

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

            XmlElement element = xmlDocument.CreateElement("name");
            element.InnerText = Name;
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("iterator");
            element.InnerText = Iterator.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("countThread");
            element.InnerText = _countThread.ToString();
            xroot.AppendChild(element);

            string value = "";

            switch (_selectThreadDispatcher) {
                case ThreadDispatcherType.Auto:
                    value = "Auto";
                    break;

                case ThreadDispatcherType.Async:
                    value = "Async";
                    break;

                case ThreadDispatcherType.Sync:
                    value = "Sync";
                    break;
            }

            element = xmlDocument.CreateElement("threadDispatcher");
            element.InnerText = value;
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("maxWind");
            element.InnerText = MaxWind.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("wind");
            element.InnerText = _wind.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("angel");
            element.InnerText = Angel.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("length");
            element.InnerText = Length.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("ray");
            element.InnerText = Ray.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("pAxis");
            element.InnerText = _pAxis.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("pDirection");
            element.InnerText = _pDirection.ToString();
            xroot.AppendChild(element);

            element = xmlDocument.CreateElement("pollutions");
            xroot.AppendChild(element);

            foreach (var x in _pollutions)
            {
                XmlElement ePollution = xmlDocument.CreateElement("pollution");
                ePollution.SetAttribute("x1", x.xStart.ToString());
                ePollution.SetAttribute("x2", x.xEnd.ToString());
                ePollution.SetAttribute("y1", x.yStart.ToString());
                ePollution.SetAttribute("y2", x.yEnd.ToString());
                ePollution.SetAttribute("z1", x.zStart.ToString());
                ePollution.SetAttribute("z2", x.zEnd.ToString());
                element.AppendChild(ePollution);
            }

            xmlDocument.Save(pathXML);

        }

        private void Add(int XStart, int XEnd, int YStart, int YEnd, int ZStart, int ZEnd) {

            for (int x = XStart; x <= XEnd; x++)
            {
                for (int y = YStart; y <= YEnd; y++)
                {
                    for (int z = ZStart; z <= ZEnd; z++)
                    {

                        Auto[x, y, z] = true;

                    }
                }
            }

        }

        public void AddPollution(int XStart, int XEnd, int YStart, int YEnd, int ZStart, int ZEnd) {

            _pollutions.Add(new Pollution() { xStart = XStart, xEnd = XEnd, yStart = YStart, yEnd = YEnd, zStart = ZStart, zEnd = ZEnd });

            Add(XStart, XEnd, YStart, YEnd, ZStart, ZEnd);

        }

        public void Next() {
            Next(_selectThreadDispatcher);
        }

        public void Next(ThreadDispatcherType dispatcherType) {

            switch (dispatcherType){
                case ThreadDispatcherType.Auto:
                    if (Length > 200)
                    {
                        NextAsync();
                    }
                    else {
                        NextSync();
                    }
                    break;

                case ThreadDispatcherType.Async:
                    NextAsync();
                    break;

                case ThreadDispatcherType.Sync:
                    NextSync();
                    break;
            }

        }

        [Serializable]
        private struct NextParam {
           public int StartX;
           public int EndX;
           public int Start;
           public int End;
           public Random randomAxis;
           public Random randomDirection;
        }

        private void NextAsync() {
            NextAsync(true);
            NextAsync(false);
            ResetStep();
            if (Wind > 0) {
                WindStep();
            }
            Iterator++;
        }

        private void NextAsync(bool direction) {

            int length = direction ? _length : _length - 1;

            int max = _countThread;
            Thread[] threads = new Thread[max];

            int step = length / max;

            step = step % 2 == 0 ? step : step - 1;

            int Start = direction ? 0 : 1;
            int End = direction ? 0 : 1;

            for (int i = 0; i < max; i++)
            {
                if (i == max - 1)
                {
                    End = length;
                }
                else
                {
                    End += step;
                }

                threads[i] = new Thread(new ParameterizedThreadStart(Step)) { Name = "Поворот " + i, IsBackground = false };

                threads[i].Start(new NextParam()
                {
                    StartX = Start,
                    EndX = End,
                    Start = direction ? 0 : 1,
                    End = length,
                    randomAxis = new Random(),
                    randomDirection = new Random()

                });

                Start = End;


            }


            foreach (var item in threads)
            {
                item.Join();
            }

        }

        private void Step(object obj) {
            NextParam nextParam = (NextParam)obj;
            Step(nextParam.StartX, nextParam.EndX,nextParam.Start,nextParam.End,nextParam.randomAxis,nextParam.randomDirection);
        }

        private void NextSync() {

            Step(0, _length);
            Step(1, _length - 1);
            ResetStep();
            if (Wind > 0)
            {
                WindStep();
            }
            Iterator++;

        }

        private void Step(int start, int end) {
            if (start == 0)
            {
                Step(0, _length, 0, _length,_randomAxis,_randomDirection);
            }
            else {
                Step(1, _length - 1, 1, _length - 1,_randomAxis,_randomDirection);
            }
        }

        protected void Step(int startX, int endX, int start, int end, Random randomAxis, Random randomDirection) {

            for (int x = startX; x < endX; x += 2)
            {
                for (int y = start; y < end; y += 2)
                {
                    for (int z = start; z < end; z += 2)
                    {
                        double randAxis = randomAxis.NextDouble();
                        double randDirection = randomDirection.NextDouble();


                        bool a, b, c, d, e, f, g, h;


                        //Поворот вдоль Ox
                        if ((randAxis >= 0) && (randAxis < _pAxis))
                        {

                            if ((randDirection >= 0) && (randDirection < _pDirection))
                            {
                                a = Auto[x, y, z + 1];
                                b = Auto[x, y + 1, z + 1];
                                c = Auto[x, y + 1, z];
                                d = Auto[x, y, z];
                                e = Auto[x + 1, y, z + 1];
                                f = Auto[x + 1, y + 1, z + 1];
                                g = Auto[x + 1, y + 1, z];
                                h = Auto[x + 1, y, z];
                            }
                            else if ((randDirection > (1 - _pDirection)) && (randDirection <= 1))
                            {
                                a = Auto[x, y + 1, z];
                                b = Auto[x, y, z];
                                c = Auto[x, y, z + 1];
                                d = Auto[x, y + 1, z + 1];
                                e = Auto[x + 1, y + 1, z];
                                f = Auto[x + 1, y, z];
                                g = Auto[x + 1, y, z + 1];
                                h = Auto[x + 1, y + 1, z + 1];
                            }
                            else
                            {
                                continue;
                            }

                            Auto[x, y, z] = a;
                            Auto[x, y, z + 1] = b;
                            Auto[x, y + 1, z + 1] = c;
                            Auto[x, y + 1, z] = d;
                            Auto[x + 1, y, z] = e;
                            Auto[x + 1, y, z + 1] = f;
                            Auto[x + 1, y + 1, z + 1] = g;
                            Auto[x + 1, y + 1, z] = h;
                        }
                        //Oy
                        else if ((randAxis > (1 - _pAxis)) && (randAxis <= 1))
                        {
                            if ((randDirection >= 0) && (randDirection < _pDirection))
                            {
                                a = Auto[x, y, z + 1];
                                b = Auto[x + 1, y, z + 1];
                                c = Auto[x + 1, y, z];
                                d = Auto[x, y, z];
                                e = Auto[x, y + 1, z + 1];
                                f = Auto[x + 1, y + 1, z + 1];
                                g = Auto[x + 1, y + 1, z];
                                h = Auto[x, y + 1, z];
                            }
                            else if ((randDirection > (1 - _pDirection)) && (randDirection <= 1))
                            {
                                a = Auto[x + 1, y, z];
                                b = Auto[x, y, z];
                                c = Auto[x, y, z + 1];
                                d = Auto[x + 1, y, z + 1];
                                e = Auto[x + 1, y + 1, z];
                                f = Auto[x, y + 1, z];
                                g = Auto[x, y + 1, z + 1];
                                h = Auto[x + 1, y + 1, z + 1];
                            }
                            else
                            {
                                continue;
                            }

                            Auto[x, y, z] = a;
                            Auto[x, y, z + 1] = b;
                            Auto[x + 1, y, z + 1] = c;
                            Auto[x + 1, y, z] = d;
                            Auto[x, y + 1, z] = e;
                            Auto[x, y + 1, z + 1] = f;
                            Auto[x + 1, y + 1, z + 1] = g;
                            Auto[x + 1, y + 1, z] = h;

                        }
                        //Oz
                        else
                        {
                            if ((randDirection >= 0) && (randDirection < _pDirection))
                            {
                                a = Auto[x, y + 1, z];
                                b = Auto[x + 1, y + 1, z];
                                c = Auto[x + 1, y + 1, z];
                                d = Auto[x, y, z];
                                e = Auto[x, y + 1, z + 1];
                                f = Auto[x + 1, y + 1, z + 1];
                                g = Auto[x + 1, y + 1, z + 1];
                                h = Auto[x, y, z + 1];
                            }
                            else if ((randDirection > (1 - _pDirection)) && (randDirection <= 1))
                            {
                                a = Auto[x + 1, y, z];
                                b = Auto[x, y, z];
                                c = Auto[x, y + 1, z];
                                d = Auto[x + 1, y + 1, z];
                                e = Auto[x + 1, y, z + 1];
                                f = Auto[x, y, z + 1];
                                g = Auto[x, y + 1, z + 1];
                                h = Auto[x + 1, y + 1, z + 1];
                            }
                            else
                            {
                                continue;
                            }

                            Auto[x, y, z] = a;
                            Auto[x, y + 1, z] = b;
                            Auto[x + 1, y + 1, z] = c;
                            Auto[x + 1, y, z] = d;
                            Auto[x, y, z + 1] = e;
                            Auto[x, y + 1, z + 1] = f;
                            Auto[x + 1, y + 1, z + 1] = g;
                            Auto[x + 1, y, z + 1] = h;
                        }

                    }
                }
            }
        }

        private void ResetStep() {

            //Обнуляем
            for (int x = 0; x < Length; x++)
            {
                //По Z
                for (int y = 0; y < Length; y++)
                {
                    Auto[x, y, 0] = false;
                    Auto[x, y, Length - 1] = false;
                }

                //По Y
                for (int z = 0; z < Length; z++)
                {
                    Auto[x, 0, z] = false;
                    Auto[x, Length - 1, z] = false;
                }

            }

            //По X
            for (int y = 0; y < Length; y++)
            {
                for (int z = 0; z < Length; z++)
                {
                    Auto[0, y, z] = false;
                    Auto[Length - 1, y, z] = false;
                }
            }

        }

        protected void WindStep() {
            for (int x = 0; x < _length; x++)
            {
                for (int y = 0; y < _length; y++)
                {
                    for (int z = 0; z < _length; z++)
                    {
                        int i = Math.Cos(Angel) > 0 ? _length - 1 - x : x;
                        int j = Math.Sin(Angel) > 0 ? _length - 1 - z : z;

                        if (Auto[i, y, j])
                        {

                            double rand = _randomWind.NextDouble();
                            if (rand <= (_wind / MaxWind))
                            {

                                WindStep(i, j, y, (int)_wind);

                            }
                        }
                    }
                }
            }
        }

        private void WindStep(int i, int j, int y, int wind) {
            WindStep(i, j, y, wind, out int inew, out int jnew);
        }

        private void WindStep(int i, int j, int y, int wind,out int inew,out int jnew) {
            inew = i;
            jnew = j;
            Auto[inew, y, jnew] = false;
            bool stop = false;

            for (int k = 1; k < wind; k++)
            {
                int i1 = (int)Math.Round(inew + Math.Cos(Angel * 3.14 / 180));
                int j1 = (int)Math.Round(jnew + Math.Sin(Angel * 3.14 / 180));

                

                if (i1 <= 0 || i1 >= _length - 1 || j1 <= 0 || j1 >= _length - 1) {
                    inew = i1;
                    jnew = j1;
                    stop = true;
                    break;
                }

                if (Auto[i1, y, j1])
                {
                   
                    int newWind = (int)((double)(wind - k) / 2 + 0.5);
                    if (newWind > 0)
                    {
                        WindStep(i1, j1, y, newWind, out int inew2, out int jnew2);
                        if ((i1 == inew2 )&& (j1 == jnew2))
                        {
                            Auto[inew, y, jnew] = true;
                            stop = true;
                            break;
                        }
                        else {
                            inew = i1;
                            jnew = j1;
                            Auto[inew, y, jnew] = true;
                            stop = true;
                            break;
                        }
                    }
                    else
                    {
                        Auto[inew, y, jnew] = true;
                        stop = true;
                        break;
                    }
                    
                }
                else {
                    inew = i1;
                    jnew = j1;
                }
            }

            if (!stop) {
                Auto[inew, y, jnew] = true;
            }
        }

        public void Remove() {
            Length = _length;
        }

        public double[,] GetPollution(int index, Axis axis) {
            return GetPollution(index, axis, _selectThreadDispatcher);
        }

        public double[,] GetPollution(int index, Axis axis,ThreadDispatcherType dispatcherType) {
            double[,] res;
            switch (dispatcherType) {
                default:
                case ThreadDispatcherType.Auto:
                    if ((Length > 150) || (Ray > 14))
                    {
                        res = GetAsyncPollution(index, axis);
                    }
                    else {
                        res = GetSyncPollution(index, axis);
                    }
                    break;

                case ThreadDispatcherType.Async:
                    res = GetAsyncPollution(index, axis);
                    break;

                case ThreadDispatcherType.Sync:
                    res = GetSyncPollution(index, axis);
                    break;
            }
            return res;
        }

        private double[,] GetAsyncPollution(int index, Axis axis) {
            double[,] result = new double[_length, _length];

            int max = _countThread;
            Thread[] threads = new Thread[max];

            int step = _length / max;

            int Start = 0;
            int End = 0;

            for (int i = 0; i<max; i++)
            {
                if (i == max - 1)
                {
                    End = _length;
                }
                else {
                    End += step;
                }

                threads[i] = new Thread(new ParameterizedThreadStart(GetPollution)) { Name = "Осриднение " + i, IsBackground = false };

                threads[i].Start(new PollutionRayParam() {
                    axis = axis,
                    index = index,
                    res = result,
                    Start = Start,
                    End = End
                });

                Start = End;
                

            }


            foreach (var item in threads)
            {
                item.Join();
            }


            return result;

        }

        private void GetPollution(object obj) {
            PollutionRayParam param = (PollutionRayParam)(obj);
            GetPollution(param.index, param.axis, param.Start, param.End, param.res);
        }

        private double[,] GetSyncPollution(int index, Axis axis) {

            double[,] result = new double[_length, _length];

            GetPollution(index, axis, 0, _length, result);

            return result;

        }

        protected void GetPollution(int index, Axis axis, int Start, int End, double[,] result)
        {

            switch (axis)
            {
                case Axis.Ox:

                    int xStart = (index - Ray >= 0) ? (index - Ray) : 0;
                    int xEnd = (index + Ray < _length) ? (index + Ray) : _length - 1;

                    for (int y = Start; y < End; y++)
                    {
                        for (int z = 0; z < _length; z++)
                        {
                            int sum = 0;


                            for (int xIndex = xStart; xIndex <= xEnd; xIndex++)
                            {

                                int yStart = (y - Ray >= 0) ? (y - Ray) : 0;
                                int yEnd = (y + Ray < _length) ? (y + Ray) : _length - 1;

                                for (; yStart <= yEnd; yStart++)
                                {

                                    int zStart = (z - Ray >= 0) ? (z - Ray) : 0;
                                    int zEnd = (z + Ray < _length) ? (z + Ray) : _length - 1;

                                    for (; zStart <= zEnd; zStart++)
                                    {

                                        if (Auto[xIndex, yStart, zStart]) { sum++; }

                                    }

                                }

                            }

                            result[y, z] = sum / Math.Pow((2 * Ray + 1), 3);

                        }
                    }


                    break;

                case Axis.Oy:

                    int yStart1 = (index - Ray >= 0) ? (index - Ray) : 0;
                    int yEnd1 = (index + Ray < _length) ? (index + Ray) : _length - 1;

                    for (int x = Start; x < End; x++)
                    {
                        for (int z = 0; z < _length; z++)
                        {
                            int sum = 0;


                            for (int yIndex = yStart1; yIndex <= yEnd1; yIndex++)
                            {

                                int xStart1 = (x - Ray >= 0) ? (x - Ray) : 0;
                                int xEnd1 = (x + Ray < _length) ? (x + Ray) : _length - 1;

                                for (; xStart1 <= xEnd1; xStart1++)
                                {

                                    int zStart1 = (z - Ray >= 0) ? (z - Ray) : 0;
                                    int zEnd1 = (z + Ray < _length) ? (z + Ray) : _length - 1;

                                    for (; zStart1 <= zEnd1; zStart1++)
                                    {

                                        if (Auto[xStart1, yIndex, zStart1]) { sum++; }

                                    }

                                }

                            }

                            result[x, z] = sum / Math.Pow((2 * Ray + 1), 3);

                        }
                    }


                    break;

                case Axis.Oz:

                    int zStart2 = (index - Ray >= 0) ? (index - Ray) : 0;
                    int zEnd2 = (index + Ray < _length) ? (index + Ray) : _length - 1;

                    for (int x = Start; x < End; x++)
                    {
                        for (int y = 0; y < _length; y++)
                        {
                            int sum = 0;


                            for (int zIndex = zStart2; zIndex <= zEnd2; zIndex++)
                            {

                                int xStart2 = (x - Ray >= 0) ? (x - Ray) : 0;
                                int xEnd2 = (x + Ray < _length) ? (x + Ray) : _length - 1;

                                for (; xStart2 <= xEnd2; xStart2++)
                                {

                                    int yStart2 = (y - Ray >= 0) ? (y - Ray) : 0;
                                    int yEnd2 = (y + Ray < _length) ? (y + Ray) : _length - 1;

                                    for (; yStart2 <= yEnd2; yStart2++)
                                    {

                                        if (Auto[xStart2, yStart2, zIndex]) { sum++; }

                                    }

                                }

                            }

                            result[x, y] = sum / Math.Pow((2 * Ray + 1), 3);

                        }
                    }


                    break;


            }

        }

        public bool GetPointPollution(Axis axis, int selectIndex, int index1, int index2) {

            switch (axis) {

                default:
                case Axis.Ox:
                    return Auto[selectIndex, index1, index2];
                case Axis.Oy:
                    return Auto[index1, selectIndex, index2];
                case Axis.Oz:
                    return Auto[index1, index2, selectIndex];

            }

        }

        public void Reset() {
            Auto = new bool[_length, _length, _length];
            Iterator = 0;
            foreach (var pollution in _pollutions)
            {
                Add(pollution.xStart, pollution.xEnd, pollution.yStart, pollution.yEnd, pollution.zStart, pollution.zEnd);
            }
        }

        [Serializable]
        private struct PollutionRayParam{
            public int index;
            public Axis axis;
            public int Start;
            public int End;
            public double[,] res;
        }

        [Serializable]
        private struct Pollution {
            public int xStart;
            public int xEnd;
            public int yStart;
            public int yEnd;
            public int zStart;
            public int zEnd;
        }
    }
}
