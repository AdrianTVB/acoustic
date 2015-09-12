using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestApp_QuadTree.Src
{
    public class Player
    {
        public string PlayerName { get; set; }
        public List<Army> Armies { get; set; }

        public Dictionary<Player, bool> IsHostile { get; set; }

        public Player(string playerName, List<Army> units = null)
        {
            PlayerName = playerName;
            Armies = units ?? new List<Army>();
            IsHostile = new Dictionary<Player, bool>();
        }

        public Tile MoveArmy(World world, int armyIndex, int newCoordinateX, int newCoordinateY)
        {
            if (Armies.Count < armyIndex)
            {
                throw new InvalidCommandException(string.Format("There is no armies with the index {0}", armyIndex));
            }

            Army army = Armies[armyIndex];

            if (army.CurrentTile.FightGoingOn)
            {
                throw new InvalidCommandException(string.Format("Cannot move from tile {0}-{1} as there is a fight going on.", army.CurrentTile.CoordinateX, army.CurrentTile.CoordinateY));   
            }

            Tile newTile = world.TileGrid[newCoordinateX, newCoordinateY];

            if (newTile == null)
            {
                throw new InvalidCommandException(string.Format("There is no tile at the coordinates {0}-{1}.", newCoordinateX, newCoordinateY));
            }

            army.CurrentTile.Remove(army);
            newTile.Add(army);

            return newTile;
        }
    }
}
