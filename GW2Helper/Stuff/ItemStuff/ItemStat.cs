using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.ItemStuff
{
    public class ItemStat
    {
        public enum Attribute
        {
            AgonyResistance,
            BoonDuration,
            ConditionDamage,
            ConditionDuration,
            CritDamage,
            Healing,
            Power,
            Precision,
            Toughness,
            Vitality
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public KeyValuePair<Attribute, double> Attributes { get; set; }
    }
}
