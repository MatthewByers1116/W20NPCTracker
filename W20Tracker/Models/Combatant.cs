using System;
using System.Collections.Generic;

namespace W20Tracker.Models
{
    public enum DamageState { None, Bashing, Lethal, Aggravated }

    public class Attack
    {
        public string Name { get; set; } = string.Empty;
        public int AttackPool { get; set; }
    }

    public class Combatant
    {
        public string Name { get; set; } = "Unknown Threat";
        public string Category { get; set; } = "Mortal";

        // Defensive & Core Stats
        public int Dexterity { get; set; } = 2;
        public int InitiativeBase { get; set; }
        public int DodgePool { get; set; }
        public int BlockPool { get; set; }
        public int ParryPool { get; set; }
        public int SoakDice { get; set; }

        // Movement Modifiers
        public double SpeedMultiplier { get; set; } = 1.0;
        public int RunSpeedBonus { get; set; } = 0;
        public bool CanFly { get; set; } = false;

        // Armor
        public string ArmorName { get; set; } = "None";
        public int ArmorRating { get; set; } = 0;
        public int ArmorDexPenalty { get; set; } = 0;

        // Resources
        public int MaxWillpower { get; set; }
        public int CurrentWillpower { get; set; }
        public int MaxRage { get; set; }
        public int CurrentRage { get; set; }
        public int MaxGnosis { get; set; }
        public int CurrentGnosis { get; set; }
        public int MaxEssence { get; set; }
        public int CurrentEssence { get; set; }

        public List<Attack> Attacks { get; set; } = new List<Attack>();
        public List<string> SpecialAbilities { get; set; } = new List<string>();

        // Health Track - 7 standard WoD boxes
        // 0:Bruised(0), 1:Hurt(-1), 2:Injured(-1), 3:Wounded(-2), 4:Mauled(-2), 5:Crippled(-5), 6:Incapacitated
        public DamageState[] HealthBoxes { get; set; } = new DamageState[7];

        public int CurrentWoundPenalty
        {
            get
            {
                if (HealthBoxes[6] != DamageState.None) return -99; // Incapacitated
                if (HealthBoxes[5] != DamageState.None) return -5;  // Crippled
                if (HealthBoxes[4] != DamageState.None || HealthBoxes[3] != DamageState.None) return -2; // Mauled/Wounded
                if (HealthBoxes[2] != DamageState.None || HealthBoxes[1] != DamageState.None) return -1; // Injured/Hurt
                return 0;
            }
        }

        public string CurrentMovementDisplay
        {
            get
            {
                // Absolute lowest health states override all math
                if (HealthBoxes[6] != DamageState.None) return "0 yds (Incapacitated)";
                if (HealthBoxes[5] != DamageState.None) return "1 yd (Crawling)";
                if (HealthBoxes[4] != DamageState.None) return "3 yds (Staggering)";

                // Base formulas from W20 pg. 267
                double walk = 7 * SpeedMultiplier;
                double jog = (12 + Dexterity) * SpeedMultiplier;
                double run = ((20 + (3 * Dexterity)) * SpeedMultiplier) + RunSpeedBonus;

                // Wounded (-2 box): May walk but cannot run
                if (HealthBoxes[3] != DamageState.None)
                {
                    return $"{Math.Floor(walk)} yds (Walk Only - Cannot Move & Attack)";
                }

                // Injured (-1 box): Halve maximum running speed
                if (HealthBoxes[2] != DamageState.None)
                {
                    run = Math.Floor(run / 2);
                }

                string result = $"Walk {Math.Floor(walk)}y | Jog {Math.Floor(jog)}y | Run {Math.Floor(run)}y";
                if (CanFly) result += " | Fly 13y";

                return result;
            }
        }
    }
}