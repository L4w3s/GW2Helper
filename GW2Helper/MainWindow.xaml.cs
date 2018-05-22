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
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using GW2Helper.Stuff;
using GW2Helper.Stuff.CharacterStuff.Equipment;
using Newtonsoft.Json;

namespace GW2Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
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
        CharacterListObject selectedCharacter = new CharacterListObject();
        Action _cancelWork;
        Main Global = new Main();
        DispatcherTimer timer1, timer2;
        public delegate void timerTick();
        timerTick tick1, tick2;
        DateTime lastCharacterRefresh = DateTime.Now, nextCharacterRefresh = DateTime.Now.AddMinutes(5);

        public MainWindow()
        {
            InitializeComponent();
            Global.CharStatusUpdate += charStatusUpdate;

            Global.ObjectTypes.Add("Achievement");
            Global.ObjectTypes.Add("BackstoryQuestion");
            Global.ObjectTypes.Add("BackstoryAnswer");
            Global.ObjectTypes.Add("Colour");
            Global.ObjectTypes.Add("GuildEmblemBackground");
            Global.ObjectTypes.Add("GuildEmblemForeground");
            Global.ObjectTypes.Add("ItemStat");
            Global.ObjectTypes.Add("Item");
            Global.ObjectTypes.Add("Mastery");
            Global.ObjectTypes.Add("Minipet");
            Global.ObjectTypes.Add("Recipe");
            Global.ObjectTypes.Add("Skill");
            Global.ObjectTypes.Add("Skin");
            Global.ObjectTypes.Add("Title");

            inv_1.Items.Clear();
            inv_2.Items.Clear();
            inv_3.Items.Clear();
            bank.Items.Clear();
            skill_pve.Items.Clear();
            skill_pvp.Items.Clear();
            skill_wvw.Items.Clear();

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMinutes(5);
            timer1.Tick += new EventHandler(timer_Tick1);
            tick1 = new timerTick(task_RefreshTimer1);
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += new EventHandler(timer_Tick2);
            tick2 = new timerTick(task_RefreshTimer2);
        }
        void timer_Tick1(object sender, EventArgs e)
        {
            Dispatcher.Invoke(tick1);
        }
        void timer_Tick2(object sender, EventArgs e)
        {
            Dispatcher.Invoke(tick2);
        }
        async void task_RefreshTimer1()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            selectedCharacter = (CharacterListObject)lst_characters.SelectedItem;

            Dispatcher.Invoke(() =>
            {
                lbl_loaddetails.Content = "Refreshing character " + selectedCharacter.CharacterName;
            });

            await Task.Run(() => RefreshCharacter(), token);
        }
        async void task_RefreshTimer2()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            TimeSpan time1 = DateTime.Now - lastCharacterRefresh;
            TimeSpan time2 = nextCharacterRefresh - DateTime.Now;

            bool minutes1 = ((time1.TotalMinutes >= 1.0) ? true : false);
            bool minutes2 = ((time2.TotalMinutes >= 1.0) ? true : false);

            Dispatcher.Invoke(() =>
            {
                lbl_loadid.Content = "Last: " + ((minutes1) ? time1.Minutes.ToString("D2") + "m:" : "") + time1.Seconds.ToString("D2") + "s  Next: " + ((minutes2) ? time2.Minutes.ToString("D2") + "m:" : "") + time2.Seconds.ToString("D2") + "s";
            });
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

            return Character.GetCharacterFromJSON(html, Global, apiToken);
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

            Global.SavedCharacters.Clear();
            List<string> characters = JsonConvert.DeserializeObject<List<string>>(html);
            List<CharacterListObject> characterList = new List<CharacterListObject>();
            if (!chk_replacecharlist.IsChecked.Value)
            {
                characterList = lst_characters.Items.OfType<CharacterListObject>().ToList();
            }
            for (int i = 0; i < characters.Count; i++)
            {
                characterList.Add(new CharacterListObject { CharacterName = characters[i], APIToken = apiToken });
            }
            for (int i = 0; i < characterList.Count; i++)
            {
                Global.SavedCharacters.Add(characterList[i]);
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

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "characters.csv"))
            {
                List<CharacterListObject> characterList = new List<CharacterListObject>();

                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "characters.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(new[] { ";" }, StringSplitOptions.None);

                        CharacterListObject charListObj = new CharacterListObject
                        {
                            CharacterName = values[0],
                            APIToken = values[1]
                        };
                        characterList.Add(charListObj);
                        Global.SavedCharacters.Add(charListObj);
                    }
                }

                lst_characters.ItemsSource = characterList;
            }

            for (int i = 0; i < Global.ObjectTypes.Count; i++)
            {
                string type = Global.ObjectTypes[i];
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"json\" + type + ".json"))
                {
                    await Task.Run(() => LoadJSONFromCSV(type), token);
                }
                else
                {
                    if (type == "Achievement") await Task.Run(() => task_PopulateAchievements(), token);
                    if (type == "BackstoryQuestion") await Task.Run(() => task_PopulateBackstoryQuestions(), token);
                    if (type == "BackstoryAnswer") await Task.Run(() => task_PopulateBackstoryAnswers(), token);
                    if (type == "Colour") await Task.Run(() => task_PopulateColours(), token);
                    if (type == "GuildEmblemBackground") await Task.Run(() => task_PopulateGuildEmblemBackgrounds(), token);
                    if (type == "GuildEmblemForeground") await Task.Run(() => task_PopulateGuildEmblemForegrounds(), token);
                    if (type == "GuildUpgrade") await Task.Run(() => task_PopulateGuildUpgrades(), token);
                    if (type == "ItemStat") await Task.Run(() => task_PopulateItemStats(), token);
                    if (type == "Item") await Task.Run(() => task_PopulateItems(), token);
                    if (type == "Mastery") await Task.Run(() => task_PopulateMasteries(), token);
                    if (type == "Minipet") await Task.Run(() => task_PopulateMinipets(), token);
                    if (type == "Recipe") await Task.Run(() => task_PopulateRecipes(), token);
                    if (type == "Skill") await Task.Run(() => task_PopulateSkills(), token);
                    if (type == "Skin") await Task.Run(() => task_PopulateSkins(), token);
                    if (type == "Title") await Task.Run(() => task_PopulateTitles(), token);
                }
            }
            
            await Task.Run(() => Global.FinalizeImport(Global), token);
            
            lbl_loaddetails.Content = "Ready for Loading!";
            lbl_loadid.Content = "";
        }
        private void GWTwoHelper_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < Global.ReadyToExportJSON.Count; i++)
            {
                string type = Global.ReadyToExportJSON[i];
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"json\");

                StringBuilder csv = new StringBuilder();
                csv.Append("[");
                List<KeyValuePair<string, string>> exportList = Global.JSON.Where(j => j.Key == type).ToList();
                for (int j = 0; j < exportList.Count; j++)
                {
                    double cur = j, max = exportList.Count;
                    string line = exportList[j].Value + ((j != exportList.Count - 1) ? "," : "");
                    csv.AppendLine(line);
                    Global.OnCharStatusUpdate("Writing " + type + " JSON file (" + j + " / " + exportList.Count + ") " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
                }
                csv.Append("]");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"json\" + type + ".json", csv.ToString());
            }

            if (Global.SavedCharacters.Count > 0)
            {
                StringBuilder chars = new StringBuilder();
                for (int i = 0; i < Global.SavedCharacters.Count; i++)
                {
                    string line = Global.SavedCharacters[i].CharacterName + ";" + Global.SavedCharacters[i].APIToken;
                    chars.AppendLine(line);
                }
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "characters.csv", chars.ToString());
            }
        }
        public void LoadJSONFromCSV(string type)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"json\" + type + ".json"))
            {
                string json = "[";
                //int totalLines = LinesInFile(AppDomain.CurrentDomain.BaseDirectory + @"json\" + type + ".json");
                //int i = 0;
                
//ContinueJSON:
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"json\" + type + ".json"))
                {
                    Global.OnCharStatusUpdate("Reading " + type + " JSON file");
                    json = reader.ReadToEnd();
//                    for (int x = 0; x < i; x++) reader.ReadLine();

//                    while (!reader.EndOfStream)
//                    {
//                        i++;
//                        double cur = i, max = totalLines;
//                        string line = reader.ReadLine();

//                        Global.JSON.Add(new KeyValuePair<string, string>(type, line));

//                        json += line + ",";

//                        Global.OnCharStatusUpdate("Reading " + type + " JSON file (" + i + " / " + totalLines + ") " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
//                        if (i % 5000 == 0 && i != 0) goto BreakFromRead;
//                    }
//BreakFromRead:
//                    json = json.Remove(json.Length - 1) + "]";
                }
            
                if (type == "Achievement") Achievement.GetAchievementsFromJSON(json, Global);
                if (type == "BackstoryQuestion") BackstoryQuestion.GetQuestionsFromJSON(json, Global);
                if (type == "BackstoryAnswer") BackstoryAnswer.GetAnswersFromJSON(json, Global);
                if (type == "Colour") Colour.GetColoursFromJSON(json, Global);
                if (type == "GuildEmblemBackground") Emblem.GetBackgroundsFromJSON(json, Global);
                if (type == "GuildEmblemForeground") Emblem.GetForegroundsFromJSON(json, Global);
                if (type == "GuildUpgrade") GuildUpgrade.GetUpgradesFromJSON(json, Global);
                if (type == "ItemStat") ItemStat.GetItemStatsFromJSON(json, Global);
                if (type == "Item") Item.GetItemsFromJSON(json, Global);
                if (type == "Mastery") Mastery.GetMasteriesFromJSON(json, Global);
                if (type == "Minipet") MiniPet.GetMiniPetsFromJSON(json, Global);
                if (type == "Recipe") Recipe.GetRecipesFromJSON(json, Global);
                if (type == "Skill") Skill.GetSkillsFromJSON(json, Global);
                if (type == "Skin") Skin.GetSkinsFromJSON(json, Global);
                if (type == "Title") Stuff.Title.GetTitlesFromJSON(json, Global);

                //json = "[";
                //if (i != totalLines) goto ContinueJSON;
            }
        }
        public void charStatusUpdate(object sender, string text)
        {
            Dispatcher.Invoke(() =>
            {
                string[] values = text.Split(new[] { ";" }, StringSplitOptions.None);
                lbl_loaddetails.Content = values[0];
                if (values.Length > 1) lbl_loadid.Content = values[1];
                else lbl_loadid.Content = "";
            });
        }

        public void task_PopulateAchievements()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/achievements");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<List<int>> achieveIDs = JsonConvert.DeserializeObject<List<int>>(html).ChunkBy(Global.MultiPullLimit);
            List<string> jsonList = new List<string>();

            for (int i = 0; i < achieveIDs.Count; i++)
            {
                double cur = i, max = achieveIDs.Count;
                List<int> newAchievements = achieveIDs[i];
                string achievementString = string.Empty;

                for (int j = 0; j < newAchievements.Count; j++)
                {
                    achievementString += newAchievements[j] + ",";
                }
                achievementString = achievementString.Remove(achievementString.Length - 1);

                request = WebRequest.Create("https://api.guildwars2.com/v2/achievements?ids=" + achievementString);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                jsonList.Add(html);
                Global.OnCharStatusUpdate("Getting achievements JSON (" + cur.ToString() + " / " + max.ToString() + ") " + ((cur > 0) ? Math.Round(cur / max, 2) * 100 : 0).ToString() + "%");
                if (Global.TotalLoaded % Global.MultipleSleepCheck == 0 && Global.TotalLoaded != 0)
                {
                    for (int a = 0; a < (Global.SleepTimeInMS / 1000); a++)
                    {
                        Global.OnCharStatusUpdate("Waiting For Sleep Timer: " + ((Global.SleepTimeInMS / 1000) - a) + " seconds remaining");
                        Thread.Sleep(1000);
                    }
                }
                Global.TotalLoaded++;
            }

            for (int i = 0; i < jsonList.Count; i++)
            {
                Achievement.GetAchievementsFromJSON(jsonList[i], Global);
            }

            Global.ReadyToExportJSON.Add("Achievement");
        }
        public void task_PopulateBackstoryQuestions()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/backstory/questions?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            BackstoryQuestion.GetQuestionsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("BackstoryQuestion");
        }
        public void task_PopulateBackstoryAnswers()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/backstory/answers?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            BackstoryAnswer.GetAnswersFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("BackstoryAnswer");
        }
        public void task_PopulateColours()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/colors?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Colour.GetColoursFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("Colour");
        }
        public void task_PopulateGuildEmblemBackgrounds()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/emblem/backgrounds?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Emblem.GetBackgroundsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("GuildEmblemBackground");
        }
        public void task_PopulateGuildEmblemForegrounds()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/emblem/foregrounds?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Emblem.GetForegroundsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("GuildEmblemForeground");
        }
        public void task_PopulateGuildUpgrades()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/guild/upgrades?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            GuildUpgrade.GetUpgradesFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("GuildUpgrade");
        }
        public void task_PopulateItems()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/items");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<List<int>> itemIDs = JsonConvert.DeserializeObject<List<int>>(html).ChunkBy(Global.MultiPullLimit);
            List<string> jsonList = new List<string>();

            for (int i = 0; i < itemIDs.Count; i++)
            {
                double cur = i, max = itemIDs.Count;
                List<int> newItems = itemIDs[i];
                string itemString = string.Empty;

                for (int j = 0; j < newItems.Count; j++)
                {
                    itemString += newItems[j] + ",";
                }
                itemString = itemString.Remove(itemString.Length - 1);

                request = WebRequest.Create("https://api.guildwars2.com/v2/items?ids=" + itemString);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                jsonList.Add(html);
                Global.OnCharStatusUpdate("Getting items JSON (" + cur.ToString() + " / " + max.ToString() + ") " + ((cur > 0) ? Math.Round(cur / max, 2) * 100 : 0).ToString() + "%");
                if (Global.TotalLoaded % Global.MultipleSleepCheck == 0 && Global.TotalLoaded != 0)
                {
                    for (int a = 0; a < (Global.SleepTimeInMS / 1000); a++)
                    {
                        Global.OnCharStatusUpdate("Waiting For Sleep Timer: " + ((Global.SleepTimeInMS / 1000) - a) + " seconds remaining");
                        Thread.Sleep(1000);
                    }
                }
                Global.TotalLoaded++;
            }

            for (int i = 0; i < jsonList.Count; i++)
            {
                Item.GetItemsFromJSON(jsonList[i], Global);
            }
            Global.ReadyToExportJSON.Add("Item");
        }
        public void task_PopulateItemStats()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/itemstats?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            ItemStat.GetItemStatsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("ItemStat");
        }
        public void task_PopulateMasteries()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/masteries?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Mastery.GetMasteriesFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("Mastery");
        }
        public void task_PopulateMinipets()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/minis?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            MiniPet.GetMiniPetsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("Minipet");
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

            List<List<int>> recipeIDs = JsonConvert.DeserializeObject<List<int>>(html).ChunkBy(Global.MultiPullLimit);
            List<string> jsonList = new List<string>();

            for (int i = 0; i < recipeIDs.Count; i++)
            {
                double cur = i, max = recipeIDs.Count;
                List<int> newRecipes = recipeIDs[i];
                string recipeString = string.Empty;

                for (int j = 0; j < newRecipes.Count; j++)
                {
                    recipeString += newRecipes[j] + ",";
                }
                recipeString = recipeString.Remove(recipeString.Length - 1);

                request = WebRequest.Create("https://api.guildwars2.com/v2/recipes?ids=" + recipeString);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                jsonList.Add(html);
                Global.OnCharStatusUpdate("Getting recipes JSON (" + cur.ToString() + " / " + max.ToString() + ") " + ((cur > 0) ? Math.Round(cur / max, 2) * 100 : 0).ToString() + "%");
                if (Global.TotalLoaded % Global.MultipleSleepCheck == 0 && Global.TotalLoaded != 0)
                {
                    for (int a = 0; a < (Global.SleepTimeInMS / 1000); a++)
                    {
                        Global.OnCharStatusUpdate("Waiting For Sleep Timer: " + ((Global.SleepTimeInMS / 1000) - a) + " seconds remaining");
                        Thread.Sleep(1000);
                    }
                }
                Global.TotalLoaded++;
            }

            for (int i = 0; i < jsonList.Count; i++)
            {
                Recipe.GetRecipesFromJSON(jsonList[i], Global);
            }
            Global.ReadyToExportJSON.Add("Recipe");
        }
        public void task_PopulateSkills()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/skills?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Skill.GetSkillsFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("Skill");
        }
        public void task_PopulateSkins()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/skins");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            List<List<int>> skinIDs = JsonConvert.DeserializeObject<List<int>>(html).ChunkBy(Global.MultiPullLimit);
            List<string> jsonList = new List<string>();

            for (int i = 0; i < skinIDs.Count; i++)
            {
                double cur = i, max = skinIDs.Count;
                List<int> newSkins = skinIDs[i];
                string skinString = string.Empty;

                for (int j = 0; j < newSkins.Count; j++)
                {
                    skinString += newSkins[j] + ",";
                }
                skinString = skinString.Remove(skinString.Length - 1);

                request = WebRequest.Create("https://api.guildwars2.com/v2/skins?ids=" + skinString);
                response = request.GetResponse();
                data = response.GetResponseStream();

                html = null;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                jsonList.Add(html);
                Global.OnCharStatusUpdate("Getting skins JSON (" + cur.ToString() + " / " + max.ToString() + ") " + ((cur > 0) ? Math.Round(cur / max, 2) * 100 : 0).ToString() + "%");
                if (Global.TotalLoaded % Global.MultipleSleepCheck == 0 && Global.TotalLoaded != 0)
                {
                    for (int a = 0; a < (Global.SleepTimeInMS / 1000); a++)
                    {
                        Global.OnCharStatusUpdate("Waiting For Sleep Timer: " + ((Global.SleepTimeInMS / 1000) - a) + " seconds remaining");
                        Thread.Sleep(1000);
                    }
                }
                Global.TotalLoaded++;
            }

            for (int i = 0; i < jsonList.Count; i++)
            {
                Skin.GetSkinsFromJSON(jsonList[i], Global);
            }
            Global.ReadyToExportJSON.Add("Skin");
        }
        public void task_PopulateTitles()
        {
            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/titles?ids=all");
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = null;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            Stuff.Title.GetTitlesFromJSON(html, Global);
            Global.ReadyToExportJSON.Add("Title");
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
                lbl_loaddetails.Content = "Ready for Loading!";
                lbl_loadid.Content = "";
            });
        }

        private async void lst_characters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            selectedCharacter = (CharacterListObject)lst_characters.SelectedItem;

            await Task.Run(() => RefreshCharacter(), token);
            timer1.Start();
            timer2.Start();
        }
        public async void RefreshCharacter()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            this._cancelWork = () => { cancellationTokenSource.Cancel(); };
            var token = cancellationTokenSource.Token;
            string charName = selectedCharacter.CharacterName.Replace(" ", "%20");
            string apiToken = selectedCharacter.APIToken;

            await Task.Run(() => character = GetCharacter(charName, apiToken), token);
            lastCharacterRefresh = DateTime.Now;
            nextCharacterRefresh = DateTime.Now.AddMinutes(5);

            TimeSpan time = DateTime.Now - character.CreationDate;
            int TotalMonths = (int)Math.Floor(time.TotalDays / 28.0);
            bool months = (TotalMonths > 0) ? true : false;
            int TotalWeeks = (int)Math.Floor(time.TotalDays / 7.0);
            bool weeks = (months) ? true : ((TotalWeeks > 0) ? true : false);
            bool days = (weeks) ? true : ((time.TotalDays >= 1.0) ? true : false);
            bool hours = (days) ? true : ((time.TotalHours >= 1.0) ? true : false);
            bool minutes = (hours) ? true : ((time.TotalMinutes >= 1.0) ? true : false);

            int Weeks = TotalWeeks - (TotalMonths * 4);
            int Days = time.Days - (TotalWeeks * 7);

            Dispatcher.Invoke(() =>
            {
                journal_name.Content = character.Name;
                string journalEntry = character.Backstory[0].Journal + character.Backstory[1].Journal + " " + character.Backstory[2].Journal + character.Backstory[3].Journal + " " + character.Backstory[4].Journal;
                journalEntry = journalEntry.Replace("<br>", Environment.NewLine);
                journal_entry.Text = journalEntry;

                if (character.Inv.Bags.Count > 0) inv_1.ItemsSource = character.Inv.Bags[0].Contents;
                else inv_1.ItemsSource = null;
                if (character.Inv.Bags.Count > 1) inv_2.ItemsSource = character.Inv.Bags[1].Contents;
                else inv_2.ItemsSource = null;
                if (character.Inv.Bags.Count > 2) inv_3.ItemsSource = character.Inv.Bags[2].Contents;
                else inv_3.ItemsSource = null;

                if (character.PVESkills.Count > 0) skill_pve.ItemsSource = character.PVESkills;
                else skill_pve.ItemsSource = null;
                if (character.PVPSkills.Count > 0) skill_pvp.ItemsSource = character.PVPSkills;
                else skill_pvp.ItemsSource = null;
                if (character.WVWSkills.Count > 0) skill_wvw.ItemsSource = character.WVWSkills;
                else skill_wvw.ItemsSource = null;

                if (character.AccountBank.Items.Count > 0) bank.ItemsSource = character.AccountBank.Items;
                else bank.ItemsSource = null;

                equipment_head.Visibility = Visibility.Collapsed;
                equipment_head_upgrade1.Visibility = Visibility.Collapsed;
                equipment_head_upgrade2.Visibility = Visibility.Collapsed;
                equipment_shoulders.Visibility = Visibility.Collapsed;
                equipment_shoulders_upgrade1.Visibility = Visibility.Collapsed;
                equipment_shoulders_upgrade2.Visibility = Visibility.Collapsed;
                equipment_chest.Visibility = Visibility.Collapsed;
                equipment_chest_upgrade1.Visibility = Visibility.Collapsed;
                equipment_chest_upgrade2.Visibility = Visibility.Collapsed;
                equipment_gloves.Visibility = Visibility.Collapsed;
                equipment_gloves_upgrade1.Visibility = Visibility.Collapsed;
                equipment_gloves_upgrade2.Visibility = Visibility.Collapsed;
                equipment_leggings.Visibility = Visibility.Collapsed;
                equipment_leggings_upgrade1.Visibility = Visibility.Collapsed;
                equipment_leggings_upgrade2.Visibility = Visibility.Collapsed;
                equipment_boots.Visibility = Visibility.Collapsed;
                equipment_boots_upgrade1.Visibility = Visibility.Collapsed;
                equipment_boots_upgrade2.Visibility = Visibility.Collapsed;
                equipment_weapona1.Visibility = Visibility.Collapsed;
                equipment_weapona1_upgrade1.Visibility = Visibility.Collapsed;
                equipment_weapona1_upgrade2.Visibility = Visibility.Collapsed;
                equipment_weapona2.Visibility = Visibility.Collapsed;
                equipment_weapona2_upgrade1.Visibility = Visibility.Collapsed;
                equipment_weapona2_upgrade2.Visibility = Visibility.Collapsed;
                equipment_weaponb1.Visibility = Visibility.Collapsed;
                equipment_weaponb1_upgrade1.Visibility = Visibility.Collapsed;
                equipment_weaponb1_upgrade2.Visibility = Visibility.Collapsed;
                equipment_weaponb2.Visibility = Visibility.Collapsed;
                equipment_weaponb2_upgrade1.Visibility = Visibility.Collapsed;
                equipment_weaponb2_upgrade2.Visibility = Visibility.Collapsed;

                for (int i = 0; i < character.Equips.Equips.Count; i++)
                {
                    Equipment equipment = character.Equips.Equips[i];
                    switch (equipment.EquipSlot)
                    {
                        case Equipment.Slot.Helm:
                            equipment_head.Visibility = Visibility.Visible;
                            equipment_head.Tag = equipment.ID;
                            equipment_head_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_head_upgrade1.Visibility = Visibility.Visible;
                                equipment_head_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_head_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_head_upgrade2.Visibility = Visibility.Visible;
                                equipment_head_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_head_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.Shoulders:
                            equipment_shoulders.Visibility = Visibility.Visible;
                            equipment_shoulders.Tag = equipment.ID;
                            equipment_shoulders_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_shoulders_upgrade1.Visibility = Visibility.Visible;
                                equipment_shoulders_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_shoulders_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_shoulders_upgrade2.Visibility = Visibility.Visible;
                                equipment_shoulders_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_shoulders_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.Coat:
                            equipment_chest.Visibility = Visibility.Visible;
                            equipment_chest.Tag = equipment.ID;
                            equipment_chest_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_chest_upgrade1.Visibility = Visibility.Visible;
                                equipment_chest_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_chest_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_chest_upgrade2.Visibility = Visibility.Visible;
                                equipment_chest_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_chest_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.Gloves:
                            equipment_gloves.Visibility = Visibility.Visible;
                            equipment_gloves.Tag = equipment.ID;
                            equipment_gloves_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_gloves_upgrade1.Visibility = Visibility.Visible;
                                equipment_gloves_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_gloves_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_gloves_upgrade2.Visibility = Visibility.Visible;
                                equipment_gloves_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_gloves_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.Leggings:
                            equipment_leggings.Visibility = Visibility.Visible;
                            equipment_leggings.Tag = equipment.ID;
                            equipment_leggings_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_leggings_upgrade1.Visibility = Visibility.Visible;
                                equipment_leggings_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_leggings_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_leggings_upgrade2.Visibility = Visibility.Visible;
                                equipment_leggings_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_leggings_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.Boots:
                            equipment_boots.Visibility = Visibility.Visible;
                            equipment_boots.Tag = equipment.ID;
                            equipment_boots_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_boots_upgrade1.Visibility = Visibility.Visible;
                                equipment_boots_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_boots_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_boots_upgrade2.Visibility = Visibility.Visible;
                                equipment_boots_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_boots_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.WeaponA1:
                            equipment_weapona1.Visibility = Visibility.Visible;
                            equipment_weapona1.Tag = equipment.ID;
                            equipment_weapona1_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_weapona1_upgrade1.Visibility = Visibility.Visible;
                                equipment_weapona1_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_weapona1_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_weapona1_upgrade2.Visibility = Visibility.Visible;
                                equipment_weapona1_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_weapona1_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.WeaponA2:
                            equipment_weapona2.Visibility = Visibility.Visible;
                            equipment_weapona2.Tag = equipment.ID;
                            equipment_weapona2_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_weapona2_upgrade1.Visibility = Visibility.Visible;
                                equipment_weapona2_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_weapona2_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_weapona2_upgrade2.Visibility = Visibility.Visible;
                                equipment_weapona2_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_weapona2_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.WeaponB1:
                            equipment_weaponb1.Visibility = Visibility.Visible;
                            equipment_weaponb1.Tag = equipment.ID;
                            equipment_weaponb1_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_weaponb1_upgrade1.Visibility = Visibility.Visible;
                                equipment_weaponb1_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_weaponb1_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_weaponb1_upgrade2.Visibility = Visibility.Visible;
                                equipment_weaponb1_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_weaponb1_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                        case Equipment.Slot.WeaponB2:
                            equipment_weaponb2.Visibility = Visibility.Visible;
                            equipment_weaponb2.Tag = equipment.ID;
                            equipment_weaponb2_image.Source = new BitmapImage(new Uri(equipment.EquippedItem.Image));
                            if (equipment.Upgrades.Count > 0)
                            {
                                equipment_weaponb2_upgrade1.Visibility = Visibility.Visible;
                                equipment_weaponb2_upgrade1.Tag = equipment.Upgrades[0].ID;
                                equipment_weaponb2_upgrade1_image.Source = new BitmapImage(new Uri(equipment.Upgrades[0].Image));
                            }
                            if (equipment.Upgrades.Count > 1)
                            {
                                equipment_weaponb2_upgrade2.Visibility = Visibility.Visible;
                                equipment_weaponb2_upgrade2.Tag = equipment.Upgrades[1].ID;
                                equipment_weaponb2_upgrade2_image.Source = new BitmapImage(new Uri(equipment.Upgrades[1].Image));
                            }
                            continue;
                    }
                }

                char_race.Content = character.CharRace.ToString();
                char_gender.Content = character.CharGender.ToString();
                char_profession.Content = character.CharProfession.ToString();
                if (character.CharTitle != null) char_title.Content = character.CharTitle.Name;
                else char_title.Content = "";
                char_level.Content = character.Level;
                char_deaths.Content = character.Deaths;
                char_age.Content = ((months) ? TotalMonths.ToString("D2") + "M:" : "") + ((weeks) ? Weeks.ToString("D2") + "w:" : "") + ((days) ? Days.ToString("D2") + "d:" : "") + ((hours) ? time.Hours.ToString("D2") + "h:" : "") + ((minutes) ? time.Minutes.ToString("D2") + "m:" : "") + time.Seconds.ToString("D2") + "s";

                lbl_loaddetails.Content = "Ready for Loading!";
                lbl_loadid.Content = "";
            });
        }

        public int LinesInFile(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
        
        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Visible;
            int id = int.Parse(((Grid)sender).Tag.ToString());
            Item item = Global.Items.FirstOrDefault(it => it.ID == id);
            string desc = item.Description;
            if (item.ItemType == Item.Type.UpgradeComponent)
            {
                if (item.Details != null) if (((UpgradeItemDetail)item.Details).InfixUpgrades != null) if (((UpgradeItemDetail)item.Details).InfixUpgrades.Buff != null) desc = ((UpgradeItemDetail)item.Details).InfixUpgrades.Buff.Description;
            }
            if (character.Equips.Equips.Where(eq => eq.EquippedItem.ID == id).ToArray().Length > 0) desc += ((!string.IsNullOrEmpty(desc)) ? Environment.NewLine + Environment.NewLine : "") + "Currently equipped in the " + character.Equips.Equips.FirstOrDefault(eq => eq.EquippedItem.ID == id).EquipSlot.ToString() + " slot.";

            tooltip_image.Source = new BitmapImage(new Uri(item.Image));
            tooltip_name.Content = item.Name;
            tooltip_type.Content = item.ItemType.ToString();
            tooltip_description.Text = desc;
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Collapsed;
        }

        private void item_MouseMove(object sender, MouseEventArgs e)
        {
            if (tooltip.Visibility == Visibility.Visible)
            {
                Point point = Mouse.GetPosition(this);
                double x = point.X += 5, y = point.Y -= 25;
                tooltip.Margin = new Thickness(x, y, 0, 0);
            }
        }
        private void skill_MouseEnter(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Visible;
            int id = int.Parse(((Grid)sender).Tag.ToString());
            Skill skill = Global.Skills.FirstOrDefault(sk => sk.ID == id);
            tooltip_image.Source = new BitmapImage(new Uri(skill.Image));
            tooltip_name.Content = skill.Name;
            tooltip_type.Content = "";
            tooltip_description.Text = skill.Description;
        }

        private void skill_MouseLeave(object sender, MouseEventArgs e)
        {
            tooltip.Visibility = Visibility.Collapsed;
        }

        private void skill_MouseMove(object sender, MouseEventArgs e)
        {
            if (tooltip.Visibility == Visibility.Visible)
            {
                Point point = Mouse.GetPosition(this);
                double x = point.X += 5, y = point.Y -= 25;
                tooltip.Margin = new Thickness(x, y, 0, 0);
            }
        }
    }

    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
