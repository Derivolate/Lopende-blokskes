using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Map_data
    {
        [SerializeField]
        private Map_pieces _type;
        public Map_pieces type { get { return _type; } }
        [SerializeField]
        private Vector3 _position;
        public Vector3 position { get { return _position; } }
        [SerializeField]
        private Vector3 _e_rotation;
        public Vector3 e_rotation { get { return _e_rotation; } }
        [SerializeField]
        private Vector3 _scale;
        public Vector3 scale { get { return _scale; } }
        //public Map_pieces type { get; }
        //public Vector3 position { get; }
        //public Vector3 e_rotation { get; }
        //public Vector3 scale { get; }
        public Map_data(Map_pieces type, Vector3 position, Vector3 e_rotation, Vector3 scale)
        {
            //this.type = type;
            //this.position = position;
            //this.e_rotation = e_rotation;
            //this.scale = scale;

            this._type = type;
            this._position = position;
            this._e_rotation = e_rotation;
            this._scale = scale;
        }
    }
}
