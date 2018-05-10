using GW2Helper.Stuff.CharacterStuff;
using GW2Helper.Stuff.CharacterStuff.Equipment;
using GW2Helper.Stuff.CharacterStuff.Skills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Guild { get; set; }
        public int Age { get; set; }
        public DateTime CreationDate { get; set; }
        public int Deaths { get; set; }
        public Title CharTitle { get; set; }
        public List<CraftingSkill> CraftingSkills { get; set; }
        public EquipmentContainer Equips { get; set; }
        public List<string> Heropoints { get; set; }
        public Inventory Bags { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Specialization> Specializations { get; set; }
        public List<string> WVWAbilities { get; set; }
        public List<Equipment> PVPEquips { get; set; }
        public List<Recipe> Recipes { get; set; }
        public List<Training> TrainingStats { get; set; }

        public static Character GetCharacterFromJSON(string json)
        {
            CharacterRAW charRAW = JsonConvert.DeserializeObject<CharacterRAW>(json);
            Character newChar = new Character
            {
                Name = charRAW.name,
                CharRace = (Race)Enum.Parse(typeof(Race), charRAW.race),
                CharGender = (Gender)Enum.Parse(typeof(Gender), charRAW.gender),
                Flags = charRAW.flags.ToList(),
                Backstory = new List<BackstoryAnswer>(),
                CharProfession = (Profession)Enum.Parse(typeof(Profession), charRAW.profession)
            };

            for (int i = 0; i < charRAW.backstory.Length; i++)
            {
                string id = charRAW.backstory[i];
                newChar.Backstory.Add(BackstoryAnswer.FromID(id));
            }

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
        public SkillRAW skills { get; set; }
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
        public int[] traits { get; set; }
    }
    class SkillRAW
    {
        public SkillSubRAW[] pve { get; set; }
        public SkillSubRAW[] pvp { get; set; }
        public SkillSubRAW[] wvw { get; set; }
    }
    class SkillSubRAW
    {
        public int heal { get; set; }
        public int[] utilities { get; set; }
        public int elite { get; set; }
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
