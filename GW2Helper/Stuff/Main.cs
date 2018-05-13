using GW2Helper.Stuff.CharacterStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Main
    {
        public event EventHandler<string> CharStatusUpdate;

        public List<BackstoryAnswer> BackstoryAnswers = new List<BackstoryAnswer>();
        public List<BackstoryQuestion> BackstoryQuestions = new List<BackstoryQuestion>();
        public List<Item> Items = new List<Item>();
        public List<Recipe> Recipes = new List<Recipe>();
        public List<Achievement> Achievements = new List<Achievement>();
        public List<Title> Titles = new List<Title>();
        public List<Mastery> Masteries = new List<Mastery>();
        public List<MiniPet> Minis = new List<MiniPet>();
        public List<Skin> Skins = new List<Skin>();
        public List<Colour> Colours = new List<Colour>();

        public virtual void OnCharStatusUpdate(string e)
        {
            CharStatusUpdate?.Invoke(this, e);
        }
    }
}
