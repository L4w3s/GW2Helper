using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Bag
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public int Size { get; set; }
        public List<Item> Items { get; set; }
    }
}
