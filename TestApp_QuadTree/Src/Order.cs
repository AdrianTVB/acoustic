using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class Order : List<UnitAction>
    {
        public int DelayUntilNextAction
        {
            get { return this.First().Delay; }
        }

        public int TimeUntilOrderCompletion
        {
            get { return this.Select(action => action.Delay + 1).Sum(); }
        }

        public void Execute(World world)
        {
            List<UnitAction> ActionsToBeDeleted = new List<UnitAction>();
            foreach (UnitAction action in this)
            {
                if (action.Delay == 0)
                {
                    UnitAction.ActionResult result = action.Run(world);
                    switch (result)
                    {
                        case UnitAction.ActionResult.Defer:
                            // Do nothing, the Delay will be internally adjusted 
                            break;
                        case UnitAction.ActionResult.Failure:
                            throw new NotImplementedException();
                            break;
                        case UnitAction.ActionResult.Success:
                            ActionsToBeDeleted.Add(action);
                            break;

                    }
                }
                else
                {
                    action.Delay--;
                }
            }

            foreach (UnitAction action in ActionsToBeDeleted)
            {
                this.Remove(action);
            }
        }
    }
}
