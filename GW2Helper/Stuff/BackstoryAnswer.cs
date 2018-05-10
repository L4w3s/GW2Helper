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
        public List<string> Professions { get; set; }
        public List<string> Races { get; set; }
    }
}
