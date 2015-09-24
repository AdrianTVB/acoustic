using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DeenGames.Utils.AStarPathFinder;
using TestApp_QuadTree.Src.Terrains;

namespace TestApp_QuadTree.Src
{
    public class Player
    {
        public string PlayerName { get; set; }
        public List<Army> Armies { get; set; }

        public char PlayerCode { get; set; }

        public Dictionary<Player, bool> IsHostile { get; set; }

        public Player(string playerName, char playerCode, List<Army> units = null)
        {
            PlayerCode = playerCode;
            PlayerName = playerName;
            Armies = units ?? new List<Army>();
            IsHostile = new Dictionary<Player, bool>();
        }
        
        public Order MoveArmy(World world, int armyIndex, int newCoordinateX, int newCoordinateY)
        {
            if (Armies.Count < armyIndex)
            {
                throw new InvalidCommandException(string.Format("There is no armies with the index {0}", armyIndex));
            }

            Army army = Armies[armyIndex];

            Tile newTile = world.TileGrid[newCoordinateX, newCoordinateY];

            if (newTile == null)
            {
                throw new InvalidCommandException(string.Format("There is no tile at the coordinates {0}-{1}.", newCoordinateX, newCoordinateY));
            }

            return world.MakeMoveOrder(army, newTile);
        }
    }
}
