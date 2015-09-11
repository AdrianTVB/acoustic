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
        public PlayerList Players { get; set; }
        public List<Region> Regions { get; set; }

        public Tile[,] TileGrid { get; set; }

        public QuadTree<Tile> TileQuadTree { get; private set; }

        public World(int tileGridHeight, int tileGridWidth)
        {
            Players = new PlayerList();
            Regions = new List<Region>();
            TileQuadTree = new QuadTree<Tile>(new Size(5, 5), 20);
            TileGrid = new Tile[tileGridHeight, tileGridWidth];
        }

        public bool AddTile(Tile tile)
        {
            if(TileGrid[tile.CoordinateX, tile.CoordinateY] != null)
            {
                return false;
            }

            TileGrid[tile.CoordinateX, tile.CoordinateY] = tile;
            TileQuadTree.Insert(tile);
            return true;
        }

        public class PlayerList : List<Player>
        {
            public new void Add(Player newPlayer)
            {
                foreach(Player existingPlayer in this)
                {
                    existingPlayer.IsHostile.Add(newPlayer, true);
                    newPlayer.IsHostile.Add(existingPlayer, true);
                }

                base.Add(newPlayer);
            }
        }
    }
}
