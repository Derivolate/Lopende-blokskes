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
        //private NetworkClient client;

        public GameObject unit_prefab;
        public Team team;

        //Units is an array of all the units belonging to this player.
        private GameObject[] units = new GameObject[0];
        //A list of all selected units in the scene. Everything in here is also in units
        private List<GameObject> selected_units = new List<GameObject>();
        //This is used for temporary storage of the initial mouse-click when dragging to select a unit
        private Vector2 selection_square_corner;

        //All these functions are called by the unityengine
        #region unity functions
        private void Start()
        {

            //Instantiate prefabs and find the camera for later reference
            main_cam = FindObjectOfType<Camera>();
            cursor = Instantiate(cursor);
            selection_square = Instantiate(selection_square, FindObjectOfType<Canvas>().transform);

            //Initialize the prefabs
            hide_cursor();
            reset_selection_square();

        }
        private void Update()
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
                //Get the bottom left corner and set the position of the selection square to that corner
                Vector2 bot_left_corner = new Vector2(
                    (mouse_pos.x < selection_square_corner.x ? mouse_pos.x : selection_square_corner.x),
                    (mouse_pos.y < selection_square_corner.y ? mouse_pos.y : selection_square_corner.y));
                selection_square.rectTransform.position = bot_left_corner;

                //Get the size of the square and set it
                Vector2 size = new Vector2(
                    Mathf.Abs(mouse_pos.x - selection_square_corner.x),
                    Mathf.Abs(mouse_pos.y - selection_square_corner.y));
                selection_square.rectTransform.sizeDelta = size;

                //Select all the units in the square
                drag_select_unit(bot_left_corner, bot_left_corner + size);

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
                if (selected_units.Count == 0)
                    return;
                RaycastHit hit;
                Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    //Todo: let the this raycast go through units so it will always hit the 'world'
                    if (hit.transform.tag == "World")
                    {
                        //Set the destination for each selected unit to the clicked location
                        foreach (GameObject unit in selected_units)
                        {
                            unit.GetComponent<Unit_controller>().destination = hit.point;
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
                    spawn_block(team);
                }
            }

        }
        private void FixedUpdate()
        {
            foreach (GameObject unit in units)
            {
                unit.GetComponent<Unit_controller>().move();
            }
        }
        #endregion

        //All network communication stuff is in here
        #region Network functions
        /// <summary>
        /// Sends a message to the server to ask it to spawn a unit for the specified team
        /// </summary>
        /// <param name="team"></param>
        private void spawn_block(Team team)
        {
            //client.Send(Reference.spawn_message, new IntegerMessage());
            GameObject unit = Instantiate(unit_prefab);
            Unit_controller controller = unit.GetComponent<Unit_controller>();
            controller.init(new Vector3(1, .5f, 1), team);
        }

        /// <summary>
        /// Updates the units array. This needs to be called on the player object each time a unit of that team spawns or is destroyed
        /// </summary>
        public void update_units()
        {
            units = GameObject.FindGameObjectsWithTag(Reference.unit_tags[(int)team]);
        }

        public void remove_unit(GameObject unit)
        {
            if (selected_units.Contains(unit))
                selected_units.Remove(unit);

            update_units();
        }
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
            if (!Input.GetKey(KeyCode.LeftShift) && selected_units.Count > 1)
            {
                deselect_all_units();
            }
            foreach (GameObject unit in units)
            {
                Vector2 pos = main_cam.WorldToScreenPoint(unit.transform.position);
                if (pos.x > botleft.x && pos.x < topright.x && pos.y > botleft.y && pos.y < topright.y)
                {
                    selected_units.Add(unit);
                    unit.GetComponent<Unit_controller>().set_selected_color();
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
            foreach (GameObject unit in units)
            {
                if (unit.transform.Equals(hit.transform))
                {
                    selected_units.Add(unit);
                    unit.GetComponent<Unit_controller>().set_selected_color();
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
            foreach (GameObject unit in selected_units)
            {
                unit.GetComponent<Unit_controller>().reset_color();
            }
            selected_units.Clear();
        }
        #endregion


    }
}
