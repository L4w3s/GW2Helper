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
    public class GuildUpgrade
    {
        public enum Type
        {
            AccumulatingCurrency,
            BankBag,
            Boost,
            Claimable,
            Consumable,
            Decoration,
            GuildHall,
            GuildHallExpedition,
            Hub,
            Queue,
            Unlock
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type UpgradeType { get; set; }
        public string Image { get; set; }
        public int? MaxItems { get; set; }
        public int? MaxCoins { get; set; }
        public int BuildTime { get; set; }
        public int RequiredLevel { get; set; }
        public int Exp { get; set; }
        public List<GuildUpgrade> Prerequisites { get; set; }
        public List<int> PrerequisiteID { get; set; }
        public List<UpgradeCost> Costs { get; set; }
        
        public static void GetUpgradesFromJSON(string json, Main main)
        {
            GuildUpgradeRAW[] rawGuildUpgrades = new GuildUpgradeRAW[1];
            try
            {
                rawGuildUpgrades = JsonConvert.DeserializeObject<GuildUpgradeRAW[]>(json);
            }
            catch (Exception e)
            {
                rawGuildUpgrades[0] = JsonConvert.DeserializeObject<GuildUpgradeRAW>(json);
            }
            for (int g = 0; g < rawGuildUpgrades.Length; g++)
            {
                double cur = g, max = rawGuildUpgrades.Length;
                GuildUpgradeRAW guildUpgradeRAW = rawGuildUpgrades[g];
                main.JSON.Add(new KeyValuePair<string, string>("GuildUpgrade", JsonConvert.SerializeObject(guildUpgradeRAW)));
                GuildUpgrade newUpgrade = new GuildUpgrade
                {
                    ID = guildUpgradeRAW.id,
                    Name = guildUpgradeRAW.name,
                    Description = guildUpgradeRAW.description,
                    UpgradeType = (Type)Enum.Parse(typeof(Type), guildUpgradeRAW.type),
                    BuildTime = guildUpgradeRAW.build_time,
                    RequiredLevel = guildUpgradeRAW.required_level,
                    Exp = guildUpgradeRAW.experience,
                    Prerequisites = new List<GuildUpgrade>(),
                    PrerequisiteID = new List<int>(),
                    Costs = new List<UpgradeCost>()
                };

                if (guildUpgradeRAW.bag_max_items.HasValue) newUpgrade.MaxItems = guildUpgradeRAW.bag_max_items.Value;
                if (guildUpgradeRAW.bag_max_coins.HasValue) newUpgrade.MaxCoins = guildUpgradeRAW.bag_max_coins.Value;

                for (int i = 0; i < guildUpgradeRAW.costs.Length; i++)
                {
                    UpgradeCost upgradeCost = new UpgradeCost
                    {
                        CostType = (UpgradeCost.Type)Enum.Parse(typeof(UpgradeCost.Type), guildUpgradeRAW.costs[i].type),
                        Name = guildUpgradeRAW.costs[i].name,
                        Count = guildUpgradeRAW.costs[i].count
                    };
                    if (guildUpgradeRAW.costs[i].item_id.HasValue)
                    {
                        upgradeCost.ItemID = guildUpgradeRAW.costs[i].item_id.Value;
                    }
                    newUpgrade.Costs.Add(upgradeCost);
                }

                for (int i = 0; i < guildUpgradeRAW.prerequisites.Length; i++)
                {
                    newUpgrade.PrerequisiteID.Add(guildUpgradeRAW.prerequisites[i]);
                }

                if (guildUpgradeRAW.icon != null)
                {
                    string fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = guildUpgradeRAW.icon.Substring(guildUpgradeRAW.icon.LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\guild\upgrades\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\guild\upgrades\" + fileName)) client.DownloadFileAsync(new Uri(guildUpgradeRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"images\guild\upgrades\" + fileName);
                    }
                    newUpgrade.Image = AppDomain.CurrentDomain.BaseDirectory + @"images\guild\upgrades\" + fileName;
                }

                main.GuildUpgrades.Add(newUpgrade);
                main.OnCharStatusUpdate("Generated Guild Upgrade " + newUpgrade.Name + ";" + newUpgrade.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    public class UpgradeCost
    {
        public enum Type
        {
            Item,
            Collectible,
            Currency,
            Coins
        }
        public Type CostType { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public Item RequiredItem { get; set; }
        public int? ItemID { get; set; }
    }

    class GuildUpgradeRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int? bag_max_items { get; set; }
        public int? bag_max_coins { get; set; }
        public string icon { get; set; }
        public int build_time { get; set; }
        public int required_level { get; set; }
        public int experience { get; set; }
        public int[] prerequisites { get; set; }
        public GuildUpgradeSubRAW[] costs { get; set; }
    }
    class GuildUpgradeSubRAW
    {
        public string type { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public int? item_id { get; set; }
    }
}
