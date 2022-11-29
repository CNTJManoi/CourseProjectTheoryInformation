using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSender.Models
{
    public class HammingRow
    {
        public HammingRow(int one, int two, int three, int four, int five, int six, int seven)
        {
            One = one;
            Two = two;
            Three = three;
            Four = four;
            Five = five;
            Six = six;
            Seven = seven;
        }

        public int One { get; set; }
        public int Two { get; set; }
        public int Three { get; set; }
        public int Four { get; set; }
        public int Five { get; set; }
        public int Six { get; set; }
        public int Seven { get; set; }
    }
}
