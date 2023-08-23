using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal static class MyCityJobs
    {
        public static void init()
        {
            JobCityAsset job;
            job = new JobCityAsset();
            job.id = C.job_player_city;
            job.addTask("check_culture");
            job.addTask("random_wait_test");
            //job.addTask("check_pop_points");
            //job.addTask("do_checks");
            AssetManager.job_city.add(job);
        }
    }
}
