using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.CharacterStuff
{
    public class Achievement
    {
        public enum Type
        {
            Default,
            ItemSet
        }
        public int ID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string UnlockedDescription { get; set; }
        public string Requirement { get; set; }
        public string LockedDescription { get; set; }
        public Type AchieveType { get; set; }
        public List<string> Flags { get; set; }
        public List<Tier> Tiers { get; set; }
        public List<Achievement> Prerequisites { get; set; }
        public List<Reward> Rewards { get; set; }
        public List<Bit> Bits { get; set; }
        public int PointCap { get; set; }
    }

    public class Tier
    {
        public int Count { get; set; }
        public int Points { get; set; }
    }

    public abstract class Reward
    {
        public enum Type
        {
            Coin,
            Item,
            Mastery,
            Title
        }
        public Type RewardType { get; set; }
    }
    public class CoinReward : Reward
    {
        public int Count { get; set; }
    }
    public class ItemReward : Reward
    {
        public Item RewardItem { get; set; }
        public int Count { get; set; }
    }
    public class MasteryReward : Reward
    {
        public enum Region
        {
            Tyria,
            Maguuma,
            Desert
        }
        public Mastery RewardMastery { get; set; }
        public Region RegionMastery { get; set; }
    }
    public class TitleReward : Reward
    {
        public Title RewardTitle { get; set; }
    }

    public class Bit
    {
        public enum Type
        {
            Text,
            Item,
            Minipet,
            Skin
        }
        public Type BitType { get; set; }
        public int ID { get; set; }
        public string Text { get; set; }
    }
}
