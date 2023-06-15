using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Extension
{
    internal static class BaseSystemDataExtesion
    {
        public static void change(this BaseSystemData data, string key, float value, float min=-999, float max=999)
        {
			data.get(key, out float num, 0);
			num += value;
			if (num < min)
			{
				num = min;
			}
			if (num > max)
			{
				num = max;
			}
			data.set(key, num);
		}
    }
}
