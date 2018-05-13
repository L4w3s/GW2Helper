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
    public class BackstoryQuestion
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public List<BackstoryAnswer> Answers { get; set; }
        public int OrderPosition { get; set; }
        public List<Character.Race> Races { get; set; }
        public List<Character.Profession> Professions { get; set; }

        public static BackstoryQuestion GetBackstoryFromJSON(string json, Main main)
        {
            BackstoryQuestionRAW bsRAW = JsonConvert.DeserializeObject<BackstoryQuestionRAW>(json);
            BackstoryQuestion newBS = new BackstoryQuestion
            {
                ID = bsRAW.id,
                Question = bsRAW.title,
                OrderPosition = bsRAW.order,
                Answers = new List<BackstoryAnswer>(),
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
            for (int i = 0; i < bsRAW.answers.Length; i++)
            {
                WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/backstory/answers/" + bsRAW.answers[i]);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                string html = string.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                BackstoryAnswer newBSAns = BackstoryAnswer.GetAnswerFromJSON(html, main);

                newBS.Answers.Add(newBSAns);
            }

            for (int i = 0; i < newBS.Answers.Count; i++)
            {
                newBS.Answers[i].Question = newBS;
            }

            main.OnCharStatusUpdate("Generated Backstory Question " + newBS.ID);
            return newBS;
        }
    }
    class BackstoryQuestionRAW
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] answers { get; set; }
        public int order { get; set; }
        public string[] races { get; set; }
        public string[] professions { get; set; }
    }
}
