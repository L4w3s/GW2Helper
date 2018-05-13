using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using GW2Helper.Stuff;
using Newtonsoft.Json;

namespace GW2Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        // CHECK OUT API DOCUMENTATION HERE:
        //https://wiki.guildwars2.com/wiki/API:Main
        //
        //https://wiki.guildwars2.com/wiki/API:2/items
        //E.G. https://api.guildwars2.com/v2/items/19596
        //
        //https://wiki.guildwars2.com/wiki/API:2/characters
        //E.G. https://api.guildwars2.com/v2/characters/Sunbucks?access_token=0A5E1557-B486-0D42-B70B-05B45DA2A43C431C5179-393D-4A88-8633-3E42EC6A3EB2
        //
        //API KEY:
        //0A5E1557-B486-0D42-B70B-05B45DA2A43C431C5179-393D-4A88-8633-3E42EC6A3EB2
        //
        //CREATE JSON OBJECTS FROM STRINGS RETURNED VIA API LINKS
        //
        //
        //
        //COMPLETE SKILL!

        Character character = new Character();
        Action _cancelWork;
        Main Global = new Main();

        public MainWindow()
        {
            InitializeComponent();
            Global.CharStatusUpdate += charStatusUpdate;
        }

        public Character GetCharacter(string selectedCharacter, string apiToken)
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/characters/" + selectedCharacter + "?access_token=" + apiToken);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            return Character.GetCharacterFromJSON(html, Global);
        }
        public void LoadCharacters(string apiToken)
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/characters" + "?access_token=" + apiToken);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<string> characters = JsonConvert.DeserializeObject<List<string>>(html);
            List<CharacterListObject> characterList = new List<CharacterListObject>();
            for (int i = 0; i < characters.Count; i++)
            {
                characterList.Add(new CharacterListObject { CharacterName = characters[i], APIToken = apiToken });
            }
            Dispatcher.Invoke(() =>
            {
                lst_characters.ItemsSource = characterList;
            });
        }

        private async void GWTwoHelper_Loaded(object sender, RoutedEventArgs e)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;

            await Task.Run(() => task_PopulateMasteries(), token);
            await Task.Run(() => task_PopulateBackstories(), token);
            //await Task.Run(() => task_PopulateTitles(), token);
            //await Task.Run(() => task_PopulateMinipets(), token);
            //await Task.Run(() => task_PopulateColours(), token);
            //await Task.Run(() => task_PopulateRecipes(), token);

            lbl_charloaddetails.Content = "Ready for Loading!";
        }
        public void charStatusUpdate(object sender, string text)
        {
            Dispatcher.Invoke(() =>
            {
                lbl_charloaddetails.Content = text;
            });
        }

        public void task_PopulateRecipes()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/recipes");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> recipeIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < recipeIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/recipes/" + recipeIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.Recipes.Add(Recipe.GetRecipeFromJSON(html));
            }
        }
        public void task_PopulateBackstories()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/backstory/questions");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> backstoryIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < backstoryIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/backstory/questions/" + backstoryIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.BackstoryQuestions.Add(BackstoryQuestion.GetBackstoryFromJSON(html, Global));
            }
        }
        public void task_PopulateMasteries()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/masteries");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> masteryIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < masteryIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/masteries/" + masteryIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.Masteries.Add(Mastery.GetMasteryFromJSON(html, Global));
            }
        }
        public void task_PopulateMinipets()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/minis");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> miniIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < miniIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/minis/" + miniIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.Minis.Add(MiniPet.GetMiniPetFromJSON(html, Global));
            }
        }
        public void task_PopulateColours()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/colors");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> colourIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < colourIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/colors/" + colourIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.Colours.Add(Colour.GetColourFromJSON(html, Global));
            }
        }
        public void task_PopulateTitles()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/titles");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<int> titleIDs = JsonConvert.DeserializeObject<List<int>>(html);

            for (int i = 0; i < titleIDs.Count; i++)
            {
                request = WebRequest.Create("https://api.guildwars2.com/v2/titles/" + titleIDs[i]);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                Global.Titles.Add(Stuff.CharacterStuff.Title.GetTitleFromJSON(html, Global));
            }
        }

        private async void btn_loadchar_Click(object sender, RoutedEventArgs e)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            string apiToken = txt_api.Text;
            await Task.Run(() => LoadCharacters(apiToken), token);

            Dispatcher.Invoke(() =>
            {
                lbl_charloaddetails.Content = "Ready for Loading!";
            });
        }

        private async void lst_characters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            CharacterListObject charListObject = (CharacterListObject)lst_characters.SelectedItem;
            string charName = charListObject.CharacterName.Replace(" ", "%20");
            string apiToken = charListObject.APIToken;

            await Task.Run(() => character = GetCharacter(charName, apiToken), token);

            Dispatcher.Invoke(() =>
            {
                journal_name.Content = character.Name;
                string journalEntry = character.Backstory[0].Journal + character.Backstory[1].Journal + " " + character.Backstory[2].Journal + character.Backstory[3].Journal + " " + character.Backstory[4].Journal;
                journalEntry = journalEntry.Replace("<br>", Environment.NewLine);
                journal_entry.Text = journalEntry;

                lbl_charloaddetails.Content = "Ready for Loading!";
            });
        }
    }
}
