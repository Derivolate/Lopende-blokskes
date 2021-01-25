﻿using System;
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

        private GameObject map_cube;
        private GameObject map_slant;
        private GameObject map_corner;
        private GameObject map_peek;
        private GameObject map_stomp;

        private Map map;

        private Scene simulation_scene;
        private PhysicsScene physics_scene;
        private bool simulation_enabled;

        Dictionary<int, Unit_simulator> units;
        private int next_unit_id;

        public GameObject unit_prefab;

        public World_simulator(GameObject unit_prefab, GameObject plane_prefab, GameObject map_cube, GameObject map_slant, GameObject map_corner, GameObject map_peek, GameObject map_stomp, float simulation_speed = 0.05F)   // No idea what this speed should be
        {
            this.unit_prefab = unit_prefab;
            this.map_cube = map_cube;
            this.map_slant = map_slant;
            this.map_corner = map_corner;
            this.map_peek = map_peek;
            this.map_stomp = map_stomp;

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
            simulation_enabled = true;
        }

        public void pause()
        {
            simulation_enabled = false;
        }

        public void simulate()
        {
            if (simulation_enabled)
                physics_scene.Simulate(simulation_speed);
        }

        public void load_map(TextAsset map_file)
        {
            map = JsonUtility.FromJson<Map>(map_file.text);

            Map_piece test_piece = new Map_piece();

            foreach (Map_piece piece in map.pieces)
            {
                GameObject go;
                switch (piece.type)
                {
                    case Map_pieces.cube:
                        go = create_object(map_cube);
                        break;
                    case Map_pieces.corner:
                        go = create_object(map_corner);
                        break;
                    case Map_pieces.peek:
                        go = create_object(map_peek);
                        break;
                    case Map_pieces.slant:
                        go = create_object(map_slant);
                        break;
                    default: //stomp
                        go = create_object(map_stomp);
                        break;
                }
                go.transform.rotation = Quaternion.Euler(piece.e_rotation);
                go.transform.localScale = piece.scale;
                go.transform.position = piece.position;
                //Disable the meshrenderer in the simulation
                go.GetComponent<MeshRenderer>().enabled = false;
            }
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

            return new World_data(unit_data, map);
        }
    }
}