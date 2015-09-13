using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp_QuadTree.Src;

namespace SpaceTimeContinuum.Src
{
    public class Core
    {
        public World MyWorld { get; set; }
        public Core(World myWorld)
        {
            MyWorld = myWorld;
        }

        private void TakeCommand(World world, Player activePlayer)
        {
            bool endTurn = false;
            while (!endTurn)
            {
                Console.WriteLine("Please enter a command....");
                string command = Console.ReadLine();
                try
                {
                    switch (command)
                    {
                        case "help":
                        case "Help":
                            break;
                        case "End turn":
                            endTurn = true;
                            break;
                        case "status-terrain":
                            DrawMap(MapView.Terrain);
                            break;
                        case "status-resource":
                            DrawMap(MapView.Resource);
                            break;
                        case "status-army":
                            DrawMap(MapView.Army);
                            break;
                        default:
                            if (command != null && (command.IndexOf("move") > -1 || command.IndexOf("Move") > -1))
                            {
                                string[] commandElements = command.Split(' ', '-');
                                int armyIndex;
                                if (int.TryParse(commandElements[1], out armyIndex))
                                {
                                    int coordinateX;
                                    if (int.TryParse(commandElements[3], out coordinateX))
                                    {
                                        int coordinateY;
                                        if (int.TryParse(commandElements[4], out coordinateY))
                                        {
                                            Tile tile = activePlayer.MoveArmy(world, armyIndex, coordinateX, coordinateY);
                                            Console.WriteLine(@"Army succesfully moved to {0}-{1}", tile.CoordinateX, tile.CoordinateY);

                                            if (tile.FightGoingOn)
                                            {
                                                Console.WriteLine("There is a fight going on in the tile we have just moved into");
                                                for (int i = 0; i < tile.FightingArmies.Count(); i++)
                                                {
                                                    Console.WriteLine(@"Side {0}", i);
                                                    foreach (Army army in tile.FightingArmies[i])
                                                    {
                                                        Console.WriteLine(@"\t {0}: {1}-{2}", army.Owner.PlayerName, army.ArmyName, army.ArmyCount);
                                                    }
                                                }

                                                List<Army> nuetralArmiesNotInFight = tile
                                                    .Where(army => tile.FightingArmies.All(fightingSide => !fightingSide.Contains(army)))
                                                    .ToList();
                                                if (nuetralArmiesNotInFight.Any())
                                                {
                                                    Console.WriteLine("Nuetral Armies:");
                                                    foreach (Army army in nuetralArmiesNotInFight)
                                                    {
                                                        Console.WriteLine(@"\t {0}: {1}-{2}", army.Owner.PlayerName, army.ArmyName, army.ArmyCount);
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }
                            }

                            Console.WriteLine("That command is unrecognised.");
                            break;
                    }
                }
                catch (InvalidCommandException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private enum MapView
        {
            Terrain, 
            Resource,
            Army, 
        }
        private void DrawMap(MapView mapViewSetting)
        {
            Dictionary<MapView, Func<Tile, string>> mapViewCodeLookup = new Dictionary<MapView, Func<Tile, string>>
            {
                { MapView.Army, tile =>
                {
                    List<IGrouping<Player, Army>> armiesAtTile = tile.GroupBy(army => army.Owner).ToList();
                    if(!armiesAtTile.Any())
                    {
                        return "---";
                    }
                    else if(armiesAtTile.Count == 1)
                    {
                        int regimentCount = armiesAtTile.First().Select(army => army.Count).Sum();
                        return string.Format("{0}-{1}", armiesAtTile.First().Key.PlayerCode, regimentCount);
                    }
                    else if(armiesAtTile.Count <= 3)
                    {
                        return string.Join("", armiesAtTile.Select(grouping => grouping.Key.PlayerCode));
                    }
                    else
                    {
                        return "P>3";
                    }
                } },
                { MapView.Terrain, tile =>
                {
                    return tile.TerrainType.TerrainCode + "  ";
                } },
                { MapView.Resource, tile =>
                {
                    return tile.ResourceType.ResourceCode + "  ";
                } },
            };
            
            DrawMap(mapViewCodeLookup[mapViewSetting]);
        }

        private void DrawMap(Func<Tile, string> characterSelector)
        {
            Console.WriteLine("Current world:");
            int maxWidth = MyWorld.TileGrid.GetLength(0);
            int maxHeight = MyWorld.TileGrid.GetLength(1);

            for (int width = maxWidth - 1; width > 0; width--)
            {
                Console.Write("{0}   ", width);
                for (int height = 0; height < maxHeight; height++)
                {
                    Tile tile = MyWorld.TileGrid[width, height];
                    string identifyingCharacter;
                    if (tile == null)
                    {
                        identifyingCharacter = "   ";
                    }
                    else
                    {
                        identifyingCharacter = characterSelector(tile);
                    }

                    Console.Write("{0} ", identifyingCharacter);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("    ");
            for(int width = 0; width < maxHeight; width++)
            {
                Console.Write("{0}   ", width);
            }

            Console.WriteLine();
        }

        public void Run()
        {
            int day = 0;
            bool stopApplication = false;
            while (!stopApplication)
            {
                day++;
                Console.WriteLine(@"Welcome to day {0} of New Zealand.");
                foreach (Player player in MyWorld.Players)
                {
                    Console.WriteLine(@"Player {0} turn.");
                    Console.WriteLine("You have the following units:");
                    for (int i = 0; i < player.Armies.Count; i++)
                    {
                        Army army = player.Armies[i];
                        Console.WriteLine(
                            @"\t{0}= Army {1}:{2} is at {3}-{4}", 
                            i,
                            army.ArmyName, 
                            army.ArmyCount, 
                            army.CurrentTile.CoordinateX,
                            army.CurrentTile.CoordinateY);
                    }
                    // TODO: Print something about resources.

                    Console.WriteLine(@"What would you like to do {0}?", player.PlayerName);
                    TakeCommand(MyWorld, player);
                }
            }
        }
    }
}
