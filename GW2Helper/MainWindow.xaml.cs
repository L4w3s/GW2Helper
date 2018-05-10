using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        public Character ObtainCharacterInformation(string URL)
        {
            WebRequest request = WebRequest.Create(URL);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Character>(html);
        }
    }
}
