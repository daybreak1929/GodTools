using ai.behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodTools.Code
{
    internal static class MyActorTasks
    {
        public static void init()
        {
            BehaviourTaskActor task;

            task = new BehaviourTaskActor();
            task.id = C.task_always_wait;
            task.addBeh(new BehAlwaysWait(5f));
            add(task);

            task = new BehaviourTaskActor();
            task.id = C.task_gt_fight;
            task.fighting = true;
            task.addBeh(new BehCheckGTFight());
            task.addBeh(new BehGoToActorTarget("sameTile", true));
            task.addBeh(new BehRestartTask());
            add(task);


            task = new BehaviourTaskActor();
            task.id = C.task_always_move;
            task.addBeh(new BehAlwaysMove());
            add(task);

            task = new BehaviourTaskActor();
            task.id = C.task_construct_new_building;
            task.addBeh(new BehFindConstructionTile());
            task.addBeh(new BehGoToTileTarget());
            task.addBeh(new BehLookAtTarget("building_target"));
            task.addBeh(new BehAngleAnimation("building_target", "event:/SFX/BUILDINGS/СonstructionBuildingGeneric", 0f, 40f));
            task.addBeh(new BehBuildTargetProgressAndHeal());
            task.addBeh(new BehRandomWait(0.5f, 1f));
            task.addBeh(new BehCheckTargetProgressAndHealth());
            add(task);
        }
        private static void add(BehaviourTaskActor task) { AssetManager.tasks_actor.add(task); }
        class BehAlwaysWait : BehaviourActionActor
        {
            private float time;
            public BehAlwaysWait(float t) { time = t; }
            public override BehResult execute(Actor pObject)
            {
                pObject.timer_action = time;
                pObject.restoreHealth(Toolbox.randomInt(0, 3));
                return BehResult.RepeatStep;
            }
        }
        public class BehCheckGTFight : BehaviourActionActor
        {
            public static Actor target; 
            public override BehResult execute(Actor pActor)
            {
                if (target == null || !target.base_data.alive)
                {
                    target = null;
                    return BehResult.Stop;
                }
                if (!pActor.canAttackTarget(target))
                {
                    if(!pActor._targets_to_ignore.Contains(target)) pActor._targets_to_ignore.Add(target);
                    return BehResult.Stop;
                }
                pActor.attackTarget = target;
                pActor.beh_actor_target = target;
                //Main.log($"{pActor.getName()} Continue");
                return BehResult.Continue;
            }
        }
        public class BehAlwaysMove : BehaviourActionActor
        {
            public override BehResult execute(Actor pActor)
            {
                pActor.data.get("random_steps", out int step, 0);
                pActor.data.get("direction", out int dir_idx, -1);
                ActorDirection dir;
                if (step > 0)
                {
                    if (pActor.beh_tile_target != null && pActor.currentTile != pActor.beh_tile_target)
                    {
                        if(pActor.goTo(pActor.beh_tile_target, true) == ai.ExecuteEvent.True)
                            return BehResult.RestartTask;
                    }
                    dir = Toolbox.directions_all[dir_idx];
                }
                else
                {
                    step = Toolbox.randomInt(1, 60);
                    if (dir_idx < 0)
                    {
                        dir = Toolbox.getRandom<ActorDirection>(Toolbox.directions_all);
                        dir_idx = Toolbox.directions_all.IndexOf(dir);
                    }
                    else
                    {
                        dir = Toolbox.directions_all[dir_idx];
                        dir = Toolbox.getRandom<ActorDirection>(Toolbox.directions_all_turns[dir]);
                        dir_idx = Toolbox.directions_all.IndexOf(dir);
                    }
                    pActor.data.set("direction", dir_idx);
                }
                step--;
                pActor.beh_tile_target = Ant.getNextTile(pActor.currentTile, dir);
                if (pActor.beh_tile_target == null)
                {
                    pActor.data.set("random_steps", 0);
                    pActor.data.set("direction", -1);
                    return BehResult.RepeatStep;
                }
                pActor.data.set("random_steps", step);
                return BehResult.RepeatStep;
            }
        }
        public class BehCheckTargetProgressAndHealth : BehaviourActionActor
        {
            public override void create()
            {
                base.create();
                this.check_building_target_non_usable = true;
                this.null_check_city = true;
                this.null_check_tile_target = true;
            }
            public override BehResult execute(Actor pActor)
            {
                if (pActor.beh_building_target.isUnderConstruction() || pActor.beh_building_target.data.health < pActor.beh_building_target.getMaxHealth()) return BehResult.RestartTask;
                return BehResult.Continue;
            }
        }
        public class BehBuildTargetProgressAndHeal: BehaviourActionActor
        {
            public override void create()
            {
                base.create();
                this.check_building_target_non_usable = true;
                this.null_check_city = true;
                this.null_check_tile_target = true;
            }
            public override BehResult execute(Actor pActor)
            {
                int max_health = pActor.beh_building_target.getMaxHealth();
                if (pActor.beh_building_target.data.health < max_health)
                {
                    pActor.beh_building_target.data.health += 10;
                    if (pActor.beh_building_target.data.health > max_health) pActor.beh_building_target.data.health = max_health;
                    return BehResult.Continue;
                }
                if (!pActor.beh_building_target.isUnderConstruction())
                {
                    return BehResult.Stop;
                }
                pActor.beh_building_target.updateBuild(1);
                return BehResult.Continue;
            }
        }
    }
}
