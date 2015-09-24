using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public abstract class UnitAction
    {
        public enum ActionResult
        {
            Defer, 
            Success, 
            Failure
        }

        public int Delay { get; set; }
        
        public Army ArmyToDoSomethingWith { get; set; }

        public UnitAction(Army army, int delay)
        {
            ArmyToDoSomethingWith = army;
            Delay = delay;
        }

        public virtual ActionResult Run(World world)
        {
            throw new Exception("This method should always be overridden.");
        }
    }
}
