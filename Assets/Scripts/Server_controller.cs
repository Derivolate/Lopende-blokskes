using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class Server_controller:NetworkBehaviour
    {
        public GameObject unit_prefab;
        public void Cmd_spawn_unit(Team team)
        {

            GameObject unit = Instantiate(unit_prefab);
            unit.GetComponent<Unit_controller>().init(new Vector3(1, .5f, 1), team);
            NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);

        }
    }
}
