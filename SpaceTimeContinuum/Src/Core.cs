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
                        case "h":
                            Console.WriteLine("-map-terrain");
                            Console.WriteLine("--mt");
                            Console.WriteLine("-map-resource");
                            Console.WriteLine("--mr");
                            Console.WriteLine("-map-army1");
                            Console.WriteLine("--ma1");
                            Console.WriteLine("-map-army2");
                            Console.WriteLine("--ma2");
                            Console.WriteLine("-armies");
                            Console.WriteLine("--a");
                            Console.WriteLine("-move ['0' = army number] to ['0' = x coord]-['0' = y coord]");
                            Console.WriteLine("-end-turn");
                            Console.WriteLine("--e");
                            break;
                        case "-end-turn":
                        case "end":
                        case "--e":
                            endTurn = true;
                            break;
                        case "-map-terrain":
                        case "--mt":
                            DrawMap(MapView.Terrain);
                            break;
                        case "-map-resource":
                        case "--mr":
                            DrawMap(MapView.Resource);
                            break;
                        case "-map-army1":
                        case "--ma1":
                            DrawMap(MapView.ArmyOne);
                            break;
                        case "-map-army2":
                        case "--ma2":
                            DrawMap(MapView.ArmyTwo);
                            break;
                        case "-armies":
                        case "--a":
                            SeeArmies(activePlayer);
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
                                            Order order = activePlayer.MoveArmy(world, armyIndex, coordinateX, coordinateY);
                                            Console.WriteLine(@"Army succesfully moving to {0}-{1}. Will take {2} days.", coordinateX, coordinateY, order.TimeUntilOrderCompletion);

                                            //if (tile.FightGoingOn)
                                            //{
                                            //    Console.WriteLine("There is a fight going on in the tile we have just moved into");
                                            //    for (int i = 0; i < tile.FightingArmies.Count(); i++)
                                            //    {
                                            //        Console.WriteLine(@"Side {0}", i);
                                            //        foreach (Army army in tile.FightingArmies[i])
                                            //        {
                                            //            Console.WriteLine(@"\t {0}: {1}-{2}", army.Owner.PlayerName, army.ArmyName, army.ArmyCount);
                                            //        }
                                            //    }

                                            //    List<Army> nuetralArmiesNotInFight = tile
                                            //        .Where(army => tile.FightingArmies.All(fightingSide => !fightingSide.Contains(army)))
                                            //        .ToList();
                                            //    if (nuetralArmiesNotInFight.Any())
                                            //    {
                                            //        Console.WriteLine("Nuetral Armies:");
                                            //        foreach (Army army in nuetralArmiesNotInFight)
                                            //        {
                                            //            Console.WriteLine(@"\t {0}: {1}-{2}", army.Owner.PlayerName, army.ArmyName, army.ArmyCount);
                                            //        }
                                            //    }
                                            //}

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
            ArmyOne,
            ArmyTwo 
        }
        private void DrawMap(MapView mapViewSetting)
        {
            Dictionary<MapView, Func<Tile, string>> mapViewCodeLookup = new Dictionary<MapView, Func<Tile, string>>
            {
                { MapView.ArmyOne, tile =>
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
                { MapView.ArmyTwo, tile =>
                {
                    List<IGrouping<Player, Army>> armiesAtTile = tile.GroupBy(army => army.Owner).ToList();
                    if(!armiesAtTile.Any())
                    {
                        return "---";
                    }
                    else if(armiesAtTile.Count == 1)
                    {
                        if(tile.Count == 1)
                        {
                            return string.Join("", tile.Select(army => string.Format("{0}{1}", army.ArmyIdentifier, army.Count)));
                        }
                        else if(tile.Count <= 3)
                        {
                            return string.Join("", armiesAtTile.First().Select(army => army.ArmyIdentifier));
                        }
                        else
                        {
                            return "a>3";
                        }
                    }
                    else
                    {
                        return "P>1";
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

        public void SeeArmies(Player player)
        {
            Console.WriteLine("You have the following units:");
            for (int i = 0; i < player.Armies.Count; i++)
            {
                Army army = player.Armies[i];
                Console.WriteLine(
                    string.Format("\t{0}= Army {1}:{2} is at {3}-{4}",
                    i,
                    army.ArmyName,
                    army.ArmyCount,
                    army.CurrentTile.CoordinateX,
                    army.CurrentTile.CoordinateY));
            }
        }

        public void Run()
        {
            int day = 0;
            bool stopApplication = false;
            while (!stopApplication)
            {
                Console.WriteLine(@"Welcome to day {0} of New Zealand.", day);
                foreach (Player player in MyWorld.Players)
                {
                    Console.WriteLine(@"Player {0} turn.", player.PlayerName);
                    SeeArmies(player);
                    // TODO: Print something about resources.

                    Console.WriteLine(@"What would you like to do {0}?", player.PlayerName);
                    TakeCommand(MyWorld, player);
                }

                day++;
                MyWorld.IncrementDay();
            }
        }
        
    }
}
