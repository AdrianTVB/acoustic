using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class Regiment
    {
        public UnitType UnitTypeInRegiment { get; set; }
        public int ManCount { get; set; }
        public double Moral { get; set; }

        public double CalculateOffensiveStrength()
        {
            return ManCount * UnitTypeInRegiment.OffensiveStrength * Moral;
        }
        public double CalculateDefensiveStrength()
        {
            return ManCount * UnitTypeInRegiment.DefensiveStrength * Moral;
        }

        public bool DealDamage(double damageDelt)
        {
            ManCount = Convert.ToInt32(ManCount * (damageDelt / 10));
            // TODO: Also update moral here.

            return ManCount > 0;
        }
    }
}
