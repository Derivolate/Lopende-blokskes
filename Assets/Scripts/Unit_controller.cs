using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Assets.Scripts
{
    //public class Unit_controller:NetworkBehaviour
    public class Unit_controller:MonoBehaviour
    {
        public Team team;
        private Renderer rend;
        private Color base_color;


        private void Awake()
        {
            rend = GetComponent<Renderer>();
        }


        public void init(Vector3 coords, Team _team)
        {
            transform.position = coords;
            team = _team;
        }

        private void Start()
        {
            base_color = Reference.colors[(int)team];
            tag = Reference.unit_tags[(int)team];
            reset_color();
        }

        #region coloring methods
        public void reset_color()
        {
            set_color(base_color);
        }
        public void set_selected_color()
        {
            set_color(new Color(
                base_color.r + Reference.selected_color_addition,
                base_color.g + Reference.selected_color_addition,
                base_color.b + Reference.selected_color_addition));
        }
        public void set_color(Color color)
        {
            rend.material.color = color;
        }
        #endregion

    }
}
