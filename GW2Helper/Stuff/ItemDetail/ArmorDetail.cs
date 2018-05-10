using GW2Helper.Stuff.ItemStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.ItemDetail
{
    public class ArmorDetail : ItemDetail
    {
        public string Type { get; set; }
        public string Weight { get; set; }
        public int Defense { get; set; }
        public List<InfusionSubObject> Infusions { get; set; }
        public InfixUpgradeSubObject InfixUpgrades { get; set; }
        public List<string> StatChoices { get; set; }
    }
}
