using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src
{
    public class UnitType
    {
        private static string unitName = "genericUnit";
        private static double offensiveStrength = 5;
        private static double defensiveStrength = 5;
        private static double maximumMoral = 1;
        public string UnitName
        {
            get
            {
                return unitName;
            }

            set
            {
                unitName = value;
            }
        }
        public double OffensiveStrength
        {
            get
            {
                return offensiveStrength;
            }

            set
            {
                offensiveStrength = value;
            }
        }
        public double DefensiveStrength
        {
            get
            {
                return defensiveStrength;
            }

            set
            {
                defensiveStrength = value;
            }
        }
        public double MaximumMoral
        {
            get
            {
                return maximumMoral;
            }

            set
            {
                maximumMoral = value;
            }
        }

    }
}
