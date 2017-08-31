﻿using UnityEngine;
using System.Collections;

namespace Assets.scripts
{

    public class Unit
    {
        public GameObject cube;
        public string name;
        public int health;
        public team team;
        public Vector3 destination;
        private Renderer rend;
        private Rigidbody rb;
        private Color base_color;
        
        public Unit(GameObject _cube, Vector3 coords, team _team) :this(_cube,coords,_team, new Vector3(123,456,789)){}
        public Unit(GameObject _cube, Vector3 coords, team _team, Vector3 _destination)

        {

            cube = Object.Instantiate(_cube);
            rb = cube.GetComponent<Rigidbody>();
            rend = cube.GetComponent<Renderer>();

            cube.tag = "Unit";
            cube.transform.position = coords;

            team = _team;
            switch (team)
            {
                case team.blue:
                    base_color = Reference.blue_color;
                    break;
                case team.red:
                    base_color = Reference.red_color;
                    break;
                case team.green:
                    base_color = Reference.green_color;
                    break;
                case team.yellow:
                    base_color = Reference.yellow_color;
                    break;
            }

            if (_destination == new Vector3(123, 456, 789))
            {
                _destination = coords;
            }
            else
            {
                destination = _destination;
            }
            reset_color();
        }
        /// <summary>
        /// Must be called every fixed update. Moves towards the set destination
        /// </summary>
        public void move()
        {

            Vector3 direction = -(cube.transform.position - destination);
            direction.y = 0;

            //The cube has reached it's destination so it's horizontal velocity is set to 0 and return the function. It's vertical velocity is kept intact so it can still be affected by gravity.
            //It's destinatian is still set so if it is moved away from it's destination it will automatically move to it's destination again.
            if ((direction).sqrMagnitude < .01)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                return;
            }

            //If the cube is already at max velocity then only the direction needs to be changed.
            //This is done by taking the orthogonal of the current velocity vector in the x-z plane and projecting the normalized direction vector onto it. 
            //This vector is added as acceleration to the cube.
            //The second condition is to prevent it from making u-turns.
            if (rb.velocity.sqrMagnitude > Reference.max_velocity_squared && Mathf.Abs(Vector3.Angle(rb.velocity, direction)) < 90)
            {
                Vector3 orth_velocity = new Vector3(rb.velocity.z, 0, -rb.velocity.x);
                Vector3 steering = Vector3.Project(direction.normalized, orth_velocity) * 20;
                rb.AddForce(steering, ForceMode.Acceleration);
                //This is a bandaid to make sure it stays at max velocity instead. It shouldn't be necessary in this function but with it the cubes move smoother. Don't fix what isn't broken :)
                rb.velocity = rb.velocity.normalized * Reference.max_velocity;
            }
            //If the cube isn't at max speed or is moving away from it's destination, add an accelaration to it to steer it in the right direction
            else
            {
                if (direction.sqrMagnitude > 1)
                {
                    rb.AddForce(direction.normalized * 10, ForceMode.Acceleration);
                }
                //If the cube is extremely close to it's destination, slow down so we don't pass right by it.
                else
                {
                    rb.velocity = rb.velocity * .7f;
                    rb.AddForce(direction.normalized * 1, ForceMode.VelocityChange);
                }
            }
        }

        public void destroy()
        {
            Object.Destroy(cube);
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