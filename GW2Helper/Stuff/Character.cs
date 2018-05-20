using GW2Helper.Stuff.CharacterStuff;
using GW2Helper.Stuff.CharacterStuff.Equipment;
using GW2Helper.Stuff.CharacterStuff.Skills;
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
    public class Character
    {
        public enum Race
        {
            Asura,
            Charr,
            Human,
            Norn,
            Sylvari
        }
        public enum Gender
        {
            Male,
            Female
        }
        public enum Profession
        {
            Elementalist,
            Engineer,
            Guardian,
            Mesmer,
            Necromancer,
            Ranger,
            Revenant,
            Thief,
            Warrior
        }
        public List<BackstoryAnswer> Backstory { get; set; }
        public string Name { get; set; }
        public Race CharRace { get; set; }
        public Gender CharGender { get; set; }
        public List<string> Flags { get; set; }
        public Profession CharProfession { get; set; }
        public int Level { get; set; }
        public Guild GuildInfo { get; set; }
        public int Age { get; set; }
        public DateTime CreationDate { get; set; }
        public int Deaths { get; set; }
        public Title CharTitle { get; set; }
        public List<CraftingSkill> CraftingSkills { get; set; }
        public EquipmentContainer Equips { get; set; }
        public List<string> Heropoints { get; set; }
        public Inventory Inv { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Specialization> Specializations { get; set; }
        public List<string> WVWAbilities { get; set; }
        public List<Equipment> PVPEquips { get; set; }
        public List<Recipe> Recipes { get; set; }
        public List<Training> TrainingStats { get; set; }

        public static Character GetCharacterFromJSON(string json, Main main)
        {
            CharacterRAW charRAW = JsonConvert.DeserializeObject<CharacterRAW>(json);
            Character newChar = new Character
            {
                Backstory = new List<BackstoryAnswer>(),
                Name = charRAW.name,
                CharRace = (Race)Enum.Parse(typeof(Race), charRAW.race),
                CharGender = (Gender)Enum.Parse(typeof(Gender), charRAW.gender),
                Flags = charRAW.flags.ToList(),
                CharProfession = (Profession)Enum.Parse(typeof(Profession), charRAW.profession),
                Level = charRAW.level,
                Age = charRAW.age,
                CreationDate = DateTime.Parse(charRAW.created),
                Deaths = charRAW.deaths,
                Inv = new Inventory()
            };

            for (int i = 0; i < charRAW.backstory.Length; i++)
            {
                string id = charRAW.backstory[i];
                BackstoryAnswer answer = main.BackstoryAnswers.FirstOrDefault(ans => ans.ID == id);
                newChar.Backstory.Add(answer);
            }
            newChar.Backstory = newChar.Backstory.OrderBy(bs => bs.Question.OrderPosition).ToList();

            if (!string.IsNullOrEmpty(charRAW.guild))
            {
                string guildID = charRAW.guild;

                WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/guild/" + guildID);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                string html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                Guild guild = Guild.GetGuildFromJSON(html, main);
                newChar.GuildInfo = guild;
            }
            if (charRAW.title.HasValue)
            {
                int titleID = charRAW.title.Value;
                Title title = main.Titles.FirstOrDefault(ti => ti.ID == titleID);
                newChar.CharTitle = title;
            }
            if (charRAW.bags != null)
            {
                newChar.Inv.Bags = new List<Bag>();

                for (int i = 0; i < charRAW.bags.Length; i++)
                {
                    if (charRAW.bags[i] != null)
                    {
                        Bag bag = new Bag
                        {
                            ID = charRAW.bags[i].id,
                            Size = charRAW.bags[i].size,
                            Contents = new List<ItemStack>()
                        };
                        if (charRAW.bags[i].inventory != null)
                        {
                            for (int j = 0; j < charRAW.bags[i].inventory.Length; j++)
                            {
                                if (charRAW.bags[i].inventory[j] != null)
                                {
                                    ItemStack itemStack = new ItemStack
                                    {
                                        Count = charRAW.bags[i].inventory[j].count,
                                        Infusions = new List<Item>(),
                                        Upgrades = new List<Item>()
                                    };
                                    if (charRAW.bags[i].inventory[j].binding != null)
                                    {
                                        itemStack.ItemBinding = (ItemStack.Binding)Enum.Parse(typeof(ItemStack.Binding), charRAW.bags[i].inventory[j].binding);
                                        itemStack.BoundTo = charRAW.bags[i].inventory[j].bound_to;
                                    }

                                    int itemID = charRAW.bags[i].inventory[j].id;
                                    Item newItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                                    itemStack.Item = newItem;

                                    if (charRAW.bags[i].inventory[j].skin.HasValue)
                                    {
                                        int skinID = charRAW.bags[i].inventory[j].skin.Value;
                                        Skin newSkin = main.Skins.FirstOrDefault(sk => sk.ID == skinID);
                                        itemStack.Skin = newSkin;
                                    }

                                    if (charRAW.bags[i].inventory[j].infusions != null)
                                    {
                                        for (int k = 0; k < charRAW.bags[i].inventory[j].infusions.Length; k++)
                                        {
                                            itemID = charRAW.bags[i].inventory[j].infusions[k].Value;
                                            Item infusionItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                                            itemStack.Infusions.Add(infusionItem);
                                        }
                                    }
                                    if (charRAW.bags[i].inventory[j].upgrades != null)
                                    {
                                        for (int k = 0; k < charRAW.bags[i].inventory[j].upgrades.Length; k++)
                                        {
                                            itemID = charRAW.bags[i].inventory[j].upgrades[k].Value;
                                            Item upgradeItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                                            itemStack.Upgrades.Add(upgradeItem);
                                        }
                                    }

                                    if (charRAW.bags[i].inventory[j].stats != null)
                                    {
                                        ItemStat itemStat = main.ItemStats.FirstOrDefault(it => it.ID == charRAW.bags[i].inventory[j].stats.id);
                                        ItemStackAttribute itemStackAttribute = new ItemStackAttribute
                                        {
                                            Power = (charRAW.bags[i].inventory[j].stats.attributes.Power.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.Power.Value : 0,
                                            Precision = (charRAW.bags[i].inventory[j].stats.attributes.Precision.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.Precision.Value : 0,
                                            Toughness = (charRAW.bags[i].inventory[j].stats.attributes.Toughness.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.Toughness.Value : 0,
                                            Vitality = (charRAW.bags[i].inventory[j].stats.attributes.Vitality.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.Vitality.Value : 0,
                                            ConditionDamage = (charRAW.bags[i].inventory[j].stats.attributes.ConditionDamage.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.ConditionDamage.Value : 0,
                                            ConditionDuration = (charRAW.bags[i].inventory[j].stats.attributes.ConditionDuration.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.ConditionDuration.Value : 0,
                                            Healing = (charRAW.bags[i].inventory[j].stats.attributes.Healing.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.Healing.Value : 0,
                                            BoonDuration = (charRAW.bags[i].inventory[j].stats.attributes.BoonDuration.HasValue) ? charRAW.bags[i].inventory[j].stats.attributes.BoonDuration.Value : 0
                                        };
                                        ItemStackStat itemStackStat = new ItemStackStat
                                        {
                                            Stat = itemStat,
                                            Attributes = itemStackAttribute
                                        };
                                        itemStack.Stats = itemStackStat;
                                    }

                                    bag.Contents.Add(itemStack);
                                }
                            }
                            newChar.Inv.Bags.Add(bag);
                        }
                    }
                }
            }

            main.OnCharStatusUpdate("Generated Character " + newChar.Name);
            return newChar;
        }
    }

    class CharacterRAW
    {
        public string name { get; set; }
        public string race { get; set; }
        public string gender { get; set; }
        public string[] flags { get; set; }
        public string profession { get; set; }
        public int level { get; set; }
        public string guild { get; set; }
        public int age { get; set; }
        public string created { get; set; }
        public int deaths { get; set; }
        public int? title { get; set; }
        public CraftingRAW[] crafting { get; set; }
        public string[] backstory { get; set; }
        public WVWAbilitiesRAW[] wvw_abilities { get; set; }
        public SpecializationRAW specializations { get; set; }
        public CharSkillRAW skills { get; set; }
        public EquipmentRAW[] equipment { get; set; }
        public int[] recipies { get; set; }
        public EquipmentPVPRAW equipment_pvp { get; set; }
        public TrainingRAW[] training { get; set; }
        public BagRAW[] bags { get; set; }
        public string[] heropoints { get; set; }
        public SABZoneRAW[] zones { get; set; }
        public SABUnlockRAW[] unlocks { get; set; }
        public SABSongRAW[] songs { get; set; }
    }
    class CraftingRAW
    {
        public string discipline { get; set; }
        public int rating { get; set; }
        public bool active { get; set; }
    }
    class WVWAbilitiesRAW
    {
        public int id { get; set; }
        public int rank { get; set; }
    }
    class SpecializationRAW
    {
        public SpecializationSubRAW[] pve { get; set; }
        public SpecializationSubRAW[] pvp { get; set; }
        public SpecializationSubRAW[] wvw { get; set; }
    }
    class SpecializationSubRAW
    {
        public int id { get; set; }
        public int?[] traits { get; set; }
    }
    class CharSkillRAW
    {
        public SkillSubRAW pve { get; set; }
        public SkillSubRAW pvp { get; set; }
        public SkillSubRAW wvw { get; set; }
    }
    class SkillSubRAW
    {
        public int heal { get; set; }
        public int?[] utilities { get; set; }
        public int? elite { get; set; }
        public string[] legends { get; set; }
    }
    class EquipmentRAW
    {
        public int id { get; set; }
        public string slot { get; set; }
        public int?[] infusions { get; set; }
        public int?[] upgrades { get; set; }
        public int? skin { get; set; }
        public EquipmentSub1RAW stats { get; set; }
        public string binding { get; set; }
        public int? charges { get; set; }
        public string bound_to { get; set; }
        public int?[] dyes { get; set; }
    }
    class EquipmentSub1RAW
    {
        public int id { get; set; }
        public EquipmentSub2RAW attributes { get; set; }
    }
    class EquipmentSub2RAW
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
    class EquipmentPVPRAW
    {
        public int amulet { get; set; }
        public int rune { get; set; }
        public int[] sigils { get; set; }
    }
    class TrainingRAW
    {
        public int id { get; set; }
        public int spent { get; set; }
        public bool done { get; set; }
    }
    class BagRAW
    {
        public int id { get; set; }
        public int size { get; set; }
        public BagSubRAW[] inventory { get; set; }
    }
    class BagSubRAW
    {
        public int id { get; set; }
        public int count { get; set; }
        public int?[] infusions { get; set; }
        public int?[] upgrades { get; set; }
        public int? skin { get; set; }
        public EquipmentSub1RAW stats { get; set; }
        public string binding { get; set; }
        public string bound_to { get; set; }
    }
    class SABZoneRAW
    {
        public int id { get; set; }
        public string mode { get; set; }
        public int world { get; set; }
        public int zone { get; set; }
    }
    class SABUnlockRAW
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    class SABSongRAW
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
