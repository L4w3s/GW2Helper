using GW2Helper.Stuff.ItemStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.CharacterStuff.Equipment
{
    public class Equipment
    {
        public enum Slot
        {
            HelmAquatic,
            Backpack,
            Coat,
            Boots,
            Gloves,
            Helm,
            Leggings,
            Shoulders,
            Accessory1,
            Accessory2,
            Ring1,
            Ring2,
            Amulet,
            WeaponAquaticA,
            WeaponAquaticB,
            WeaponA1,
            WeaponA2,
            WeaponB1,
            WeaponB2,
            Sickle,
            Axe,
            Pick
        }
        public enum Binding
        {
            Character,
            Account
        }
        public int ID { get; set; }
        public Slot EquipSlot { get; set; }
        public List<Item> Infusions { get; set; }
        public List<Item> Upgrades { get; set; }
        public Skin ItemSkin { get; set; }
        public ItemStat Stat { get; set; }
        public Binding ItemBind { get; set; }
        public int Charges { get; set; }
        public string CharacterBound { get; set; }
        public List<Dye> Dyes { get; set; }
    }
}
