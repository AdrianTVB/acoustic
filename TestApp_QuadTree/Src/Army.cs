using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class Army : List<Regiment>
    {
        public Tile CurrentTile { get; set; }
        public Player Owner { get; set; }
        public string ArmyName { get; set; }

        public int ArmyCount
        {
            get
            {
                return this.Sum(regiment => regiment.ManCount);
            }
        }

        public double CalculateOffensiveStrength()
        {
            return this.Sum(regiment => CalculateOffensiveStrength());
        }
        public double CalculateDefensiveStrength()
        {
            return this.Sum(regiment => regiment.CalculateDefensiveStrength());
        }

        public bool DealDamage(double damageDelt)
        {
            // TODO: Balance this out to ensure each regiment takes a portion of the damage based on how large the regiment is. (On second thought, maybe not as each regiment should be roughly the same size.)
            double damagePerRegiment = damageDelt / Count;
            for (int i = 0; i < Count; i++)
            {
                Regiment regiment = this[i];
                bool regimentStillAlive = regiment.DealDamage(damagePerRegiment);

                if (!regimentStillAlive)
                {
                    Remove(regiment);
                    i--;
                }
            }

            return Count > 0;
        }
    }
}
