using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class Game_controller : MonoBehaviour
    {
        public GameObject unit_prefab;
        public GameObject plane_prefab;

        public float simulation_speed;

        private World_simulator w_simulator;

        // Start is called before the first frame update
        void Start()
        {
            w_simulator = new World_simulator(unit_prefab, plane_prefab, simulation_speed);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            w_simulator.simulate();
        }

        /// <summary>
        /// Creates a unit in the game for the given team at given position
        /// </summary>
        /// <param name="team"></param>
        /// <param name="position"></param>
        /// <returns>Whether allowed to create the unit</returns>
        public bool create_unit(Team team, Vector3 position)
        {
            //TODO: Add logic for authentication when necessary
            GameObject obj = w_simulator.create_unit();
            Unit_simulator us = obj.GetComponent<Unit_simulator>();
            us.init(position, team);

            return true;
        }

        public bool set_target(int id, Vector3 target)
        {
            //TODO: Add logic for authentication
            w_simulator.set_target(id, target);

            return true;
        }

        /// <summary>
        /// Get data about the physical game world
        /// </summary>
        /// <returns></returns>
        public World_data get_world_data()
        {
            //TODO: Add logic for vision restrictions
            return w_simulator.get_world_data();
        }
    }
}