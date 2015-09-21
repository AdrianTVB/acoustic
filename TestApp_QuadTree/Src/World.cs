using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QuadTreeLibrary;
using TestApp_QuadTree.Src.Resources;
using TestApp_QuadTree.Src.Terrains;

namespace TestApp_QuadTree.Src
{
    public class World
    {
        public PlayerList Players { get; set; }
        public List<Region> Regions { get; set; }

        public Tile[,] TileGrid { get; set; }

        public List<Resource> PossibleResources { get; set; }
        public List<Terrain> PossibleTerrain { get; set; }

        public QuadTree<Tile> TileQuadTree { get; private set; }

        public World(int tileGridHeight, int tileGridWidth, List<Resource> resources, List<Terrain> terrain)
        {
            PossibleResources = resources;
            PossibleTerrain = terrain;
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

        public void IncrementDay()
        {
            for(int width = 0; width < TileGrid.GetLength(0); width++)
            {
                for(int height = 0; height < TileGrid.GetLength(1); height++)
                {
                    Tile tile = TileGrid[width, height];
                    if(tile != null)
                    {
                        tile.IncrementDailyChanges();
                    }
                }
            }
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
