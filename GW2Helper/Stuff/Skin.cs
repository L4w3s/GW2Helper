using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Skin
    {
        public enum Type
        {
            Armor,
            Weapon,
            Back,
            Gathering
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public Type SkinType { get; set; }
        public List<string> Flags { get; set; }
        public List<string> Restrictions { get; set; }
        public string Image { get; set; }
        public string Rarity { get; set; }
        public string Description { get; set; }
        public Detail Details { get; set; }
    }

    public abstract class Detail
    {
    }
    public class ArmorDetail : Detail
    {
        public enum WeightClass
        {
            Clothing,
            Light,
            Medium,
            Heavy
        }
        public string Type { get; set; }
        public WeightClass Weight { get; set; }
        public DyeSlots Dyes { get; set; }
    }
    public class WeaponDetail : Detail
    {
        public enum DamageType
        {
            Physical,
            Fire,
            Lightning,
            Ice,
            Choking
        }
        public string Type { get; set; }
        public DamageType Damage { get; set; }
    }
    public class GatheringDetail : Detail
    {
        public enum Type
        {
            Foraging,
            Logging,
            Mining
        }
        public Type DetailType { get; set; }
    }

    public class DyeSlots
    {
        public List<Dye> Dyes { get; set; }
        public List<Dye> Overrides { get; set; }
    }
    public class Dye
    {
        public enum Material
        {
            Cloth,
            Leather,
            Metal
        }
        public Colour Color { get; set; }
        public Material DyeMaterial { get; set; }
    }
}
