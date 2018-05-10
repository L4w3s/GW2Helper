using GW2Helper.Stuff.CharacterStuff;
using GW2Helper.Stuff.CharacterStuff.Equipment;
using GW2Helper.Stuff.CharacterStuff.Skills;
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
    }
}
