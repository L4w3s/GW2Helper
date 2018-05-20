using Newtonsoft.Json;
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
        public ColourCategory Categories { get; set; }
        
        public static void GetColoursFromJSON(string json, Main main)
        {
            ColourRAW[] rawColours = new ColourRAW[1];
            try
            {
                rawColours = JsonConvert.DeserializeObject<ColourRAW[]>(json);
            }
            catch (Exception e)
            {
                rawColours[0] = JsonConvert.DeserializeObject<ColourRAW>(json);
            }
            for (int c = 0; c < rawColours.Length; c++)
            {
                double cur = c, max = rawColours.Length;
                ColourRAW colourRAW = rawColours[c];
                main.JSON.Add(new KeyValuePair<string, string>("Colour", JsonConvert.SerializeObject(colourRAW)));
                Colour newColour = new Colour
                {
                    ID = colourRAW.id,
                    Name = colourRAW.name,
                    RGB = colourRAW.base_rgb.ToList(),
                };
                ColourSubRAW colourSubRAW1 = colourRAW.cloth;
                AppearanceDetail clothDetail = new AppearanceDetail
                {
                    Brightness = colourSubRAW1.brightness,
                    Contrast = colourSubRAW1.contrast,
                    Hue = colourSubRAW1.hue,
                    Saturation = colourSubRAW1.saturation,
                    Lightness = colourSubRAW1.lightness,
                    RGB = colourSubRAW1.rgb.ToList()
                };
                newColour.ClothDetail = clothDetail;

                ColourSubRAW colourSubRAW2 = colourRAW.leather;
                AppearanceDetail leatherDetail = new AppearanceDetail
                {
                    Brightness = colourSubRAW2.brightness,
                    Contrast = colourSubRAW2.contrast,
                    Hue = colourSubRAW2.hue,
                    Saturation = colourSubRAW2.saturation,
                    Lightness = colourSubRAW2.lightness,
                    RGB = colourSubRAW2.rgb.ToList()
                };
                newColour.LeatherDetail = leatherDetail;

                ColourSubRAW colourSubRAW3 = colourRAW.metal;
                AppearanceDetail metalDetail = new AppearanceDetail
                {
                    Brightness = colourSubRAW3.brightness,
                    Contrast = colourSubRAW3.contrast,
                    Hue = colourSubRAW3.hue,
                    Saturation = colourSubRAW3.saturation,
                    Lightness = colourSubRAW3.lightness,
                    RGB = colourSubRAW3.rgb.ToList()
                };
                newColour.MetalDetail = metalDetail;

                if (colourRAW.categories.Length > 0)
                {
                    ColourCategory colourCategory = new ColourCategory();
                    if (colourRAW.categories.Length > 0) colourCategory.CatHue = (ColourCategory.Hue)Enum.Parse(typeof(ColourCategory.Hue), colourRAW.categories[0]);
                    if (colourRAW.categories.Length > 1) colourCategory.CatMaterial = (ColourCategory.Material)Enum.Parse(typeof(ColourCategory.Material), colourRAW.categories[1]);
                    if (colourRAW.categories.Length > 2) colourCategory.CatRarity = (ColourCategory.Rarity)Enum.Parse(typeof(ColourCategory.Rarity), colourRAW.categories[2]);
                    newColour.Categories = colourCategory;
                }

                main.Colours.Add(newColour);
                main.OnCharStatusUpdate("Generated Colour " + newColour.Name + ";" + newColour.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }

    public class AppearanceDetail
    {
        public int Brightness { get; set; }
        public double Contrast { get; set; }
        public int Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }
        public List<int> RGB { get; set; }
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

    class ColourRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public int[] base_rgb { get; set; }
        public ColourSubRAW cloth { get; set; }
        public ColourSubRAW leather { get; set; }
        public ColourSubRAW metal { get; set; }
        public int item { get; set; }
        public string[] categories { get; set; }
    }
    class ColourSubRAW
    {
        public int brightness { get; set; }
        public double contrast { get; set; }
        public int hue { get; set; }
        public double saturation { get; set; }
        public double lightness { get; set; }
        public int[] rgb { get; set; }
    }
}
