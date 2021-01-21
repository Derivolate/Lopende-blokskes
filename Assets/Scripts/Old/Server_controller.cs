//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.Networking.NetworkSystem;

//namespace Assets.Scripts
//{
//    class Server_controller:NetworkBehaviour
//    {
//        public GameObject unit_prefab;
//        public Network_manager network_manager;
//        public List<Connected_player> players = new List<Connected_player>();
//        private void Start()
//        {
//            network_manager.server = this;
//        }
//        //This is called when the server recieves a team_message
//        //It looks up the first available color and assigns it to that player, then responds to the player with that color
//        //Todo: write a proper team distribution
//        public void request_team(NetworkMessage msg)
//        {
//            distribute_team(msg.conn);
//        }
//        public void distribute_team(NetworkConnection conn)
//        {
//            players.Add(new Connected_player((Team)(players.Count - 1), conn));
//            IntegerMessage reply = new IntegerMessage(players.Count - 1);
//            NetworkServer.SendToClient(conn.connectionId, Reference.team_message, reply);
//        }

//        public void recieve_spawn_message(NetworkMessage msg)
//        {
//            for (int i = 0; i < players.Count; i++)
//            {
//                if (players[i].conn.connectionId == msg.conn.connectionId)
//                {
//                    spawn_unit((Team)i, msg.conn);
//                }
//            }
//        }

//        private void spawn_unit(Team team, NetworkConnection auth_conn)
//        {
//            GameObject unit = Instantiate(unit_prefab);
//            Unit_controller controller = unit.GetComponent<Unit_controller>();
//            NetworkServer.SpawnWithClientAuthority(unit, auth_conn);
//            controller.Rpc_init(new Vector3(1, .5f, 1), team);

//        }

//    }
//}
