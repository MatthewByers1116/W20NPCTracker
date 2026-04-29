using System;
using System.Collections.Generic;
using System.Linq;

namespace W20Tracker.Models
{
    public class EncounterStateService
    {
        public List<Combatant> ActiveNPCs { get; private set; } = new List<Combatant>();
        public List<Combatant> EnemyTemplates { get; private set; } = EnemyLibrary.GetAllEnemies();
        private Random _rng = new Random();

        public void SpawnEnemy(string templateName, int rank = 1)
        {
            if (templateName == "Random Fomor (Procedural)") { ActiveNPCs.Add(GenerateProceduralFomor(rank)); return; }
            if (templateName == "Random Human (Procedural)") { ActiveNPCs.Add(GenerateProceduralHuman(rank)); return; }
            if (templateName == "Random Bane (Procedural)") { ActiveNPCs.Add(GenerateProceduralBane(rank)); return; }

            var template = EnemyTemplates.FirstOrDefault(e => e.Name == templateName);
            if (template != null)
            {
                var newNpc = new Combatant
                {
                    Name = $"{template.Name} {ActiveNPCs.Count(n => n.Name.Contains(template.Name)) + 1}",
                    Category = template.Category,
                    Dexterity = template.Dexterity,
                    InitiativeBase = template.InitiativeBase,
                    DodgePool = template.DodgePool,
                    BlockPool = template.BlockPool,
                    ParryPool = template.ParryPool,
                    SoakDice = template.SoakDice,
                    MaxWillpower = template.MaxWillpower,
                    CurrentWillpower = template.MaxWillpower,
                    MaxRage = template.MaxRage,
                    CurrentRage = template.MaxRage,
                    MaxGnosis = template.MaxGnosis,
                    CurrentGnosis = template.MaxGnosis,
                    Attacks = template.Attacks.Select(a => new Attack { Name = a.Name, AttackPool = a.AttackPool }).ToList(),
                    SpecialAbilities = new List<string>(template.SpecialAbilities)
                };

                if (newNpc.Category == "Spirits")
                {
                    newNpc.MaxEssence = newNpc.MaxRage + newNpc.MaxGnosis + newNpc.MaxWillpower;
                    newNpc.CurrentEssence = newNpc.MaxEssence;
                }

                ApplyUniversalScaling(newNpc, rank);
                ActiveNPCs.Add(newNpc);
            }
        }

        private void ApplyUniversalScaling(Combatant npc, int rank)
        {
            if (rank <= 1) return;

            int bonus = rank - 1;
            npc.Dexterity += (bonus / 2); // Dex scales slowly
            npc.InitiativeBase += bonus; npc.DodgePool += bonus; npc.BlockPool += bonus; npc.ParryPool += bonus; npc.SoakDice += (bonus / 2);
            foreach (var attack in npc.Attacks) { attack.AttackPool += bonus; }

            npc.MaxWillpower = Math.Min(10, npc.MaxWillpower + bonus); npc.CurrentWillpower = npc.MaxWillpower;
            if (npc.MaxRage > 0) { npc.MaxRage = Math.Min(10, npc.MaxRage + bonus); npc.CurrentRage = npc.MaxRage; }
            if (npc.MaxGnosis > 0) { npc.MaxGnosis = Math.Min(10, npc.MaxGnosis + bonus); npc.CurrentGnosis = npc.MaxGnosis; }
            if (npc.MaxEssence > 0) { npc.MaxEssence += (bonus * 3); npc.CurrentEssence = npc.MaxEssence; }

            npc.Name += $" (Level {rank})";
            npc.SpecialAbilities.Insert(0, $"--- Level {rank} Scaling Applied ---");

            if (npc.Category == "Black Spiral Dancers")
            {
                string[] ahrounGifts = { "Resist Pain", "Razor Claws", "Silver Claws", "Wallow", "Kiss of Helios" };
                for (int i = 0; i < rank && i < ahrounGifts.Length; i++) { npc.SpecialAbilities.Add($"Gift: {ahrounGifts[i]}"); }
            }
        }

