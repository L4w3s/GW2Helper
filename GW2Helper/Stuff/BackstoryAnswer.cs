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
        public int QuestionID { get; set; }
        public List<Character.Profession> Professions { get; set; }
        public List<Character.Race> Races { get; set; }

        public static void GetAnswersFromJSON(string json, Main main)
        {
            BackstoryAnswerRAW[] rawAnswers = new BackstoryAnswerRAW[1];
            try
            {
                rawAnswers = JsonConvert.DeserializeObject<BackstoryAnswerRAW[]>(json);
            }
            catch (Exception e)
            {
                rawAnswers[0] = JsonConvert.DeserializeObject<BackstoryAnswerRAW>(json);
            }
            for (int a = 0; a < rawAnswers.Length; a++)
            {
                double cur = a, max = rawAnswers.Length;
                BackstoryAnswerRAW bsRAW = rawAnswers[a];
                main.JSON.Add(new KeyValuePair<string, string>("BackstoryAnswer", JsonConvert.SerializeObject(bsRAW)));
                BackstoryAnswer newBS = new BackstoryAnswer
                {
                    ID = bsRAW.id,
                    Answer = bsRAW.title,
                    Description = bsRAW.description,
                    Journal = bsRAW.journal,
                    QuestionID = bsRAW.question,
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
                main.OnCharStatusUpdate("Generated Backstory Answer " + newBS.Answer + ";" + newBS.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
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
