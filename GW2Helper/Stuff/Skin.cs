using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public enum Flag
        {
            ShowInWardrobe,
            NoCost,
            HideIfLocked,
            OverrideRarity
        }
        public enum Restriction
        {
            Asura,
            Charr,
            Human,
            Norn,
            Sylvari
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public Type SkinType { get; set; }
        public List<Flag> Flags { get; set; }
        public List<Restriction> Restrictions { get; set; }
        public string Image { get; set; }
        public string Rarity { get; set; }
        public string Description { get; set; }
        public Detail Details { get; set; }

        public static Skin GetSkinFromJSON(string json, Main main)
        {
            SkinRAW skinRAW = JsonConvert.DeserializeObject<SkinRAW>(json);
            Skin newSkin = new Skin
            {
                ID = skinRAW.id,
                Name = skinRAW.name,
                SkinType = (Type)Enum.Parse(typeof(Type), skinRAW.type),
                Flags = new List<Flag>(),
                Restrictions = new List<Restriction>(),
                Rarity = skinRAW.rarity,
                Description = skinRAW.description
            };
            for (int i = 0; i < skinRAW.flags.Length; i++)
            {
                newSkin.Flags.Add((Flag)Enum.Parse(typeof(Flag), skinRAW.flags[i]));
            }
            for (int i = 0; i < skinRAW.restrictions.Length; i++)
            {
                newSkin.Restrictions.Add((Restriction)Enum.Parse(typeof(Restriction), skinRAW.restrictions[i]));
            }

            if (skinRAW.type == "Armor" && skinRAW.details != null)
            {
                SkinSub1RAW skinSub1RAW = skinRAW.details;
                ArmorDetail newDetail = new ArmorDetail
                {   
                    ArmorType = (ArmorDetail.Type)Enum.Parse(typeof(ArmorDetail.Type), skinSub1RAW.type),
                    Weight = (ArmorDetail.WeightClass)Enum.Parse(typeof(ArmorDetail.WeightClass), skinSub1RAW.weight_class)
                };

                SkinSub2RAW skinSub2RAW = skinSub1RAW.dye_slots;
                DyeSlots dyeSlots = new DyeSlots();
                for (int i = 0; i < skinSub2RAW.def.Length; i++)
                {
                    SkinSub3RAW skinSub3RAW = skinSub2RAW.def[i];
                    int colourID = skinSub3RAW.color_id;
                    Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                    Dye newDye = new Dye
                    {
                        Color = colour,
                        DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                    };
                    dyeSlots.Dyes.Add(newDye);
                }

                if (skinSub2RAW.overrides != null)
                {
                    DyeOverride dyeOverride = new DyeOverride();
                    SkinSub4RAW skinSub4RAW = skinSub2RAW.overrides;
                    if (skinSub4RAW.AsuraMale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.AsuraMale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.AsuraMale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.AsuraMale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.AsuraFemale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.AsuraFemale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.AsuraFemale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.AsuraFemale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.CharrMale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.CharrMale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.CharrMale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.CharrMale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.CharrFemale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.CharrFemale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.CharrFemale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.CharrFemale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.HumanMale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.HumanMale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.HumanMale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.HumanMale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.HumanFemale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.HumanFemale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.HumanFemale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.HumanFemale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.NornMale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.NornMale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.NornMale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.NornMale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.NornFemale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.NornFemale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.NornFemale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.NornFemale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.SylvariMale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.SylvariMale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.SylvariMale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.SylvariMale.Add(newDye);
                        }
                    }
                    if (skinSub4RAW.SylvariFemale != null)
                    {
                        for (int i = 0; i < skinSub4RAW.SylvariFemale.Length; i++)
                        {
                            SkinSub3RAW skinSub3RAW = skinSub4RAW.SylvariFemale[i];
                            int colourID = skinSub3RAW.color_id;
                            Colour colour = main.Colours.FirstOrDefault(col => col.ID == colourID);
                            Dye newDye = new Dye
                            {
                                Color = colour,
                                DyeMaterial = (Dye.Material)Enum.Parse(typeof(Dye.Material), skinSub3RAW.material)
                            };
                            dyeOverride.SylvariFemale.Add(newDye);
                        }
                    }

                    dyeSlots.Overrides = dyeOverride;
                }
                newDetail.Dyes = dyeSlots;

                newSkin.Details = newDetail;
            }
            else if (skinRAW.type == "Weapon" && skinRAW.details != null)
            {
                SkinSub1RAW skinSub1RAW = skinRAW.details;
                WeaponDetail newDetail = new WeaponDetail
                {
                    WeaponType = (WeaponDetail.Type)Enum.Parse(typeof(WeaponDetail.Type), skinSub1RAW.type),
                    Damage = (WeaponDetail.DamageType)Enum.Parse(typeof(WeaponDetail.DamageType), skinSub1RAW.damage_type)
                };
                newSkin.Details = newDetail;
            }
            else if (skinRAW.type == "Gathering" && skinRAW.details != null)
            {
                SkinSub1RAW skinSub1RAW = skinRAW.details;
                GatheringDetail newDetail = new GatheringDetail
                {
                    GatheringType = (GatheringDetail.Type)Enum.Parse(typeof(GatheringDetail.Type), skinSub1RAW.type)
                };
                newSkin.Details = newDetail;
            }
            string fileName = string.Empty;
            using (WebClient client = new WebClient())
            {
                fileName = skinRAW.icon.Substring(skinRAW.icon.LastIndexOf("/") + 1);
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skins\" + fileName)) client.DownloadFileAsync(new Uri(skinRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skins\" + fileName);
            }
            newSkin.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skins\" + fileName;

            return newSkin;
        }
    }

    public abstract class Detail
    {
    }
    public class ArmorDetail : Detail
    {
        public enum Type
        {
            Boots,
            Coat,
            Gloves,
            Helm,
            HelmAquatic,
            Leggings,
            Shoulders
        }
        public enum WeightClass
        {
            Clothing,
            Light,
            Medium,
            Heavy
        }
        public Type ArmorType { get; set; }
        public WeightClass Weight { get; set; }
        public DyeSlots Dyes { get; set; }
    }
    public class WeaponDetail : Detail
    {
        public enum Type
        {
            Axe,
            Dagger,
            Mace,
            Pistol,
            Scepter,
            Sword,
            Focus,
            Shield,
            Torch,
            Warhorn,
            Greatsword,
            Hammer,
            LongBow,
            Rifle,
            ShortBow,
            Staff,
            Harpoon,
            Speargun,
            Trident,
            LargeBundle,
            SmallBundle,
            Toy,
            TwoHandedToy
        }
        public enum DamageType
        {
            Physical,
            Fire,
            Lightning,
            Ice,
            Choking
        }
        public Type WeaponType { get; set; }
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
        public Type GatheringType { get; set; }
    }

    public class DyeSlots
    {
        public List<Dye> Dyes { get; set; }
        public DyeOverride Overrides { get; set; }
    }
    public class DyeOverride
    {
        public List<Dye> AsuraMale { get; set; }
        public List<Dye> AsuraFemale { get; set; }
        public List<Dye> CharrMale { get; set; }
        public List<Dye> CharrFemale { get; set; }
        public List<Dye> HumanMale { get; set; }
        public List<Dye> HumanFemale { get; set; }
        public List<Dye> NornMale { get; set; }
        public List<Dye> NornFemale { get; set; }
        public List<Dye> SylvariMale { get; set; }
        public List<Dye> SylvariFemale { get; set; }

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

    class SkinRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string[] flags { get; set; }
        public string[] restrictions { get; set; }
        public string icon { get; set; }
        public string rarity { get; set; }
        public string description { get; set; }
        public SkinSub1RAW details { get; set; }
    }
    public class SkinSub1RAW
    {
        public string type { get; set; }
        public string weight_class { get; set; }
        public string damage_type { get; set; }
        public SkinSub2RAW dye_slots { get; set; }
    }
    public class SkinSub2RAW
    {
        public SkinSub4RAW overrides { get; set; }
        public SkinSub3RAW[] def { get; set; }
    }
    public class SkinSub3RAW
    {
        public int color_id { get; set; }
        public string material { get; set; }
    }
    public class SkinSub4RAW
    {
        public SkinSub3RAW[] AsuraMale { get; set; }
        public SkinSub3RAW[] AsuraFemale { get; set; }
        public SkinSub3RAW[] CharrMale { get; set; }
        public SkinSub3RAW[] CharrFemale { get; set; }
        public SkinSub3RAW[] HumanMale { get; set; }
        public SkinSub3RAW[] HumanFemale { get; set; }
        public SkinSub3RAW[] NornMale { get; set; }
        public SkinSub3RAW[] NornFemale { get; set; }
        public SkinSub3RAW[] SylvariMale { get; set; }
        public SkinSub3RAW[] SylvariFemale { get; set; }
    }
}
