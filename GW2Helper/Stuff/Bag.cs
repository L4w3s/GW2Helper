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
        public int Size { get; set; }
        public List<ItemStack> Contents { get; set; }
    }
    public class ItemStack
    {
        public enum Binding
        {
            Character,
            Account
        }
        public Item Item { get; set; }
        public int Count { get; set; }
        public List<Item> Infusions { get; set; }
        public List<Item> Upgrades { get; set; }
        public Skin Skin { get; set; }
        public ItemStackStat Stats { get; set; }
        public Binding ItemBinding { get; set; }
        public string BoundTo { get; set; }
    }
    public class ItemStackStat
    {
        public ItemStat Stat { get; set; }
        public ItemStackAttribute Attributes { get; set; }
    }
    public class ItemStackAttribute
    {
        public int? Power { get; set; }
        public int? Precision { get; set; }
        public int? Toughness { get; set; }
        public int? Vitality { get; set; }
        public int? ConditionDamage { get; set; }
        public int? ConditionDuration { get; set; }
        public int? Healing { get; set; }
        public int? BoonDuration { get; set; }
    }
}
