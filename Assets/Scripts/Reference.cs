using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{

    public static class Reference
    {
        public static Color blue_color = Color.blue;
        public static Color red_color = Color.red;
        public static Color yellow_color = Color.yellow;
        public static Color green_color = Color.green;

        public static float selected_color_addition = .2f;
        public static float max_velocity_squared = 10;
        public static float max_velocity = Mathf.Sqrt(max_velocity_squared);

        public static short team_message = 100;
        public static short spawn_message = 101;
    }
}
