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
    public class MiniPet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Unlock { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public Item UnlockItem { get; set; }
        public int ItemID { get; set; }
        
        public static void GetMiniPetsFromJSON(string json, Main main)
        {
            MiniPetRAW[] rawMiniPets = new MiniPetRAW[1];
            try
            {
                rawMiniPets = JsonConvert.DeserializeObject<MiniPetRAW[]>(json);
            }
            catch (Exception e)
            {
                rawMiniPets[0] = JsonConvert.DeserializeObject<MiniPetRAW>(json);
            }
            for (int m = 0; m < rawMiniPets.Length; m++)
            {
                double cur = m, max = rawMiniPets.Length;
                MiniPetRAW miniPetRAW = rawMiniPets[m];
                main.JSON.Add(new KeyValuePair<string, string>("Minipet", JsonConvert.SerializeObject(miniPetRAW)));
                MiniPet newMiniPet = new MiniPet
                {
                    ID = miniPetRAW.id,
                    Name = miniPetRAW.name,
                    Unlock = miniPetRAW.unlock,
                    Order = miniPetRAW.order,
                    ItemID = miniPetRAW.item_id
                };
                
                string fileName = string.Empty;
                using (WebClient client = new WebClient())
                {
                    fileName = miniPetRAW.icon.Substring(miniPetRAW.icon.LastIndexOf("/") + 1);
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\");
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName)) client.DownloadFileAsync(new Uri(miniPetRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName);
                }
                newMiniPet.Image = AppDomain.CurrentDomain.BaseDirectory + @"images\minipets\" + fileName;

                main.Minis.Add(newMiniPet);
                main.OnCharStatusUpdate("Generated Minipet " + newMiniPet.Name + ";" + newMiniPet.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
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
