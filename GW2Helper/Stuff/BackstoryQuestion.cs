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
        public List<string> AnswerID { get; set; }
        public int OrderPosition { get; set; }
        public List<Character.Race> Races { get; set; }
        public List<Character.Profession> Professions { get; set; }
        
        public static void GetQuestionsFromJSON(string json, Main main)
        {
            BackstoryQuestionRAW[] rawQuestions = new BackstoryQuestionRAW[1];
            try
            {
                rawQuestions = JsonConvert.DeserializeObject<BackstoryQuestionRAW[]>(json);
            }
            catch (Exception e)
            {
                rawQuestions[0] = JsonConvert.DeserializeObject<BackstoryQuestionRAW>(json);
            }
            for (int b = 0; b < rawQuestions.Length; b++)
            {
                double cur = b, max = rawQuestions.Length;
                BackstoryQuestionRAW bsRAW = rawQuestions[b];
                main.JSON.Add(new KeyValuePair<string, string>("BackstoryQuestion", JsonConvert.SerializeObject(bsRAW)));
                BackstoryQuestion newBS = new BackstoryQuestion
                {
                    ID = bsRAW.id,
                    Question = bsRAW.title,
                    OrderPosition = bsRAW.order,
                    Answers = new List<BackstoryAnswer>(),
                    AnswerID = new List<string>(),
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
                    newBS.AnswerID.Add(bsRAW.answers[i]);
                }

                main.BackstoryQuestions.Add(newBS);
                main.OnCharStatusUpdate("Generated Backstory Question " + newBS.Question + ";" + newBS.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
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
