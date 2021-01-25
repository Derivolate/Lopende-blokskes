using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit_data
    {
        public int id { get; }
        public Team team { get; }
        public Vector3 position { get; }
        public Vector3 target { get; }
        public Quaternion rotation { get; }
        public Vector3 velocity { get; }
        public Vector3 angular_velocity { get; }

        public Unit_data(int id, Team team, Vector3 position, Vector3 target,
            Quaternion rotation, Vector3 velocity, Vector3 angular_velocity)
        {
            this.id = id;
            this.team = team;
            this.position = position;
            this.target = target;
            this.rotation = rotation;
            this.velocity = velocity;
            this.angular_velocity = angular_velocity;
        }
    }
}
