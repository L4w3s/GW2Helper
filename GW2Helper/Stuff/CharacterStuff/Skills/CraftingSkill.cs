using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.CharacterStuff.Skills
{
    public class CraftingSkill
    {
        public enum Discipline
        {
            Armorsmith,
            Artificer,
            Chef,
            Huntsman,
            Jeweler,
            Leatherworker,
            Scribe,
            Tailor,
            Weaponsmith
        }
        public Discipline SkillDiscipline { get; set; }
        public int Rating { get; set; }
        public bool Active { get; set; }
    }
}
