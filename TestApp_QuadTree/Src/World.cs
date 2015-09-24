using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DeenGames.Utils.AStarPathFinder;
using QuadTreeLibrary;
using TestApp_QuadTree.Src.Actions;
using TestApp_QuadTree.Src.Resources;
using TestApp_QuadTree.Src.Terrains;
using PathFinderPoint = DeenGames.Utils.Point;

namespace TestApp_QuadTree.Src
{
    public class World
    {
        /// <summary>
        /// The path finder grid.http://pastebin.com/Ex2UvjPv
        /// http://www.csharpcity.com/reusable-code/a-path-finding-library/
        /// http://pastebin.com/Ex2UvjPv
        /// </summary>
        private byte[,] pathFinderGrid;

        private PathFinderFast pathFinder;

        public List<List<Order>> OrderQueue { get; set; }

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
            UpdatePathFinderGrid();
        }

        public void UpdatePathFinderGrid()
        {
            int worldWidth = TileGrid.GetLength(0);
            int worldHeight = TileGrid.GetLength(1);

            int pathfinderGridWidth = PathFinderHelper.RoundToNearestPowerOfTwo(worldWidth);
            int pathfinderGridHeight = PathFinderHelper.RoundToNearestPowerOfTwo(worldHeight);

            pathFinderGrid = new byte[pathfinderGridWidth, pathfinderGridHeight];

            for (int width = 0; width < pathfinderGridWidth; width++)
            {
                for (int height = 0; height < pathfinderGridHeight; height++)
                {
                    if (width < worldWidth && height < worldHeight)
                    {
                        Tile tile = TileGrid[width, height];
                        if (tile != null)
                        {
                            if (!(tile.TerrainType is Marsh))
                            {
                                pathFinderGrid[width, height] = PathFinderHelper.EMPTY_TILE;
                            }
                        }
                    }

                    pathFinderGrid[width, height] = PathFinderHelper.BLOCKED_TILE;
                }
            }

            pathFinder = new PathFinderFast(pathFinderGrid);
        }

        public void AddOrderToQueue(Order order)
        {
            for (int i = OrderQueue.Count; i <= order.DelayUntilNextAction; i++)
            {
                OrderQueue.Add(new List<Order>());
            }
            
            OrderQueue[order.DelayUntilNextAction].Add(order);
        }

        public Order MakeMoveOrder(Army armyToMove, Tile destinationTile)
        {
            Tile sourceTile = armyToMove.CurrentTile;
            List<PathFinderNode> path = pathFinder.FindPath(
                new PathFinderPoint(sourceTile.CoordinateX, sourceTile.CoordinateY),
                new PathFinderPoint(destinationTile.CoordinateX, destinationTile.CoordinateY));
            if (path != null)
            {
                Order order = new Order();

                foreach (PathFinderNode pathFinderNode in path)
                {
                    Tile actionDestinationTile = TileGrid[pathFinderNode.X, pathFinderNode.Y];
                    MoveAction moveAction = new MoveAction(armyToMove, actionDestinationTile, 0);
                    order.Add(moveAction);
                }

                AddOrderToQueue(order);
            }

            return null;
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
            for (int width = 0; width < TileGrid.GetLength(0); width++)
            {
                for (int height = 0; height < TileGrid.GetLength(1); height++)
                {
                    Tile tile = TileGrid[width, height];
                    if(tile != null)
                    {
                        tile.IncrementDailyChanges();
                    }

                    List<Order> ordersToExecuteThisDay = OrderQueue[0];
                    OrderQueue.RemoveAt(0);

                    foreach (Order order in ordersToExecuteThisDay)
                    {
                        order.Execute(this);
                    }
                }
            }
        }

        public class PlayerList : List<Player>
        {
            public new void Add(Player newPlayer)
            {
                foreach (Player existingPlayer in this)
                {
                    existingPlayer.IsHostile.Add(newPlayer, true);
                    newPlayer.IsHostile.Add(existingPlayer, true);
                }

                base.Add(newPlayer);
            }
        }
    }
}
