//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;
//namespace Assets.Scripts
//{
//    class Network_manager : NetworkManager
//    {

//        public Server_controller server { get; set; }
//        public void Start()
//        {
//            Time.timeScale = 1.0f;
//        }
//        #region server methods
//        // called when a client connects 
//        public override void OnServerConnect(NetworkConnection conn)
//        {
            
//        }

//        // called when a client disconnects
//        public override void OnServerDisconnect(NetworkConnection conn)
//        {
//            NetworkServer.DestroyPlayersForConnection(conn);
//        }

//        // called when a client is ready
//        public override void OnServerReady(NetworkConnection conn)
//        {
//            NetworkServer.SetClientReady(conn);
//            NetworkServer.RegisterHandler(Reference.team_message, server.request_team);
//            NetworkServer.RegisterHandler(Reference.spawn_message, server.recieve_spawn_message);
//        }

//        // called when a new player is added for a client
//        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
//        {
//            var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(), Quaternion.identity);
//            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
//        }

//        // called when a player is removed for a client
//        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
//        {
//            NetworkServer.Destroy(player.gameObject);
//        }

//        // called when a network error occurs
//        public override void OnServerError(NetworkConnection conn, int errorCode)
//        {

//        }
//        #endregion
//        #region client methods
//        // called when connected to a server
//        public override void OnClientConnect(NetworkConnection conn)
//        {
//            ClientScene.Ready(conn);
//            ClientScene.AddPlayer(0);
//        }

//        // called when disconnected from a server
//        public override void OnClientDisconnect(NetworkConnection conn)
//        {
//            Destroy(GameObject.FindGameObjectWithTag("Cursor"));
//            Destroy(GameObject.FindGameObjectWithTag("Selection_square"));
//            StopClient();
//        }

//        // called when a network error occurs
//        public override void OnClientError(NetworkConnection conn, int errorCode)
//        {

//        }

//        // called when told to be not-ready by a server
//        public override void OnClientNotReady(NetworkConnection conn)
//        {

//        }
//        #endregion

//    }
//}
