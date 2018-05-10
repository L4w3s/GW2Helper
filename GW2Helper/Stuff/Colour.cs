using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Colour
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<int> RGB { get; set; }
        public AppearanceDetail ClothDetail { get; set; }
        public AppearanceDetail LeatherDetail { get; set; }
        public AppearanceDetail MetalDetail { get; set; }
        public Item DyeItem { get; set; }
        public List<ColourCategory> Categories { get; set; }
    }

    public class AppearanceDetail
    {
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Lightness { get; set; }
        public List<List<int>> RGBList { get; set; }
    }

    public class ColourCategory
    {
        public enum Hue
        {
            Gray,
            Brown,
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple
        }
        public enum Material
        {
            Vibrant,
            Leather,
            Metal
        }
        public enum Rarity
        {
            Starter,
            Common,
            Uncommon,
            Rare
        }
        public Hue CatHue { get; set; }
        public Material CatMaterial { get; set; }
        public Rarity CatRarity { get; set; }
    }
}
