using SpaceTimeContinuum.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp_QuadTree.Src;
using TestApp_QuadTree.Src.Resources;
using TestApp_QuadTree.Src.Terrains;

namespace SpaceTimeContinuum
{
    class Program
    {
        static void Main(string[] args)
        {
            World world = ConstructNewZealand();
            Core core = new Core(world);
            core.Run();
        }

        static World ConstructNewZealand()
        {

            string[] regions = new string[]
            {
                "Hawkes Bay",
                "Auckland",
                "New Plymouth",
                "Wellington",
                "Nelson",
                "Marlborough",
                "Westland",
                "Canterbury",
                "Otago",
                "Southland"
            };

            List<Resource> availableResources = new List<Resource>
            {
                new Wood(),
                new Stone(),
                new Moa(),
                new Chicken()
            };

            List<Terrain> availableTerrain = new List<Terrain>
            {
                new Grassland(),
                new Mountains(), 
                new Forest(), 
                new Marsh(),
            };

            int maxHeight = 30;
            int maxWidth = 30;

            World world = new World(maxHeight, maxWidth, availableResources, availableTerrain);
            
            Random randomNumberGenerator = new Random();
            for(int i = 0; i < regions.Length; i++)
            {
                string regionName = regions[i];
                Region region = new Region(world);
                world.Regions.Add(region);
                for (int tileCounter = 0; tileCounter < 100; tileCounter++)
                {
                    int tileWidth = randomNumberGenerator.Next(1, 29);
                    int tileHeight = randomNumberGenerator.Next(1, 29);
                    if (world.TileGrid[tileWidth, tileHeight] == null)
                    {
                        int terrainIndex = randomNumberGenerator.Next(0, availableTerrain.Count);
                        Terrain terrain = availableTerrain[terrainIndex];
                        int resourceIndex = randomNumberGenerator.Next(0, availableResources.Count);
                        Resource resource = availableResources[resourceIndex];

                        Tile tile = new Tile(tileWidth, tileHeight, terrain, resource, 5, region);
                        region.Add(tile);
                    }
                }
            }



            Player playerOne = new Player("Player One", 'X');
            world.Players.Add(playerOne);
            Tile armyOneStartTile = null;
            Tile armyTwoStartTile = null;
            for (int width = 0; width < world.TileGrid.GetLength(0); width++)
            {
                for (int height = 0; height < world.TileGrid.GetLength(1); height++)
                {
                    if (world.TileGrid[width, height] != null)
                    {
                        if (armyOneStartTile == null)
                        {
                            armyOneStartTile = world.TileGrid[width, height];
                        }
                        else if(armyTwoStartTile == null)
                        {
                            armyTwoStartTile = world.TileGrid[width, height];
                            break;
                        }
                    }
                }
            }

            Army armyOne = new Army("armyOne", armyOneStartTile, playerOne, 'A');
            armyOneStartTile.Add(armyOne);
            playerOne.Armies.Add(armyOne);
            int regimentCount = randomNumberGenerator.Next(1, 10);
            for (int i = 0; i < regimentCount; i++)
            {
                Regiment regiment = new Regiment(1000, new UnitType());
                armyOne.Add(regiment);
            }


            Player playerTwo = new Player("Player Two", 'B');
            world.Players.Add(playerTwo);
            
            Army armyTwo = new Army("armyTwo", armyTwoStartTile, playerTwo, '2');
            armyTwoStartTile.Add(armyTwo);
            playerTwo.Armies.Add(armyTwo);
            int playerTwoRegimentCount = randomNumberGenerator.Next(1, 10);
            for (int i = 0; i < playerTwoRegimentCount; i++)
            {
                Regiment regiment = new Regiment(1000, new UnitType());
                armyTwo.Add(regiment);
            }
            
            return world;

        }
    }
}
