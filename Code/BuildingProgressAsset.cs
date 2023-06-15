using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GodTools.Code.Delegates;

namespace GodTools.Code
{
    internal class BuildingProgressAsset : Asset
    {
        public BuildingProgressAction begin_action;
        public BuildingProgressAction end_action;
        public float progress;
        public float prio;
        public string translate_key;
        public Func<string,string,float,string> translate_action;
    }
}
