using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit_simulator : MonoBehaviour
    {
        private Team team;

        private Rigidbody rb;
        private Vector3 target;

        public void Start()
        {
            this.rb = this.gameObject.GetComponent<Rigidbody>();
            this.target = new Vector3(0, .25f, 0);
        }

        public void FixedUpdate()
        {
            Vector3 direction = -(transform.position - target);
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
                    rb.AddForce(direction.normalized * .5f, ForceMode.VelocityChange);
                }
            }
        }

        public void init(Vector3 coords, Team _team)
        {
            transform.position = coords;
            team = _team;
        }

        public void set_team(Team team)
        {
            this.team = team;
        }

        public void set_target(Vector3 target)
        {
            this.target = target;
        }

        public Unit_data get_unit_data(int id)
        {
            return new Unit_data(id, team, transform.position,
                target, transform.rotation,
                rb.velocity, rb.angularVelocity);
        }
    }
}
