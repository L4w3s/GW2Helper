using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Achievement
    {
        public enum Type
        {
            Default,
            ItemSet
        }
        public enum Flag
        {
            Pvp,
            CategoryDisplay,
            MoveToTop,
            IgnoreNearlyComplete,
            Repeatable,
            Hidden,
            RequiresUnlock,
            RepairOnLogin,
            Daily,
            Weekly,
            Monthly,
            Permanent
        }
        public int ID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string UnlockedDescription { get; set; }
        public string Requirement { get; set; }
        public string LockedDescription { get; set; }
        public Type AchieveType { get; set; }
        public List<Flag> Flags { get; set; }
        public List<Tier> Tiers { get; set; }
        public List<Achievement> Prerequisites { get; set; }
        public List<int> PrerequisiteID { get; set; }
        public List<Reward> Rewards { get; set; }
        public List<Bit> Bits { get; set; }
        public int PointCap { get; set; }

        public static void GetAchievementsFromJSON(string json, Main main)
        {
            AchievementRAW[] rawAchievements = new AchievementRAW[1];
            try
            {
                rawAchievements = JsonConvert.DeserializeObject<AchievementRAW[]>(json);
            }
            catch (Exception e)
            {
                rawAchievements[0] = JsonConvert.DeserializeObject<AchievementRAW>(json);
            }
            for (int a = 0; a < rawAchievements.Length; a++)
            {
                double cur = a, max = rawAchievements.Length;
                AchievementRAW achievementRAW = rawAchievements[a];
                main.JSON.Add(new KeyValuePair<string, string>("Achievement", JsonConvert.SerializeObject(achievementRAW)));
                Achievement newAchievement = new Achievement
                {
                    ID = achievementRAW.id,
                    Name = achievementRAW.name,
                    UnlockedDescription = achievementRAW.description,
                    Requirement = achievementRAW.requirement,
                    LockedDescription = achievementRAW.locked_text,
                    PointCap = (achievementRAW.point_cap.HasValue) ? achievementRAW.point_cap.Value : 0,
                    AchieveType = (Type)Enum.Parse(typeof(Type), achievementRAW.type),
                    Flags = new List<Flag>(),
                    Tiers = new List<Tier>(),
                    Prerequisites = new List<Achievement>(),
                    PrerequisiteID = new List<int>(),
                    Rewards = new List<Reward>(),
                    Bits = new List<Bit>(),
                };
                string fileName = string.Empty;

                if (!string.IsNullOrEmpty(achievementRAW.icon))
                {
                    fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = achievementRAW.icon.Substring(achievementRAW.icon.LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\achievements\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\achievements\" + fileName)) client.DownloadFileAsync(new Uri(achievementRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"images\achievements\" + fileName);
                    }
                    newAchievement.Image = AppDomain.CurrentDomain.BaseDirectory + @"images\achievements\" + fileName;
                }
                for (int i = 0; i < achievementRAW.flags.Length; i++)
                {
                    newAchievement.Flags.Add((Flag)Enum.Parse(typeof(Flag), achievementRAW.flags[i]));
                }
                for (int i = 0; i < achievementRAW.tiers.Length; i++)
                {
                    TierRAW tierRAW = achievementRAW.tiers[i];
                    Tier newTier = new Tier
                    {
                        Count = tierRAW.count,
                        Points = tierRAW.points
                    };
                    newAchievement.Tiers.Add(newTier);
                }
                if (achievementRAW.prerequisites != null)
                {
                    for (int i = 0; i < achievementRAW.prerequisites.Length; i++)
                    {
                        newAchievement.PrerequisiteID.Add(achievementRAW.prerequisites[i].Value);
                    }
                }
                if (achievementRAW.rewards != null)
                {
                    for (int i = 0; i < achievementRAW.rewards.Length; i++)
                    {
                        RewardRAW rewardRAW = achievementRAW.rewards[i];
                        if (rewardRAW.type == "Coins")
                        {
                            CoinReward newReward = new CoinReward
                            {
                                RewardType = Reward.Type.Coin,
                                Count = rewardRAW.count.Value
                            };
                            newAchievement.Rewards.Add(newReward);
                        }
                        else if (rewardRAW.type == "Item")
                        {
                            ItemReward newReward = new ItemReward
                            {
                                RewardType = Reward.Type.Item,
                                Count = rewardRAW.count.Value,
                                ItemID = rewardRAW.id.Value
                            };

                            newAchievement.Rewards.Add(newReward);
                        }
                        else if (rewardRAW.type == "Mastery")
                        {
                            MasteryReward newReward = new MasteryReward
                            {
                                RewardType = Reward.Type.Mastery,
                                RegionMastery = (Mastery.Region)Enum.Parse(typeof(Mastery.Region), achievementRAW.rewards[i].region)
                            };
                            int masteryID = rewardRAW.id.Value;

                            Mastery mastery = main.Masteries.FirstOrDefault(ma => ma.ID == masteryID);
                            newReward.RewardMastery = mastery;

                            newAchievement.Rewards.Add(newReward);
                        }
                        else if (rewardRAW.type == "Title")
                        {
                            TitleReward newReward = new TitleReward
                            {
                                RewardType = Reward.Type.Title,
                                TitleID = rewardRAW.id.Value
                            };

                            newAchievement.Rewards.Add(newReward);
                        }
                    }
                }
                if (achievementRAW.bits != null)
                {
                    for (int i = 0; i < achievementRAW.bits.Length; i++)
                    {
                        BitRAW bitRAW = achievementRAW.bits[i];

                        if (bitRAW.type == "Text")
                        {
                            TextBit newBit = new TextBit
                            {
                                BitType = Bit.Type.Text,
                                Text = bitRAW.text
                            };

                            newAchievement.Bits.Add(newBit);
                        }
                        else if (bitRAW.type == "Item")
                        {
                            ItemBit newBit = new ItemBit
                            {
                                BitType = Bit.Type.Item,
                                ItemID = bitRAW.id.Value
                            };

                            newAchievement.Bits.Add(newBit);
                        }
                        else if (bitRAW.type == "Minipet")
                        {
                            MinipetBit newBit = new MinipetBit
                            {
                                BitType = Bit.Type.Minipet,
                                MiniID = bitRAW.id.Value
                            };

                            newAchievement.Bits.Add(newBit);
                        }
                        else if (bitRAW.type == "Skin")
                        {
                            SkinBit newBit = new SkinBit
                            {
                                BitType = Bit.Type.Skin,
                                SkinID = bitRAW.id.Value
                            };

                            newAchievement.Bits.Add(newBit);
                        }
                    }
                }

                main.Achievements.Add(newAchievement);
                main.OnCharStatusUpdate("Generated Achievement " + newAchievement.Name + ";" + newAchievement.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
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
        public int ItemID { get; set; }
        public int Count { get; set; }
    }
    public class MasteryReward : Reward
    {
        public Mastery RewardMastery { get; set; }
        public Mastery.Region RegionMastery { get; set; }
    }
    public class TitleReward : Reward
    {
        public Title RewardTitle { get; set; }
        public int TitleID { get; set; }
    }

    public abstract class Bit
    {
        public enum Type
        {
            Text,
            Item,
            Minipet,
            Skin
        }
        public Type BitType { get; set; }
    }
    public class TextBit : Bit
    {
        public string Text { get; set; }
    }
    public class ItemBit : Bit
    {
        public Item BitItem { get; set; }
        public int ItemID { get; set; }
    }
    public class MinipetBit : Bit
    {
        public MiniPet BitMini { get; set; }
        public int MiniID { get; set; }
    }
    public class SkinBit : Bit
    {
        public Skin BitSkin { get; set; }
        public int SkinID { get; set; }
    }

    class AchievementRAW
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string requirement { get; set; }
        public string locked_text { get; set; }
        public string type { get; set; }
        public string[] flags { get; set; }
        public TierRAW[] tiers { get; set; }
        public int?[] prerequisites { get; set; }
        public RewardRAW[] rewards { get; set; }
        public BitRAW[] bits { get; set; }
        public int? point_cap { get; set; }
    }
    class TierRAW
    {
        public int count { get; set; }
        public int points { get; set; }
    }
    class RewardRAW
    {
        public string type { get; set; }
        public int? id { get; set; }
        public int? count { get; set; }
        public string region { get; set; }
    }
    class BitRAW
    {
        public string type { get; set; }
        public int? id { get; set; }
        public string text { get; set; }
    }
}
