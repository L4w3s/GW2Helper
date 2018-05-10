using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public int Level { get; set; }
        public int VendorValue { get; set; }
        public List<string> Flags { get; set; }
        public List<string> GameTypes { get; set; }
        public List<string> Restrictions { get; set; }
        public ItemDetail.ItemDetail Details { get; set; }
    }
}
