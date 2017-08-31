using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts
{
    public class Main : MonoBehaviour
    {
        public Transform cursor;
        public GameObject cube_prefab;
        public GameObject select_panel;
        public Camera main_cam;

        private List<Unit> units = new List<Unit>();
        private List<Unit> selected_units = new List<Unit>();
        // Use this for initialization
        void Start()
        {
            hide_cursor();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = main_cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //If the left mouse button is pressed, try selecting a unit
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    //If the cursor was indeed on a unit, select that unit
                    if (hit.transform.tag == "Unit")
                    {
                        //hide_cursor();
                        select_unit(hit);
                    }
                    //If the cursor was anywhere else, hide teh cursor and deselect all units
                    else
                    {
                        hide_cursor();
                        deselect_all_units();
                    }
                }
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

                        foreach (Unit unit in selected_units)
                        {
                            unit.destination = hit.point;
                            Debug.Log("Changed destination to " + hit.point.ToString());
                        }
                        cursor.transform.position = hit.point + new Vector3(0, .001f, 0);

                    }
                }
            }

        }

        private void FixedUpdate()
        {
            foreach (Unit unit in units)
            {
                unit.move();
            }
        }
        /// <summary>
        /// Moves the cursor to a place far far away from here
        /// </summary>
        private void hide_cursor()
        {
            cursor.transform.position = new Vector3(100000, 100000, 100000);
        }

        private void select_unit(RaycastHit hit)
        {
            Renderer rend = hit.transform.GetComponent<MeshRenderer>();
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                deselect_all_units();
            }
            foreach (var unit in units)
            {
                if (unit.cube.transform.Equals(hit.transform))
                {
                    selected_units.Add(unit);
                    break;
                }
            }
            foreach (var unit in selected_units)
            {
                unit.set_selected_color();
            }
            hide_cursor();
        }

        /// <summary>
        /// Resets the color of all units and empties selected_units
        /// </summary>
        private void deselect_all_units()
        {
            foreach (var unit in selected_units)
            {
                unit.reset_color();
            }
            selected_units.Clear();
        }

        public void spawn_block(int _team)
        {
            team team = (team)_team;
            units.Add(new Unit(cube_prefab, new Vector3(1, .5f, 1), team));
        }
        public void delete_all_cubes()
        {
            foreach (Unit unit in units)
            {
                unit.destroy();
            }
            units.Clear();
            selected_units.Clear();
        }
        private void OnGUI()
        {

        }
    }

    public enum team
    {
        blue,
        red,
        green,
        yellow,
    }
}
