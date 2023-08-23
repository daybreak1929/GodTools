using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal static class MyActorJobs
    {
        public static void init()
        {
            ActorJob job;

            job = new ActorJob();
            job.id = C.wait;
            job.addTask(C.task_always_wait);
            add(job);

            job = new ActorJob();
            job.id = C.attack_unit;
            job.addTask(C.task_gt_fight);
            job.addTask("end_job");
            add(job);

            job = new ActorJob();
            job.id = C.explore;
            job.addTask(C.task_always_move);
            add(job);
        }
        private static void add(ActorJob job) { AssetManager.job_actor.add(job); }

    }
}
