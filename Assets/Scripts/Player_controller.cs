using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace Assets.Scripts
{
    public class Player_controller : NetworkBehaviour
    {
        public Transform cursor;
        public Image selection_square;
        private Camera main_cam;
        private Team team;
        private uint netid;
        //Units is an array of all the units belonging to this player.
        private GameObject[] units = new GameObject[0];
        private List<GameObject> selected_units = new List<GameObject>();
        private Server_controller server_controller = new Server_controller();


        private Vector2 selection_square_corner;
        #region unity functions
        private void Awake()
        {

        }
        void Start()
        {
            main_cam = FindObjectOfType<Camera>();
            cursor = Instantiate(cursor);
            selection_square = Instantiate(selection_square, FindObjectOfType<Canvas>().transform);

            hide_cursor();
            reset_selection_square();
        }


        void Update()
        {
            Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //If the left mouse button is pressed, try selecting a unit
            if (Input.GetMouseButtonDown(0))
            {
                selection_square_corner = Input.mousePosition;
                if (Physics.Raycast(ray, out hit))
                {
                    //If the cursor was indeed on a unit, select that unit
                    if (hit.transform.tag == "Unit_blue")
                    {
                        //hide_cursor();
                        click_select_unit(hit);
                    }
                    //If the cursor was anywhere else, hide teh cursor and deselect all units
                    else
                    {
                        //hide_cursor();
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

                if (Physics.Raycast(ray, out hit))
                {
                    //TODO: let the this raycast go through units so it will always hit the 'world'
                    if (hit.transform.tag == "World")
                    {

                        foreach (GameObject unit in selected_units)
                        {
                            unit.GetComponent<Unit_controller>().destination = hit.point;
                        }
                        cursor.transform.position = hit.point + new Vector3(0, .001f, 0);

                    }
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                spawn_block(0);
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

        /// <summary>
        /// Moves the cursor to a place far far away from here
        /// </summary>
        #region unit selection methods
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
        private void OnConnectedToServer()
        {
            netid = GetComponent<NetworkIdentity>().netId.Value;
            server_controller.Cmd_register_player(netid);
        }


        private void spawn_block(Team team)
        {
            server_controller.Cmd_spawn_unit(team);
            units = GameObject.FindGameObjectsWithTag("Unit_blue");
        }

    }
}
