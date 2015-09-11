using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class Region : List<Tile>
    {
        private World world;

        public Region(World world)
        {
            this.world = world;
        }

        public new void Add(Tile newTile)
        {
            world.TileQuadTree.Insert(newTile);
            base.Add(newTile);
        }
    }
}
