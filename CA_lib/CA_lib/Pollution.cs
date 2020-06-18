using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_lib
{
    [Serializable]
    class Pollution : Rectangle
    {
        public int Frequency { get; set; }
        public bool StartPollutin { get; set; }
    }
}
