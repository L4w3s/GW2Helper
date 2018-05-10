using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<string> Races { get; set; }
        public List<string> Professions { get; set; }
    }
}
