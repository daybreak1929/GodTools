using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GodTools.Game
{
    internal class Game
    {
        public bool is_running { get; private set; }
        public static Game instance;
        public static NormalData normal_data;
        public static CurrentData current_data;
        
        private GameObject m_gameobject;
        private OperateConsole console;
        public InputController input_controller;
        public Kingdom player;

        internal void init()
        {
            instance = this;
            normal_data = new();
            normal_data.init();
            current_data = new();
            m_gameobject = new GameObject("GT Game");
            m_gameobject.transform.SetParent(CanvasMain.instance.canvas_ui.transform);
            m_gameobject.transform.localPosition = Vector3.zero;
            m_gameobject.transform.localScale = new Vector3(1, 1);
            input_controller = new InputController();
            input_controller.init();
            console = new GameObject("Console", typeof(OperateConsole)).GetComponent
                <OperateConsole>();
            console.transform.SetParent(m_gameobject.transform);
            console.init();
            m_gameobject.SetActive(false);
        }
        public void start()
        {
            this.is_running = true;
            current_data.reset(normal_data);
            m_gameobject.SetActive(true);
            console.active();
        }

        internal void update(float elapsed)
        {
            if (!is_running) return;
            input_controller.update(elapsed);
            console.update(elapsed);
        }
        public void end()
        {
            this.is_running = false;
            input_controller.disable();
            m_gameobject.SetActive(false);
        }
        public void init_curr_kingdom()
        {
            player.ai.nextJobDelegate = new GetNextJobID(() => { return C.job_player_kingdom; });
            player.capital.ai.nextJobDelegate = new GetNextJobID(() => { return C.job_player_city; });
            player.capital.ai.single_actions = new List<SingleAction<BehaviourTaskCity, BehaviourActionCity>>();
        }
    }
}