        private void AssignProceduralArmor(Combatant npc, int level)
        {
            if (npc.Category != "Humans" && npc.Category != "Fomori") return;

            int roll = _rng.Next(1, 11) + level;

            if (roll <= 3) return;
            else if (roll <= 5) { npc.ArmorName = "Reinforced Clothing"; npc.ArmorRating = 1; npc.ArmorDexPenalty = 0; }
            else if (roll <= 6) { npc.ArmorName = "Biker Jacket"; npc.ArmorRating = 1; npc.ArmorDexPenalty = 1; }
            else if (roll <= 7) { npc.ArmorName = "Leather Duster"; npc.ArmorRating = 2; npc.ArmorDexPenalty = 2; }
            else if (roll <= 8) { npc.ArmorName = "Kevlar Vest"; npc.ArmorRating = 3; npc.ArmorDexPenalty = 1; }
            else if (roll <= 9) { npc.ArmorName = "Bearskin Coat"; npc.ArmorRating = 3; npc.ArmorDexPenalty = 3; }
            else if (roll <= 10) { npc.ArmorName = "Steel Breastplate"; npc.ArmorRating = 3; npc.ArmorDexPenalty = 2; }
            else if (roll <= 12) { npc.ArmorName = "Flak Vest"; npc.ArmorRating = 4; npc.ArmorDexPenalty = 2; }
            else { npc.ArmorName = "Riot Suit"; npc.ArmorRating = 5; npc.ArmorDexPenalty = 3; }

            if (npc.Category == "Fomori" && _rng.Next(1, 3) == 1)
            {
                int hideLevel = Math.Min(3, (level + 1) / 2);
                npc.ArmorName = $"Tough Hide {hideLevel}";
                npc.ArmorRating = hideLevel;
                npc.ArmorDexPenalty = 0;
            }

            npc.SoakDice += npc.ArmorRating;
            npc.InitiativeBase = Math.Max(0, npc.InitiativeBase - npc.ArmorDexPenalty);
            npc.DodgePool = Math.Max(0, npc.DodgePool - npc.ArmorDexPenalty);
            npc.BlockPool = Math.Max(0, npc.BlockPool - npc.ArmorDexPenalty);
            npc.ParryPool = Math.Max(0, npc.ParryPool - npc.ArmorDexPenalty);

            // Note: In strict W20, armor lowers Dexterity for ALL pools, but we just apply it to combat pools here to keep base Dex pure for movement math
        }

        private Combatant GenerateProceduralHuman(int level)
        {
            var human = new Combatant
            {
                Name = $"Mercenary / Thug {ActiveNPCs.Count(n => n.Category == "Humans") + 1} (Level {level})",
                Category = "Humans",
                Dexterity = 2,
                InitiativeBase = 4,
                DodgePool = 3,
                BlockPool = 3,
                ParryPool = 3,
                SoakDice = 2,
                MaxWillpower = 3,
                CurrentWillpower = 3,
                Attacks = new List<Attack>(),
                SpecialAbilities = new List<string>()
            };

            string[] ranged = { "Heavy Pistol", "Shotgun", "SMG", "Assault Rifle" };
            string[] melee = { "Knife", "Baseball Bat", "Machete", "Stun Baton" };
            human.Attacks.Add(new Attack { Name = ranged[Math.Min(ranged.Length - 1, level - 1)], AttackPool = 4 });
            human.Attacks.Add(new Attack { Name = melee[Math.Min(melee.Length - 1, level - 1)], AttackPool = 4 });

            AssignProceduralArmor(human, level);
            ApplyUniversalScaling(human, level);
            return human;
        }

        private Combatant GenerateProceduralFomor(int level)
        {
            var fomor = new Combatant
            {
                Name = $"Mutated Fomor {ActiveNPCs.Count(n => n.Category == "Fomori") + 1} (Level {level})",
                Category = "Fomori",
                Dexterity = 2,
                InitiativeBase = 5,
                DodgePool = 4,
                BlockPool = 4,
                ParryPool = 3,
                SoakDice = 3,
                MaxWillpower = 3,
                CurrentWillpower = 3,
                Attacks = new List<Attack> { new Attack { Name = "Brawl / Smash", AttackPool = 5 } },
                SpecialAbilities = new List<string> { "Immunity to the Delirium" }
            };

            var selectedPowers = GetFomoriPowersList().OrderBy(x => _rng.Next()).Take(level).ToList();
            foreach (var power in selectedPowers)
            {
                fomor.SpecialAbilities.Add($"- {power.Name}: {power.Description}");
                if (power.Name == "Berserker") { fomor.MaxRage = 5; fomor.CurrentRage = 5; }
                else if (power.Name == "Armored Skin" || power.Name == "Armored Hide") fomor.SoakDice += 3;
                else if (power.Name == "Claws and Fangs") fomor.Attacks.Add(new Attack { Name = "Mutated Claws (Agg)", AttackPool = 6 });
                else if (power.Name == "Extra Limbs") { fomor.BlockPool += 3; fomor.Attacks.Add(new Attack { Name = "Tentacle Grab", AttackPool = 7 }); }
                else if (power.Name == "Bestial Mutation") { fomor.Attacks.First().AttackPool += 2; fomor.DodgePool += 2; }

                // New Movement Powers
                else if (power.Name == "Extra Speed") { fomor.SpeedMultiplier = 1.5; fomor.InitiativeBase += 2; }
                else if (power.Name == "Body Expansion") { fomor.RunSpeedBonus += 10; fomor.Attacks.First().AttackPool += 1; }
                else if (power.Name == "Wings") { fomor.CanFly = true; }
            }

            AssignProceduralArmor(fomor, level);
            ApplyUniversalScaling(fomor, (level / 2) + 1);
            return fomor;
        }

