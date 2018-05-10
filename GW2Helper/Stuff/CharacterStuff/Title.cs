using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff.CharacterStuff
{
    public class Title
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Achievement> Achievements { get; set; }
        public int PointRequirement { get; set; }
    }
}
