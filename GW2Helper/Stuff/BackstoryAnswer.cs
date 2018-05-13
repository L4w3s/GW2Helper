using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class BackstoryAnswer
    {
        public string ID { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public string Journal { get; set; }
        public BackstoryQuestion Question { get; set; }
        public List<Character.Profession> Professions { get; set; }
        public List<Character.Race> Races { get; set; }

        public static BackstoryAnswer GetAnswerFromJSON(string json, Main main)
        {
            BackstoryAnswerRAW bsRAW = JsonConvert.DeserializeObject<BackstoryAnswerRAW>(json);
            BackstoryAnswer newBS = new BackstoryAnswer
            {
                ID = bsRAW.id,
                Answer = bsRAW.title,
                Description = bsRAW.description,
                Journal = bsRAW.journal,
                Races = new List<Character.Race>(),
                Professions = new List<Character.Profession>()
            };

            if (bsRAW.races != null)
            {
                for (int i = 0; i < bsRAW.races.Length; i++)
                {
                    newBS.Races.Add((Character.Race)Enum.Parse(typeof(Character.Race), bsRAW.races[i]));
                }
            }
            if (bsRAW.professions != null)
            {
                for (int i = 0; i < bsRAW.professions.Length; i++)
                {
                    newBS.Professions.Add((Character.Profession)Enum.Parse(typeof(Character.Profession), bsRAW.professions[i]));
                }
            }

            main.BackstoryAnswers.Add(newBS);
            main.OnCharStatusUpdate("Generated Backstory Answer " + newBS.ID);
            return newBS;
        }
    }

    class BackstoryAnswerRAW
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string journal { get; set; }
        public int question { get; set; }
        public string[] professions { get; set; }
        public string[] races { get; set; }
    }
}
