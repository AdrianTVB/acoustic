using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src.Actions
{
    public class MoveAction : UnitAction
    {
        public Tile DestinationTile { get; set; }
        public MoveAction(Army army, Tile destinationTile, int delay) :
            base(army, delay)
        {
            DestinationTile = destinationTile;
        }

        public override ActionResult Run(World myWorld)
        {
            if (ArmyToDoSomethingWith.CurrentTile.FightGoingOn)
            {
                return ActionResult.Defer;
            }
        
            ArmyToDoSomethingWith.CurrentTile.Remove(ArmyToDoSomethingWith);
            DestinationTile.Add(ArmyToDoSomethingWith);

            return ActionResult.Success;
        }
    }
}
