using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Map_piece
    {
        public Map_pieces type;
        public Vector3 position;
        public Vector3 e_rotation;
        public Vector3 scale;
    }
}
