using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    class Server_controller:MonoBehaviour
    {
        private List<int> team_ids = new List<int>();
        public GameObject unit_prefab;
        private void Start()
        {
            NetworkServer.RegisterHandler(Reference.team_message, request_team);
            NetworkServer.RegisterHandler(Reference.spawn_message, spawn_unit);
        }
        //This is called when the server recieves a team_message
        //It looks up the first available color and assigns it to that player, then responds to the player with that color
        private void request_team(NetworkMessage msg)
        {
            team_ids.Add(msg.conn.connectionId);
            IntegerMessage reply = new IntegerMessage(team_ids.Count - 1);
            NetworkServer.SendToClient(msg.conn.connectionId, Reference.team_message, reply);

        }
        
        private void spawn_unit(NetworkMessage msg)
        {
            GameObject unit = Instantiate(unit_prefab);
            for (int i = 0; i < team_ids.Count; i++)
            {
                if (team_ids[i] == msg.conn.connectionId)
                {
                    unit.GetComponent<Unit_controller>().init(new Vector3(1, .5f, 1), (Team)i);
                    NetworkServer.SpawnWithClientAuthority(unit, msg.conn);
                }
            }
        }

    }
}
