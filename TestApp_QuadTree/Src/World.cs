using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QuadTreeLibrary;

namespace TestApp_QuadTree.Src
{
    public class World
    {
        public List<Player> Players { get; set; }
        public List<Region> Regions { get; set; }

        public QuadTree<Tile> TileQuadTree { get; private set; }

        public World()
        {
            TileQuadTree = new QuadTree<Tile>(new Size(5, 5), 20);
        }
    }
}
