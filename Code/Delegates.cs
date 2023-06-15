using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    public class Delegates
    {
        public delegate bool BuildingProgressAction(Building building, string custom_str, float custom_val);
        public static string wait_for_cmd_at_next_job() { return C.wait; }
    }
}
