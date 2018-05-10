using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Skill
    {
        public enum Type
        {
            Bundle,
            Elite,
            Heal,
            Profession,
            Utility,
            Weapon
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
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Type SkillType { get; set; }
        public string WeaponType { get; set; }
        public List<Profession> Professions { get; set; }
    }
}
