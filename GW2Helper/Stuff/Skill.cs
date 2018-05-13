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
        public enum SkillSlot
        {
            Downed_1,
            Downed_2,
            Downed_3,
            Downed_4,
            Pet,
            Profession_1,
            Profession_2,
            Profession_3,
            Profession_4,
            Profession_5,
            Utility,
            Weapon_1,
            Weapon_2,
            Weapon_3,
            Weapon_4,
            Weapon_5
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Type SkillType { get; set; }
        public string WeaponType { get; set; }
        public List<Character.Profession> Professions { get; set; }
        public SkillSlot Slot { get; set; }
        public List<SkillFact> Facts { get; set; }
        public List<SkillFact> TraitedFacts { get; set; }
        public List<string> Categories { get; set; }
        public string Attunement { get; set; }
        public int? Cost { get; set; }
        public string DualWieldRequirement { get; set; }
        public int? FlipTo { get; set; }
        public int? Initiative { get; set; }
        public int? NextChain { get; set; }
        public int? PrevChain { get; set; }
        public List<Skill> TransformSkills { get; set; }
        public List<Skill> BundleSkills { get; set; }
        public List<Skill> ToolbeltSkills { get; set; }
    }
    public abstract class SkillFact
    {
        public enum FactType
        {
            AttributeAdjust,
            Buff,
            ComboField,
            ComboFinisher,
            Damage,
            Distance,
            Duration,
            Heal,
            HealingAdjust,
            NoData,
            Number,
            Percent,
            PrefixedBuff,
            Radius,
            Range,
            Recharge,
            Time,
            Unblockable
        }
        public string Text { get; set; }
        public string Image { get; set; }
        public FactType Type { get; set; }
        public int? RequiredTrait { get; set; }
        public int? Override { get; set; }
    }
    public class AttriubuteAdjustSkillFact : SkillFact
    {
        public int Value { get; set; }
        public string Target { get; set; }
    }
    public class BuffSkillFact : SkillFact
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public int? Duration { get; set; }
    }
    public class ComboFieldSkillFact : SkillFact
    {
        public enum FieldType
        {
            Air,
            Dark,
            Fire,
            Ice,
            Light,
            Lightning,
            Poision,
            Smoke,
            Ethereal,
            Water
        }
        public FieldType ComboFieldType { get; set; }
    }
    public class ComboFinisherSkillFact : SkillFact
    {
        public enum FinisherType
        {
            Blast,
            Leap,
            Projectile,
            Whirl
        }
        public FinisherType ComboFinisherType { get; set; }
        public int Percent { get; set; }
    }
    public class DamageSkillFact : SkillFact
    {
        public int HitCount { get; set; }
        public int Multiplier { get; set; }
    }
    public class DistanceSkillFact : SkillFact
    {
        public int Distance { get; set; }
    }
    public class DurationSkillFact : SkillFact
    {
        public int Duration { get; set; }
    }
    public class HealSkillFact : SkillFact
    {
        public int HitCount { get; set; }
    }
    public class HealingAdjustSkillFact : SkillFact
    {
        public int HitCount { get; set; }
    }
    public class NoDataSkillFact : SkillFact
    {
    }
    public class NumberSkillFact : SkillFact
    {
        public int Value { get; set; }
    }
    public class PercentSkillFact : SkillFact
    {
        public int Percent { get; set; }
    }
    public class PrefixedBuffSkillFact : SkillFact
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public int? Duration { get; set; }
        public Prefix BuffPrefix { get; set; }
    }
    public class RadiusSkillFact : SkillFact
    {
        public int Distance { get; set; }
    }
    public class RangeSkillFact : SkillFact
    {
        public int Range { get; set; }
    }
    public class RechargeSkillFact : SkillFact
    {
        public int Value { get; set; }
    }
    public class TimeSkillFact : SkillFact
    {
        public int Duration { get; set; }
    }
    public class UnblockableSkillFact : SkillFact
    {
        public bool Value { get; set; }
    }
    public class Prefix
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
