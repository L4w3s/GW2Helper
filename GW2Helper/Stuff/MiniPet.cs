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
    public class MiniPet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Unlock { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public Item UnlockItem { get; set; }

        public static MiniPet GetMiniPetFromJSON(string json, Main main)
        {
            MiniPetRAW miniPetRAW = JsonConvert.DeserializeObject<MiniPetRAW>(json);
            MiniPet newMiniPet = new MiniPet
            {
                ID = miniPetRAW.id,
                Name = miniPetRAW.name,
                Unlock = miniPetRAW.unlock,
                Order = miniPetRAW.order
            };

            int itemID = miniPetRAW.item_id;

            WebRequest request = WebRequest.Create("https://api.guildwars2.com/v2/items/" + itemID);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            Item item = (main.Items.Where(it => it.ID == itemID).ToArray().Length > 0) ? main.Items.FirstOrDefault(it => it.ID == itemID) : Item.GetItemFromJSON(html, main);
            newMiniPet.UnlockItem = item;

            string fileName = string.Empty;
            using (WebClient client = new WebClient())
            {
                fileName = miniPetRAW.icon.Substring(miniPetRAW.icon.LastIndexOf("/") + 1);
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\");
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName)) client.DownloadFileAsync(new Uri(miniPetRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName);
            }
            newMiniPet.Image = AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName;

            return newMiniPet;
        }
    }

    class MiniPetRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public string unlock { get; set; }
        public string icon { get; set; }
        public int order { get; set; }
        public int item_id { get; set; }
    }
}
