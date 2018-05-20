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
    public class Title
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Achievement> Achievements { get; set; }
        public List<int> AchievementID { get; set; }
        public int PointRequirement { get; set; }
        
        public static void GetTitlesFromJSON(string json, Main main)
        {
            TitleRAW[] rawTitles = new TitleRAW[1];
            try
            {
                rawTitles = JsonConvert.DeserializeObject<TitleRAW[]>(json);
            }
            catch (Exception e)
            {
                rawTitles[0] = JsonConvert.DeserializeObject<TitleRAW>(json);
            }
            for (int t = 0; t < rawTitles.Length; t++)
            {
                double cur = t, max = rawTitles.Length;
                TitleRAW titleRAW = rawTitles[t];
                main.JSON.Add(new KeyValuePair<string, string>("Title", JsonConvert.SerializeObject(titleRAW)));
                Title newTitle = new Title
                {
                    ID = titleRAW.id,
                    Name = titleRAW.name,
                    PointRequirement = titleRAW.ap_required,
                    Achievements = new List<Achievement>(),
                    AchievementID = new List<int>()
                };
                if (titleRAW.achievements != null)
                {
                    for (int i = 0; i < titleRAW.achievements.Length; i++)
                    {
                        newTitle.AchievementID.Add(titleRAW.achievements[i]);
                    }
                }
                main.Titles.Add(newTitle);
                main.OnCharStatusUpdate("Generated Title " + newTitle.Name + ";" + newTitle.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
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
