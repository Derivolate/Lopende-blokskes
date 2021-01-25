using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class World_data
    {
        public Unit_data[] units { get; }

        public World_data(Unit_data[] units)
        {
            this.units = units;
        }
    }
}
