using SpaceTimeContinuum.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp_QuadTree.Src;

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
            World world = new World(1000, 1000);

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

            Random randomNumberGenerator = new Random();
            for(int i = 0; i < regions.Length; i++)
            {
                string regionName = regions[i];
                Region region = new Region(world);
                world.Regions.Add(region);
                int tileWidth = randomNumberGenerator.Next(2, 10);
                int tileHeight = randomNumberGenerator.Next(2, 10);
                int regionModifier = i * 10;
                for (int width = 0; width < tileWidth; width++)
                {
                    for (int height = 0; height < tileHeight; height++)
                    {
                        int tileType = randomNumberGenerator.Next(0, Enum.GetValues(typeof(Tile.TileTypeEnum)).Length);
                        int resourceType = randomNumberGenerator.Next(0, Enum.GetValues(typeof(Tile.ResourceTypeEnum)).Length);
                        Tile tile = new Tile(regionModifier + width, regionModifier + height, (Tile.TileTypeEnum)tileType, (Tile.ResourceTypeEnum)resourceType, region);
                        region.Add(tile);
                    }
                }
            }



            Player playerOne = new Player("Player One");
            world.Players.Add(playerOne);

            Tile tileOne = world.TileGrid[1, 1];
            Army armyOne = new Army("armyOne", tileOne, playerOne);
            tileOne.Add(armyOne);
            playerOne.Armies.Add(armyOne);
            int regimentCount = randomNumberGenerator.Next(0, 10);
            for (int i = 0; i < regimentCount; i++)
            {
                Regiment regiment = new Regiment(1000, new UnitType());
                armyOne.Add(regiment);
            }


            Player playerTwo = new Player("Player Two");
            world.Players.Add(playerTwo);

            Tile tileTwo = world.TileGrid[2, 2];
            Army armyTwo = new Army("armyTwo", tileOne, playerTwo);
            tileTwo.Add(armyTwo);
            playerTwo.Armies.Add(armyOne);
            int playerTwoRegimentCount = randomNumberGenerator.Next(0, 10);
            for (int i = 0; i < playerTwoRegimentCount; i++)
            {
                Regiment regiment = new Regiment(1000, new UnitType());
                armyOne.Add(regiment);
            }
            
            return world;

        }
    }
}
