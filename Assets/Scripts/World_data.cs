using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class World_data
    {
        [SerializeField]
        private List<Map_data> _map;
        public List<Map_data> map { get { return _map; } }
        [SerializeField]
        private Unit_data[] _units;
        public Unit_data[] units { get { return _units; } }

        public World_data(Unit_data[] units, List<Map_data> map)
        {
            this._map = map;
            this._units = units;
        }
        public World_data(List<Map_data> map)
        {
            this._map = map;
        }
        public World_data(Unit_data[] units)
        {
            this._units = units;
            
        }
    }
}
