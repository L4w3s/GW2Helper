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
    public class Guild
    {
        public string ID { get; set; }
        public int Level { get; set; }
        public string MOTD { get; set; }
        public int Influence { get; set; }
        public int Aetherium { get; set; }
        public int Resonance { get; set; }
        public int Favor { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public GuildEmblem GuildEmblemObject { get; set; }

        public static Guild GetGuildFromJSON(string json, Main main)
        {
            GuildRAW guildRAW = JsonConvert.DeserializeObject<GuildRAW>(json);
            Guild newGuild = new Guild
            {
                ID = guildRAW.id,
                Level = (guildRAW.level.HasValue) ? guildRAW.level.Value : 0,
                Influence = (guildRAW.influence.HasValue) ? guildRAW.influence.Value : 0,
                Aetherium = (guildRAW.aetherium.HasValue) ? guildRAW.aetherium.Value : 0,
                Favor = (guildRAW.favor.HasValue) ? guildRAW.favor.Value : 0,
                Name = guildRAW.name
            };
            if (!string.IsNullOrEmpty(guildRAW.motd)) newGuild.MOTD = guildRAW.motd;
            if (!string.IsNullOrEmpty(guildRAW.tag)) newGuild.Tag = guildRAW.tag;

            GuildEmblem newGuildEmblem = new GuildEmblem();
            if (guildRAW.emblem.background != null)
            {
                EmblemImage emblemImage = new EmblemImage();

                for (int i = 0; i < guildRAW.emblem.background.colors.Length; i++)
                {
                    emblemImage.Colours.Add(guildRAW.emblem.background.colors[i]);
                }
                emblemImage.EmblemObject = main.GuildEmblemBackgrounds.FirstOrDefault(em => em.ID == guildRAW.emblem.background.id);

                newGuildEmblem.Background = emblemImage;
            }
            if (guildRAW.emblem.foreground != null)
            {
                EmblemImage emblemImage = new EmblemImage();

                for (int i = 0; i < guildRAW.emblem.foreground.colors.Length; i++)
                {
                    emblemImage.Colours.Add(guildRAW.emblem.foreground.colors[i]);
                }
                emblemImage.EmblemObject = main.GuildEmblemForegrounds.FirstOrDefault(em => em.ID == guildRAW.emblem.foreground.id);

                newGuildEmblem.Background = emblemImage;
            }
            for (int i = 0; i < guildRAW.emblem.flags.Length; i++)
            {
                newGuildEmblem.FlagArray.Add((GuildEmblem.Flags)Enum.Parse(typeof(GuildEmblem.Flags), guildRAW.emblem.flags[i]));
            }

            newGuild.GuildEmblemObject = newGuildEmblem;
            
            main.OnCharStatusUpdate("Generated Guild " + newGuild.Name + ";" + newGuild.ID);
            return newGuild;
        }
    }
    public class GuildEmblem
    {
        public enum Flags
        {
            FlipBackgroundHorizontal,
            FlipBackgroundVertical
        }
        public EmblemImage Background { get; set; }
        public EmblemImage Foreground { get; set; }
        public List<Flags> FlagArray { get; set; }
    }
    public class EmblemImage
    {
        public Emblem EmblemObject { get; set; }
        public List<int> Colours { get; set; }
    }
    public class Emblem
    {
        public int ID { get; set; }
        public List<string> Images { get; set; }
        
        public static void GetBackgroundsFromJSON(string json, Main main)
        {
            EmblemRAW[] rawEmblems = new EmblemRAW[1];
            try
            {
                rawEmblems = JsonConvert.DeserializeObject<EmblemRAW[]>(json);
            }
            catch (Exception e)
            {
                rawEmblems[0] = JsonConvert.DeserializeObject<EmblemRAW>(json);
            }
            for (int a = 0; a < rawEmblems.Length; a++)
            {
                double cur = a, max = rawEmblems.Length;
                EmblemRAW emblemRAW = rawEmblems[a];
                main.JSON.Add(new KeyValuePair<string, string>("GuildEmblemBackground", JsonConvert.SerializeObject(emblemRAW)));
                Emblem newEmblem = new Emblem
                {
                    ID = emblemRAW.id,
                    Images = new List<string>()
                };
                for (int i = 0; i < emblemRAW.layers.Length; i++)
                {
                    string fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = emblemRAW.layers[i].Substring(emblemRAW.layers[i].LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\background\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\background\" + fileName)) client.DownloadFileAsync(new Uri(emblemRAW.layers[i]), AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\background\" + fileName);
                    }

                    newEmblem.Images.Add(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\background\" + fileName);
                }

                main.GuildEmblemBackgrounds.Add(newEmblem);
                main.OnCharStatusUpdate("Generated Emblem Background;" + newEmblem.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
        public static void GetForegroundsFromJSON(string json, Main main)
        {
            EmblemRAW[] rawEmblems = new EmblemRAW[1];
            try
            {
                rawEmblems = JsonConvert.DeserializeObject<EmblemRAW[]>(json);
            }
            catch (Exception e)
            {
                rawEmblems[0] = JsonConvert.DeserializeObject<EmblemRAW>(json);
            }
            for (int a = 0; a < rawEmblems.Length; a++)
            {
                double cur = a, max = rawEmblems.Length;
                EmblemRAW emblemRAW = rawEmblems[a];
                main.JSON.Add(new KeyValuePair<string, string>("GuildEmblemForeground", JsonConvert.SerializeObject(emblemRAW)));
                Emblem newEmblem = new Emblem
                {
                    ID = emblemRAW.id,
                    Images = new List<string>()
                };
                for (int i = 0; i < emblemRAW.layers.Length; i++)
                {
                    string fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = emblemRAW.layers[i].Substring(emblemRAW.layers[i].LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\foreground\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\foreground\" + fileName)) client.DownloadFileAsync(new Uri(emblemRAW.layers[i]), AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\foreground\" + fileName);
                    }

                    newEmblem.Images.Add(AppDomain.CurrentDomain.BaseDirectory + @"images\emblems\foreground\" + fileName);
                }

                main.GuildEmblemForegrounds.Add(newEmblem);
                main.OnCharStatusUpdate("Generated Emblem Foreground;" + newEmblem.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }

    class GuildRAW
    {
        public int? level { get; set; }
        public string motd { get; set; }
        public int? influence { get; set; }
        public int? aetherium { get; set; }
        public int? resonance { get; set; }
        public int? favor { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public GuildEmblemRAW emblem { get; set; }
    }
    class GuildEmblemRAW
    {
        public EmblemImageRAW background { get; set; }
        public EmblemImageRAW foreground { get; set; }
        public string[] flags { get; set; }
    }
    class EmblemImageRAW
    {
        public int id { get; set; }
        public int[] colors { get; set; }
    }
    class EmblemRAW
    {
        public int id { get; set; }
        public string[] layers { get; set; }
    }
}
