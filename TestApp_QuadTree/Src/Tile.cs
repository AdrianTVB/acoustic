using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CSharpQuadTree;

namespace TestApp_QuadTree.Src
{
    public class Tile : List<Army>, IPoint 
    {
        private bool atMaxResourceLevel = true;
        public bool FightGoingOn { get; set; }

        public enum TileTypeEnum
        {
            Grassland, 
            Forest, 
            MountainRange, 
            Marsh,
        }

        public enum ResourceTypeEnum
        {
            Wood,
            Stone, 
            Moa, 
            Chicken
        }

        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }

        public List<Army>[] FightingArmies { get; set; } 
        
        public TileTypeEnum TileType { get; set; }

        public ResourceTypeEnum ResourceType { get; set; }

        public int CurrentResourceLevel { get; set; }

        public int MaximumResourceLevel { get; set; }

        public int DailyResourceIncrement { get; set; }

        public Region CurrentRegion { get; set; }

        public void IncrementDailyChanges()
        {
            IncrementResource();
            IncrementFight();
        }

        private void IncrementResource()
        {
            if (!atMaxResourceLevel)
            {
                CurrentResourceLevel += DailyResourceIncrement;
                if (CurrentResourceLevel > MaximumResourceLevel)
                {
                    CurrentResourceLevel = MaximumResourceLevel;
                }
            }
        }

        private void IncrementFight()
        {
            if (FightGoingOn)
            {
                Random randomNumberGenerator = new Random();
                int firstHittingIndex = randomNumberGenerator.Next(0, 1);
                for (int i = 0; i < FightingArmies.Count(); i++)
                {
                    List<Army> offensiveArmies = FightingArmies[firstHittingIndex];
                    double offensiveStrength = offensiveArmies.Sum(army => army.CalculateOffensiveStrength());
                    firstHittingIndex = (firstHittingIndex + 1) % 2;
                    List<Army> defensiveArmies = FightingArmies[firstHittingIndex];
                    double defensiveStrength = offensiveArmies.Sum(army => army.CalculateDefensiveStrength());

                    double damageModifier = defensiveStrength / offensiveStrength;

                    double rawDamageDelt = offensiveStrength / damageModifier;

                    // TODO: Make this balance out to ensure that larger armies take more damage instead of all armies taking equal damage.
                    double damagePerArmy = rawDamageDelt / defensiveArmies.Count;

                    for (int j = 0; j < defensiveArmies.Count; j++)
                    {
                        Army defensiveArmy = defensiveArmies[j];
                        bool armyStillAlive = defensiveArmy.DealDamage(damagePerArmy);
                        if (!armyStillAlive)
                        {
                            defensiveArmies.Remove(defensiveArmy);
                            j--;
                        }
                    }

                    if (!defensiveArmies.Any())
                    {
                        FightingArmies = null;
                        FightGoingOn = false;
                        break;
                    }
                }
            }
        }

        public new void Add(Army newArmy)
        {
            if (FightGoingOn)
            {
                for (int i = 0; i < FightingArmies.Count(); i++)
                {
                    List<Army> side = FightingArmies[i];
                    foreach (Army unitInBattle in side)
                    {
                        if (newArmy.Owner.IsHostile[unitInBattle.Owner])
                        {
                            FightingArmies[(i + 1) % 2].Add(newArmy);
                        }
                    }
                }
            }
            else
            {
                List<Army> oppositionSide = new List<Army>();
                foreach (Army unitAlreadyOnTile in this)
                {
                    if (newArmy.Owner.IsHostile[unitAlreadyOnTile.Owner])
                    {
                        // Start a fight with them.
                        oppositionSide.Add(unitAlreadyOnTile);
                    }
                }

                if (oppositionSide.Any())
                {
                    FightGoingOn = true;
                    FightingArmies = new[]
                    {
                        new List<Army>
                        {
                            newArmy
                        },
                        oppositionSide
                    };
                }
            }

            base.Add(newArmy);
        }
        
        public Point GetPoint()
        {
            return new Point(CoordinateX, CoordinateY);
        }
    }
}
