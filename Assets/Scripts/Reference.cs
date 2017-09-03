using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{

    public static class Reference
    {
        public static float selected_color_addition = .2f;
        public static float max_velocity_squared = 10;
        public static float max_velocity = Mathf.Sqrt(max_velocity_squared);

        public static short team_message = 100;
        public static short spawn_message = 101;

        public static Color[] colors = { Color.blue, Color.red, Color.yellow, Color.green };
        public static string[] unit_tags = { "Unit_blue", "Unit_red", "Unit_yellow", "Unit_green" };
        public static string[] player_tags = { "Player_blue", "Player_red", "Unit_yellow", "Unit_green" };
    }
    public enum Team
    {
        blue,
        red,
        yellow,
        green,
    }
}
