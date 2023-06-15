using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal class MyKingdomJobs
    {
        public static void init()
        {
            KingdomJob job;
            job = new KingdomJob();
            job.id = C.job_player_kingdom;
            job.addTask("check_culture");
            job.addTask("do_checks");
            AssetManager.job_kingdom.add(job);

        }
    }
}
