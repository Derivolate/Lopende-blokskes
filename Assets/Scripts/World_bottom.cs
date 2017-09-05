using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class World_bottom : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                other.gameObject.GetComponent<Unit_controller>().destroy();
            }
            catch
            {
                Debug.LogWarning("Something else then a block fell through the world bottom. This is not supposed to happen.");
            }
        }
    }
}
