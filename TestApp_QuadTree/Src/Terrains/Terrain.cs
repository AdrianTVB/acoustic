using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src.Terrains
{
    public abstract class Terrain
    {
        private string terrainName;
        private string terrainCode;
        public string TerrainName
        {
            get
            {
                return terrainName;
            }

            set
            {
                terrainName = value;
            }
        }
        public string TerrainCode
        {
            get
            {
                return terrainCode;
            }

            set
            {
                terrainCode = value;
            }
        }
    }
}
