using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_lib
{
    [Serializable]
    class Rectangle
    {
        public int XStart { get; set; }
        public int XEnd { get; set; }
        public int YStart { get; set; }
        public int YEnd { get; set; }
        public int ZStart { get; set; }
        public int ZEnd { get; set; }
    }
}
