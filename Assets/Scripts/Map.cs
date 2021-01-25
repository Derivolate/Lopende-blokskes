using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Serialisable wrapper class for a list containing all map pieces so that the unity jsontools can easily serialize it
    /// </summary>
    [Serializable]
    public class Map
    {
        public List<Map_piece> pieces = new List<Map_piece>();

        public void Add(Map_piece piece)
        {
            pieces.Add(piece);
        }
    }
}
