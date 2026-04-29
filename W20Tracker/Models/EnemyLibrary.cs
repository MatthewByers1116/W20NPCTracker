using System.Collections.Generic;

namespace W20Tracker.Models
{
    public static class EnemyLibrary
    {
        public static List<Combatant> GetAllEnemies()
        {
            return new List<Combatant>
            {
                // === HUMANS ===
                new Combatant {
                    Name = "Random Human (Procedural)", Category = "Humans",
                    Dexterity = 2, InitiativeBase = 0, DodgePool = 0, BlockPool = 0, ParryPool = 0, SoakDice = 0, MaxWillpower = 0
                },
                new Combatant {
                    Name = "Pentex First Team Operative", Category = "Humans",
                    Dexterity = 3, InitiativeBase = 6, DodgePool = 5, BlockPool = 6, ParryPool = 5, SoakDice = 7,
                    MaxWillpower = 6, CurrentWillpower = 6,
                    Attacks = new List<Attack> { new Attack { Name = "Assault Rifle", AttackPool = 7 }, new Attack { Name = "Combat Knife", AttackPool = 6 } },
                    SpecialAbilities = new List<string> { "Pack Tactics: -1 Diff when attacking with allies." }
                },
                new Combatant {
                    Name = "Beat Cop", Category = "Humans",
                    Dexterity = 2, InitiativeBase = 5, DodgePool = 4, BlockPool = 5, ParryPool = 4, SoakDice = 4,
                    MaxWillpower = 4, CurrentWillpower = 4,
                    Attacks = new List<Attack> { new Attack { Name = ".38 Revolver", AttackPool = 5 }, new Attack { Name = "Nightstick", AttackPool = 5 } },
                    SpecialAbilities = new List<string> { "Radio: Can call for backup (SWAT arrives in 3d10 turns)." }
                },
                new Combatant {
                    Name = "Wyrm Cultist", Category = "Humans",
                    Dexterity = 2, InitiativeBase = 4, DodgePool = 3, BlockPool = 4, ParryPool = 4, SoakDice = 3,
                    MaxWillpower = 5, CurrentWillpower = 5,
                    Attacks = new List<Attack> { new Attack { Name = "Sacrificial Dagger", AttackPool = 4 }, new Attack { Name = "Pistol", AttackPool = 4 } },
                    SpecialAbilities = new List<string> { "Fanatical: Will not retreat. Ignores first level of wound penalties." }
                },

                // === FOMORI ===
                new Combatant {
                    Name = "Random Fomor (Procedural)", Category = "Fomori",
                    Dexterity = 2, InitiativeBase = 0, DodgePool = 0, BlockPool = 0, ParryPool = 0, SoakDice = 0, MaxWillpower = 0
                },
                new Combatant {
                    Name = "Magadon Bio-Guard (Fomori)", Category = "Fomori",
                    Dexterity = 2, InitiativeBase = 6, DodgePool = 4, BlockPool = 6, ParryPool = 4, SoakDice = 6,
                    MaxWillpower = 5, CurrentWillpower = 5,
                    Attacks = new List<Attack> { new Attack { Name = "Bio-Toxin Syringe", AttackPool = 5 }, new Attack { Name = "SMG", AttackPool = 6 } },
                    SpecialAbilities = new List<string> { "Bio-Agent: Inhibits Garou regeneration.", "Pain Tolerance: Ignores penalties until Crippled." }
                },
                new Combatant {
                    Name = "Gorehound (Fomori)", Category = "Fomori",
                    Dexterity = 3, InitiativeBase = 7, DodgePool = 5, BlockPool = 7, ParryPool = 2, SoakDice = 5,
                    MaxWillpower = 4, CurrentWillpower = 4,
                    Attacks = new List<Attack> { new Attack { Name = "Savage Bite", AttackPool = 7 }, new Attack { Name = "Claw Flurry", AttackPool = 6 } },
                    SpecialAbilities = new List<string> { "Berserker: Attacks closest target.", "Fungal Armor: Halves Bashing damage." }
                },

                // === BLACK SPIRAL DANCERS ===
                new Combatant {
                    Name = "BSD Shocktrooper (Ahroun)", Category = "Black Spiral Dancers",
                    Dexterity = 4, InitiativeBase = 8, DodgePool = 6, BlockPool = 8, ParryPool = 7, SoakDice = 8,
                    MaxWillpower = 6, CurrentWillpower = 6, MaxRage = 8, CurrentRage = 8, MaxGnosis = 3, CurrentGnosis = 3,
                    Attacks = new List<Attack> { new Attack { Name = "Crinos Claws", AttackPool = 9 }, new Attack { Name = "Bite", AttackPool = 8 } },
                    SpecialAbilities = new List<string> { "Regeneration: Heals 1 Bashing/Lethal per turn.", "Silver Vulnerability.", "Gift: Razor Claws" }
                },
                new Combatant {
                    Name = "BSD Hexer (Theurge)", Category = "Black Spiral Dancers",
                    Dexterity = 3, InitiativeBase = 6, DodgePool = 5, BlockPool = 5, ParryPool = 5, SoakDice = 6,
                    MaxWillpower = 7, CurrentWillpower = 7, MaxRage = 4, CurrentRage = 4, MaxGnosis = 7, CurrentGnosis = 7,
                    Attacks = new List<Attack> { new Attack { Name = "Balefire Blast (Gnosis)", AttackPool = 7 }, new Attack { Name = "Crinos Claws", AttackPool = 6 } },
                    SpecialAbilities = new List<string> { "Regeneration.", "Gift: Toxic Breath", "Can step sideways instantly." }
                },

                // === VAMPIRES ===
                new Combatant {
                    Name = "Sabbat Thug", Category = "Vampires",
                    Dexterity = 3, InitiativeBase = 6, DodgePool = 5, BlockPool = 6, ParryPool = 6, SoakDice = 5,
                    MaxWillpower = 5, CurrentWillpower = 5,
                    Attacks = new List<Attack> { new Attack { Name = "Potence Strike", AttackPool = 7 }, new Attack { Name = "Shotgun", AttackPool = 6 } },
                    SpecialAbilities = new List<string> { "Undead: Halves Bashing damage.", "Blood Buff: Spend blood to increase physical stats.", "Vulnerable to Sunlight/Fire." }
                },

                // === SPIRITS ===
                new Combatant {
                    Name = "Random Bane (Procedural)", Category = "Spirits",
                    Dexterity = 0, InitiativeBase = 0, DodgePool = 0, BlockPool = 0, ParryPool = 0, SoakDice = 0, MaxWillpower = 0
                },
                new Combatant {
                    Name = "Scrag (Bane)", Category = "Spirits",
                    Dexterity = 4, InitiativeBase = 7, DodgePool = 7, BlockPool = 6, ParryPool = 4, SoakDice = 5,
                    MaxWillpower = 6, CurrentWillpower = 6, MaxRage = 8, CurrentRage = 8, MaxGnosis = 5, CurrentGnosis = 5,
                    Attacks = new List<Attack> { new Attack { Name = "Spirit Claws", AttackPool = 8 } },
                    SpecialAbilities = new List<string> { "Materialize: Costs Gnosis.", "Incorporeal in Penumbra." }
                }
            };
        }
    }
}