        private Combatant GenerateProceduralBane(int level)
        {
            string[] natures = { "Murder", "Hunger", "Disease", "Jealousy", "Spite", "Anger", "Lust", "Self-Loathing", "Shame", "Theft", "Abandonment", "Humiliation", "Distrust", "Fear", "Dishonesty" };
            string nature = natures[_rng.Next(natures.Length)];

            int totalPoints = 9 + ((level - 1) * 5);
            int rage = Math.Max(1, (totalPoints / 3) + _rng.Next(-1, 2));
            int gnosis = Math.Max(1, (totalPoints / 3) + _rng.Next(-1, 2));
            int willpower = Math.Max(1, totalPoints - rage - gnosis);

            var bane = new Combatant
            {
                Name = $"Bane of {nature} {ActiveNPCs.Count(n => n.Category == "Spirits") + 1} (Lvl {level})",
                Category = "Spirits",
                Dexterity = willpower, // Spirits rely on Willpower
                MaxRage = rage,
                CurrentRage = rage,
                MaxGnosis = gnosis,
                CurrentGnosis = gnosis,
                MaxWillpower = willpower,
                CurrentWillpower = willpower,
                MaxEssence = rage + gnosis + willpower,
                CurrentEssence = rage + gnosis + willpower,

                InitiativeBase = willpower,
                DodgePool = willpower,
                SoakDice = willpower,
                Attacks = new List<Attack> { new Attack { Name = "Umbral Strike (Willpower)", AttackPool = willpower } },
                SpecialAbilities = new List<string> {
                    $"Nature: {nature}",
                    "Spirit Combat: Attack/Dodge/Soak use Willpower. Unsoaked damage reduces Essence. 0 Essence = Destroyed."
                }
            };

            int numCharms = level + 1;
            var selectedCharms = GetBaneCharmsList().OrderBy(x => _rng.Next()).Take(numCharms).ToList();

            foreach (var charm in selectedCharms)
            {
                bane.SpecialAbilities.Add($"- {charm.Name}: {charm.Description}");
                if (charm.Name == "Armor") bane.SoakDice += 3;
                if (charm.Name == "Materialize") bane.Attacks.Add(new Attack { Name = "Materialized Strike", AttackPool = (willpower + rage) / 2 });
            }

            return bane;
        }

        private List<(string Name, string Description)> GetBaneCharmsList()
        {
            return new List<(string, string)>
            {
                ("Blighted Touch", "On successful attack, target rolls Willpower. Fail = negative traits dominate personality for hours."),
                ("Corruption", "Whispers suggestion. Roll Gnosis vs target's Willpower. Target acts upon it."),
                ("Incite Frenzy", "Roll Rage vs target's Willpower. Success = target enters Frenzy. 6+ successes = Thrall of the Wyrm."),
                ("Possession", "Roll Gnosis vs target Willpower (or 4 for object). Time taken depends on successes. Can warp human into a Fomor."),
                ("Materialize", "Spend Gnosis to enter the physical world and gain physical Attributes."),
                ("Armor", "Grants additional dice to soak damage in the Umbra."),
                ("Blast", "Can strike targets at a distance using Rage/Willpower."),
                ("Create Wind", "Whips up unnatural, howling umbral winds to confuse and disorient.")
            };
        }

        private List<(string Name, string Description)> GetFomoriPowersList()
        {
            return new List<(string, string)>
            {
                ("Armored Skin", "Can soak and heal lethal damage as if it was bashing damage."),
                ("Berserker", "Gains 5 Rage points and is vulnerable to frenzy."),
                ("Bestial Mutation", "Gains 2 additional dice for Physical Attributes; Appearance drops to 0."),
                ("Brain Eating", "Roll Int+Occult to temporarily reduce target's Mental Attributes."),
                ("Claws and Fangs", "Attacks inflict Strength +1 aggravated damage. Cannot be concealed."),
                ("Extra Limbs", "Has an extra three dice for grapple attempts."),
                ("Fungal Touch", "Infects opponent with a touch. Target loses 1 point from Physical Attributes."),
                ("Hazardous Breath", "Spew a cloud of harmful halitosis (Diff 7 Dex+Brawl) for 1 Health Level."),
                ("Noxious Breath", "Fumes induce cravings. Failed Willpower takes 1 unsoakable Aggravated dam."),
                ("Regeneration", "Heals one level of bashing or lethal damage each turn."),
                ("Size Shift", "Gains one temporary Health Level and one Strength/Stamina dot per scene."),
                ("Toxic Secretions", "Fluids splatter attackers. Inflicts 1 die of damage over time."),
                ("Webbing", "Spew webs to trap prey or create armor with 6 soak dice and 3 Health Levels."),
                ("Body-Barbs", "Bone blades protrude from joints. Adds two dice of damage to Brawl attacks."),
                ("Venomous Bite", "Bite secretes poison inflicting Strength +2 unsoakable Aggravated damage."),
                ("Extra Speed", "Takes an extra action without splitting dice pool. Movement x1.5"),
                ("Body Expansion", "Add 10 yards to running speed. Strength +4 damage. Brittle bones."),
                ("Wings", "Can fly at a jogging speed of 13 yards per turn.")
            };
        }

        public void RemoveNPC(Combatant combatant) => ActiveNPCs.Remove(combatant);
        public void ClearEncounter() => ActiveNPCs.Clear();
    }
}