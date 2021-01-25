using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    //public class Player_controller : NetworkBehaviour
    public class Player_controller : MonoBehaviour
    {
        //A prefab for the cursor
        public Transform cursor;
        //A prefab for the square that is shown when dragging the mouse to select some units
        public Image selection_square;
        //The camera is already in the scene, so it cannot be set through the inspector and thus needs te be looked up during the initialization
        private Camera main_cam;

        public Game_controller game_controller;
        public World_data world_data;
        public GameObject unit_prefab;

        public GameObject map_cube;
        public GameObject map_slant;
        public GameObject map_corner;
        public GameObject map_peek;
        public GameObject map_stomp;

        public Team team;

        //Units is an array of all the units belonging to this player.
        private Dictionary<int, GameObject> units;
        //A list of all selected units in the scene. Everything in here is also in units
        private List<int> selected_ids = new List<int>();
        //This is used for temporary storage of the initial mouse-click when dragging to select a unit
        private Vector2 selection_square_corner;

        //Things necessary for the minimap
        public Transform minimap;
        public GameObject minimap_unit;
        private List<GameObject> minimap_units = new List<GameObject>();

        private bool update_map = true;
        //All these functions are called by the unityengine
        #region unity functions
        private void Start()
        {

            //Instantiate prefabs and find the camera for later reference
            main_cam = FindObjectOfType<Camera>();
            cursor = Instantiate(cursor);
            selection_square = Instantiate(selection_square, FindObjectOfType<Canvas>().transform);

            units = new Dictionary<int, GameObject>();

            //Initialize the prefabs
            hide_cursor();
            reset_selection_square();
        }

        private void Update()
        {
            update_world();

            update_input();
            update_minimap();

        }
        private void update_input()
        {
            //If the left mouse button is pressed, try selecting a unit
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
                selection_square_corner = Input.mousePosition;
                if (Physics.Raycast(ray, out hit))
                {
                    //If the cursor was indeed on a unit, select that unit
                    if (hit.transform.tag == Reference.unit_tags[(int)team])
                    {
                        hide_cursor();
                        click_select_unit(hit);
                    }
                    //If the cursor was anywhere else, hide teh cursor and deselect all units
                    else
                    {
                        hide_cursor();
                        deselect_all_units();
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                ////Create the selection square

                
                Vector2 mouse_pos = Input.mousePosition;

                //TODO: commenting this will reproduce 'the UI bug'. Has to be fixed
                ////Get the bottom left corner and set the position of the selection square to that corner
                //Vector2 bot_left_corner = new Vector2(
                //    (mouse_pos.x < selection_square_corner.x ? mouse_pos.x : selection_square_corner.x),
                //    (mouse_pos.y < selection_square_corner.y ? mouse_pos.y : selection_square_corner.y));
                //selection_square.rectTransform.position = bot_left_corner;

                //Get the size of the square and set it
                Vector2 size = new Vector2(
                    Mathf.Abs(mouse_pos.x - selection_square_corner.x),
                    Mathf.Abs(mouse_pos.y - selection_square_corner.y));
                selection_square.rectTransform.sizeDelta = size;

                ////Select all the units in the square
                //drag_select_unit(bot_left_corner, bot_left_corner + size);

            }
            else if (Input.GetMouseButtonUp(0))
            {
                //Select the units in the selection square and remove the selection square
                reset_selection_square();
            }
            //If the right mouse button is pressed, try moving the selected the selected point
            if (Input.GetMouseButtonDown(1))
            {
                //If no units are selected, no unit will be moved
                if (selected_ids.Count == 0)
                    return;
                RaycastHit hit;
                Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    //Todo: let the this raycast go through units so it will always hit the 'world'
                    if (hit.transform.tag == "World")
                    {
                        //Set the destination for each selected unit to the clicked location
                        foreach (int id in selected_ids)
                        {
                            game_controller.set_target(id, hit.point);
                            //unit.GetComponent<Unit_controller>().destination = hit.point;
                        }
                        //Move the cursor to the clicked location
                        cursor.transform.position = hit.point + new Vector3(0, .001f, 0);

                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                RaycastHit hit;
                Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
                //This is to prevent units spawning in each client when using multiple clients
                if (Physics.Raycast(ray, out hit))
                {
                    //spawn_block(team);
                    game_controller.create_unit(team, new Vector3(1, .5f, 1));
                }
            }
        }
        private void update_world()
        {
            world_data = game_controller.get_world_data();

            foreach (Unit_data data in world_data.units)
            {
                GameObject unit;
                if (!units.TryGetValue(data.id, out unit))
                {
                    unit = Instantiate(unit_prefab);
                    units.Add(data.id, unit);
                }

                update_unit_data(unit, data);
            }
            if (update_map)
            {
                foreach (Map_piece piece in world_data.map.pieces)
                {
                    GameObject go;
                    switch (piece.type)
                    {
                        case Map_pieces.cube:
                            go = Instantiate(map_cube);
                            break;
                        case Map_pieces.corner:
                            go = Instantiate(map_corner);
                            break;
                        case Map_pieces.peek:
                            go = Instantiate(map_peek);
                            break;
                        case Map_pieces.slant:
                            go = Instantiate(map_slant);
                            break;
                        default: //stomp
                            go = Instantiate(map_stomp);
                            break;
                    }
                    go.transform.rotation = Quaternion.Euler(piece.e_rotation);
                    go.transform.localScale = piece.scale;
                    go.transform.position = piece.position;
                }
                update_map = false;
            }
        }
        #endregion

        private void update_unit_data(GameObject obj, Unit_data data)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            obj.transform.position = data.position;
            obj.transform.rotation = data.rotation;
            rb.velocity = data.velocity;
            rb.angularVelocity = data.angular_velocity;
        }
        private void update_minimap()
        {
            //Clear all units from the minimap
            foreach (GameObject mm_unit in minimap_units)
            {
                Destroy(mm_unit);
            }
            //get minmax positions of real map
            //get minmax positions of minimap
            Vector2 min_real = new Vector2(-5, -5);
            Vector2 max_real = new Vector2(5, 5);
            Vector2 min_mini = new Vector2(0, 0);
            Vector2 max_mini = new Vector2(200, 200);
            Vector2 transl = min_mini - min_real;
            Vector2 scale = (max_mini - min_mini) / (max_real - min_real);

            foreach (GameObject unit in units.Values)
            {
                Vector2 real_pos = new Vector2(unit.transform.position.x, unit.transform.position.z);
                Vector2 mini_pos = (real_pos + transl) * scale;


                Vector3 real_euler_rotation = unit.transform.rotation.eulerAngles;

                GameObject new_mm_unit = Instantiate(minimap_unit, mini_pos, Quaternion.Euler(0, 0, real_euler_rotation.y), minimap);

                minimap_units.Add(new_mm_unit);
            }
        }

        //All network communication stuff is in here
        #region Network functions
        /// <summary>
        /// Sends a message to the server to ask it to spawn a unit for the specified team
        /// </summary>
        /// <param name="team"></param>
        private void spawn_block(Team team)
        {
            //client.Send(Reference.spawn_message, new IntegerMessage());
            //GameObject unit = Instantiate(unit_prefab);
            //Unit_controller controller = unit.GetComponent<Unit_controller>();
            //controller.init(new Vector3(1, .5f, 1), team);
        }

        /// <summary>
        /// Updates the units array. This needs to be called on the player object each time a unit of that team spawns or is destroyed
        /// </summary>
        /*public void update_units()
        {
            units = GameObject.FindGameObjectsWithTag(Reference.unit_tags[(int)team]);
        }*/

        /*public void remove_unit(GameObject unit)
        {
            if (selected_ids.Contains(unit))
                selected_ids.Remove(unit);

            update_units();
        }*/
        #endregion

        //All things concerning unit selection is in here
        #region unit selection functions
        /// <summary>
        /// Moves the cursor to a place far far away from here
        /// </summary>
        private void hide_cursor()
        {
            cursor.transform.position = new Vector3(100000, 100000, 100000);
        }
        private void reset_selection_square()
        {
            selection_square.rectTransform.position = new Vector3();
            selection_square.rectTransform.sizeDelta = new Vector2();
        }

        private void drag_select_unit(Vector2 botleft, Vector2 topright)
        {
            if (!Input.GetKey(KeyCode.LeftShift) && selected_ids.Count > 1)
            {
                deselect_all_units();
            }
            foreach (KeyValuePair<int, GameObject> u in units)
            {
                Vector2 pos = main_cam.WorldToScreenPoint(u.Value.transform.position);
                if (pos.x > botleft.x && pos.x < topright.x && pos.y > botleft.y && pos.y < topright.y)
                {
                    selected_ids.Add(u.Key);
                    u.Value.GetComponent<Unit_controller>().set_selected_color();
                }
            }
        }
        private void click_select_unit(RaycastHit hit)
        {
            //If shift is not pressed, deselect all units
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                deselect_all_units();
            }
            //Loop through all units and check if it's the one that was clicked. If it is the one select it and break the loop.
            foreach (KeyValuePair<int, GameObject> u in units)
            {
                if (u.Value.transform.Equals(hit.transform))
                {
                    selected_ids.Add(u.Key);
                    u.Value.GetComponent<Unit_controller>().set_selected_color();
                    break;
                }
            }
            //hide_cursor();
        }
        /// <summary>
        /// Resets the color of all units and empties selected_units
        /// </summary>
        private void deselect_all_units()
        {
            foreach (GameObject unit in units.Values)
            {
                unit.GetComponent<Unit_controller>().reset_color();
            }
            selected_ids.Clear();
        }
        #endregion


    }
}
