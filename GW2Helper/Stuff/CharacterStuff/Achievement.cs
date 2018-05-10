using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public List<Reward> Rewards { get; set; }
        public List<Bit> Bits { get; set; }
        public int PointCap { get; set; }

        public static Achievement GetAchievementFromJSON(string json, Main main)
        {
            AchievementRAW achievementRAW = JsonConvert.DeserializeObject<AchievementRAW>(json);
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
                Rewards = new List<Reward>(),
                Bits = new List<Bit>()
            };

            if (!string.IsNullOrEmpty(achievementRAW.icon))
            {
                string fileName = string.Empty;
                using (WebClient client = new WebClient())
                {
                    fileName = achievementRAW.icon.Substring(achievementRAW.icon.LastIndexOf("/") + 1);
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName)) client.DownloadFileAsync(new Uri(achievementRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName);
                }
                newAchievement.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName;
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
            for (int i = 0; i < achievementRAW.prerequisites.Length; i++)
            {
                int achID = achievementRAW.prerequisites[i].Value;

                WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/achievements/" + achID);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                string html = string.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Achievement achievement = (main.Achievements.Where(ach => ach.ID == achID).ToArray().Length > 0) ? main.Achievements.FirstOrDefault(ach => ach.ID == achID) : GetAchievementFromJSON(html, main);
                newAchievement.Prerequisites.Add(achievement);
            }
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
                        Count = rewardRAW.count.Value
                    };
                    int itemID = rewardRAW.id.Value;

                    WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/items/" + itemID);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();

                    string html = string.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        html = sr.ReadToEnd();
                    }

                    Item item = (main.Items.Where(it => it.ID == itemID).ToArray().Length > 0) ? main.Items.FirstOrDefault(it => it.ID == itemID) : Item.GetItemFromJSON(html, main);
                    newReward.RewardItem = item;

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
            }
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
                        BitType = Bit.Type.Item
                    };
                    int itemID = bitRAW.id.Value;

                    WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/items/" + itemID);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();

                    string html = string.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        html = sr.ReadToEnd();
                    }

                    Item item = (main.Items.Where(it => it.ID == itemID).ToArray().Length > 0) ? main.Items.FirstOrDefault(it => it.ID == itemID) : Item.GetItemFromJSON(html, main);
                    newBit.BitItem = item;

                    newAchievement.Bits.Add(newBit);
                }
                else if (bitRAW.type == "Minipet")
                {
                    MinipetBit newBit = new MinipetBit
                    {
                        BitType = Bit.Type.Minipet
                    };
                    int miniID = bitRAW.id.Value;
                    newBit.BitMini = main.Minis.FirstOrDefault(min => min.ID == miniID);

                    newAchievement.Bits.Add(newBit);
                }
                else if (bitRAW.type == "Skin")
                {
                    SkinBit newBit = new SkinBit
                    {
                        BitType = Bit.Type.Skin
                    };
                    int skinID = bitRAW.id.Value;

                    WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/skins/" + skinID);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();

                    string html = string.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        html = sr.ReadToEnd();
                    }

                    Skin skin = (main.Skins.Where(sk => sk.ID == skinID).ToArray().Length > 0) ? main.Skins.FirstOrDefault(sk => sk.ID == skinID) : Skin.GetSkinFromJSON(html, main);
                    newBit.BitSkin = skin;
                }
            }
            string fileName = string.Empty;
            using (WebClient client = new WebClient())
            {
                fileName = achievementRAW.icon.Substring(achievementRAW.icon.LastIndexOf("/") + 1);
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName)) client.DownloadFileAsync(new Uri(achievementRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName);
            }
            newAchievement.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\achievements\" + fileName;

            main.Achievements.Add(newAchievement);

            return newAchievement;
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
    }
    public class MinipetBit : Bit
    {
        public MiniPet BitMini { get; set; }
    }
    public class SkinBit : Bit
    {
        public Skin BitSkin { get; set; }
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
