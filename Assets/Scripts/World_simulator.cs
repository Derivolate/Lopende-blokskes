using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class World_simulator
    {
        private float simulation_speed;

        private Scene simulation_scene;
        private PhysicsScene physics_scene;

        Dictionary<int, Unit_simulator> units;
        private int next_unit_id;

        public GameObject unit_prefab;

        public World_simulator(GameObject unit_prefab, GameObject plane_prefab, float simulation_speed = 0.05F)   // No idea what this speed should be
        {
            this.unit_prefab = unit_prefab;

            units = new Dictionary<int, Unit_simulator>();
            next_unit_id = 0;

            this.simulation_speed = simulation_speed;

            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            simulation_scene = SceneManager.CreateScene("physics_simulator", csp);
            physics_scene = simulation_scene.GetPhysicsScene();

            create_object(plane_prefab);

            start();
        }

        public void start()
        {
            Physics.autoSimulation = true;
        }

        public void pause()
        {
            Physics.autoSimulation = false;
        }

        public void simulate()
        {
            physics_scene.Simulate(simulation_speed);
        }

        private GameObject create_object(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(obj, simulation_scene);

            return obj;
        }

        public GameObject create_unit()
        {
            GameObject unit = create_object(unit_prefab);
            Unit_simulator sim = unit.AddComponent<Unit_simulator>();

            units.Add(next_unit_id, sim);
            next_unit_id++;

            return unit;
        }

        public void set_target(int id, Vector3 target)
        {
            Unit_simulator unit;
            units.TryGetValue(id, out unit);
            unit.set_target(target);
        }

        public World_data get_world_data()
        {
            Unit_data[] unit_data = units.Select(p => p.Value.get_unit_data(p.Key)).ToArray();

            return new World_data(unit_data);
        }
    }
}
