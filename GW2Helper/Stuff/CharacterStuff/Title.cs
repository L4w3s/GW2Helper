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
    public class Title
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Achievement> Achievements { get; set; }
        public int PointRequirement { get; set; }

        public static Title GetTitleFromJSON(string json, Main main)
        {
            TitleRAW titleRAW = JsonConvert.DeserializeObject<TitleRAW>(json);
            Title newTitle = new Title
            {
                ID = titleRAW.id,
                Name = titleRAW.name,
                PointRequirement = titleRAW.ap_required,
                Achievements = new List<Achievement>()
            };
            for (int i = 0; i < titleRAW.achievements.Length; i++)
            {
                int achID = titleRAW.achievements[i];

                WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/achievements/" + achID);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                string html = string.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Achievement achievement = (main.Achievements.Where(ach => ach.ID == achID).ToArray().Length > 0) ? main.Achievements.FirstOrDefault(ach => ach.ID == achID) : Achievement.GetAchievementFromJSON(html, main);
                achievement.Rewards.Add(new TitleReward
                {
                    RewardType = Reward.Type.Title,
                    RewardTitle = newTitle
                });

                newTitle.Achievements.Add(achievement);
            }
            main.Titles.Add(newTitle);

            return newTitle;
        }
    }

    class TitleRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public int[] achievements { get; set; }
        public int ap_required { get; set; }
    }
}
