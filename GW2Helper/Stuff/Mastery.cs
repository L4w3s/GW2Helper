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
    public class Mastery
    {
        public enum Region
        {
            Tyria,
            Maguuma,
            Desert
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Requirement { get; set; }
        public int Order { get; set; }
        public string MasteryBackground { get; set; }
        public Region MasteryRegion { get; set; }
        public List<MasteryLevel> Levels { get; set; }
        
        public static void GetMasteriesFromJSON(string json, Main main)
        {
            MasteryRAW[] rawMasteries = new MasteryRAW[1];
            try
            {
                rawMasteries = JsonConvert.DeserializeObject<MasteryRAW[]>(json);
            }
            catch (Exception e)
            {
                rawMasteries[0] = JsonConvert.DeserializeObject<MasteryRAW>(json);
            }
            for (int m = 0; m < rawMasteries.Length; m++)
            {
                double cur = m, max = rawMasteries.Length;
                MasteryRAW masteryRAW = rawMasteries[m];
                main.JSON.Add(new KeyValuePair<string, string>("Mastery", JsonConvert.SerializeObject(masteryRAW)));
                Mastery newMastery = new Mastery
                {
                    ID = masteryRAW.id,
                    Name = masteryRAW.name,
                    Requirement = masteryRAW.requirement,
                    Order = masteryRAW.order.Value,
                    MasteryRegion = (Region)Enum.Parse(typeof(Region), masteryRAW.region),
                    Levels = new List<MasteryLevel>()
                };
                string fileName = string.Empty;
                using (WebClient client = new WebClient())
                {
                    fileName = masteryRAW.background.Substring(masteryRAW.background.LastIndexOf("/") + 1);
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\");
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\" + fileName)) client.DownloadFileAsync(new Uri(masteryRAW.background), AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\" + fileName);
                }
                newMastery.MasteryBackground = AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\" + fileName;

                for (int i = 0; i < masteryRAW.levels.Length; i++)
                {
                    MasteryLevelRAW masteryLevelRAW = masteryRAW.levels[i];
                    MasteryLevel newMasteryLevel = new MasteryLevel
                    {
                        Name = masteryLevelRAW.name,
                        Description = masteryLevelRAW.description,
                        Instruction = masteryLevelRAW.instruction,
                        PointCost = (masteryLevelRAW.point_cost.HasValue) ? masteryLevelRAW.point_cost.Value : 0,
                        ExpCost = (masteryLevelRAW.exp_cost.HasValue) ? masteryLevelRAW.exp_cost.Value : 0
                    };

                    using (WebClient client = new WebClient())
                    {
                        fileName = masteryLevelRAW.icon.Substring(masteryLevelRAW.icon.LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\levels\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\levels\" + fileName)) client.DownloadFileAsync(new Uri(masteryLevelRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\levels\" + fileName);
                    }
                    newMasteryLevel.LevelBackground = AppDomain.CurrentDomain.BaseDirectory + @"images\masteries\levels\" + fileName;

                    newMastery.Levels.Add(newMasteryLevel);
                }

                main.Masteries.Add(newMastery);
                main.OnCharStatusUpdate("Generated Mastery " + newMastery.Name + ";" + newMastery.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    public class MasteryLevel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string LevelBackground { get; set; }
        public int PointCost { get; set; }
        public int ExpCost { get; set; }
    }

    class MasteryRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public string requirement { get; set; }
        public int? order { get; set; }
        public string background { get; set; }
        public string region { get; set; }
        public MasteryLevelRAW[] levels { get; set; }
    }
    class MasteryLevelRAW
    {
        public string name { get; set; }
        public string description { get; set; }
        public string instruction { get; set; }
        public string icon { get; set; }
        public int? point_cost { get; set; }
        public int? exp_cost { get; set; }
    }
}
