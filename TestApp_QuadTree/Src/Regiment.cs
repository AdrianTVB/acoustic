﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class Regiment
    {
        public Regiment(int manCount, UnitType unitType)
        {
            ManCount = manCount;
            UnitTypeInRegiment = unitType;
            Moral = 1;
        }

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
            double remainingMen = ManCount - (damageDelt / 10);
            if (remainingMen <= 0)
            {
                ManCount = 0;
            }
            else
            {
                ManCount = Convert.ToInt32(remainingMen);
            }

            // TODO: Also update moral here.

            return ManCount > 0;
        }
    }
}
