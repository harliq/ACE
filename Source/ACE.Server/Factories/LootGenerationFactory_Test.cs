using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using log4net;

using ACE.Common;
using ACE.Database;
using ACE.Database.Models.World;
using ACE.Database.Models.Shard;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.Factories
{
    public class LootStats
    {
        // Counters
            public float ArmorCount { get; set; }
            public float MeleeWeaponCount { get; set; }
            public float CasterCount { get; set; }
            public float MissileWeaponCount { get; set; }
            public float JewelryCount { get; set; }
            public float GemCount { get; set; }
            public float ClothingCount { get; set; }
            public float OtherCount { get; set; }
            public float NullCount { get; set; }
            public int MinItemsCreated { get; set; }
            public int MaxItemsCreated { get; set; }

        // Weapon Tables
            public string MeleeWeapons { get; set; }
            public string MissileWeapons { get; set; }
            public string CasterWeapons { get; set; }

        // Weapon Stats
            public int ItemMaxMana { get; set; }
            public int MinMana { get; set; }
            public int MaxMana { get; set; }
            public int HasManaCount { get; set; }
            public int TotalMaxMana { get; set; }
    }
    public static class LootGenerationFactory_Test
    {
        public static string TestLootGen(int numItems, int tier)
        {
            string dataToPrint = "Hits TestLootGen Class";

            Console.WriteLine($"Creating {numItems} items, that are in tier {tier}");

             var ls = SetLootStatsDefaults(new LootStats());
            
            ////// Counters
            ////float armorCount = 0;
            ////float meleeWeaponCount = 0;
            ////float casterCount = 0;
            ////float missileWeaponCount = 0;
            ////float jewelryCount = 0;
            ////float gemCount = 0;
            ////float clothingCount = 0;
            ////float otherCount = 0;
            ////float nullCount = 0;

            ////// Weapon Properties 
            ////double missileDefMod = 0.00f;
            ////double magicDefMod = 0.00f;
            ////double wield = 0.00f;
            ////int value = 0;
            ////int itemMaxMana = 0;
            ////int minMana = 50000;
            ////int maxMana = 0;
            ////int hasManaCount = 0;
            ////int totalMaxMana = 0;

            ////string meleeWeapons = $"-----Melee Weapons----\n Skill \t\t\t Wield \t Damage \t Variance \t DefenseMod \t MagicDBonus \t MissileDBonus\t Value\t Type \n";
            ////string missileWeapons = $"-----Missile Weapons----\n Type \t Wield \t Modifier \tElementBonus \t DefenseMod \t MagicDBonus \t MissileDBonus\t Value\n";
            ////string casterWeapons = $"-----Caster Weapons----\n Wield \t ElementBonus \t DefenseMod \t MagicDBonus \t MissileDBonus \t Value \t MaxMana\n";

            // Loop depending on how many items you are creating
            for (int i = 0; i < numItems; i++)
            {
                var testItem = LootGenerationFactory.CreateRandomLootObjects(tier, true);
                ls = LootStats(testItem, ls);
            }
            DisplayStats(ls);
            return dataToPrint;
        }
        public static string TestLootGenMonster(int wcid, int numberofcorpses)
        {
            string test = "";

            var corpseContainer = new List<WorldObject>();
            var ls = SetLootStatsDefaults(new LootStats());

            Console.WriteLine($"Creating {numberofcorpses} corpses.");

            var deathTreasure = DatabaseManager.World.GetCachedDeathTreasure(998);
            for (int i = 0; i < numberofcorpses; i++)
            {
                if (deathTreasure != null)
                {
                    // TODO: get randomly generated death treasure from LootGenerationFactory
                    // log.Debug($"{_generator.Name}.TreasureGenerator(): found death treasure {Biota.WeenieClassId}");
                    corpseContainer = LootGenerationFactory.CreateRandomLootObjects(deathTreasure);
                    if (corpseContainer.Count < ls.MinItemsCreated)
                        ls.MinItemsCreated = corpseContainer.Count;
                    if (corpseContainer.Count > ls.MaxItemsCreated)
                        ls.MaxItemsCreated = corpseContainer.Count;

                    foreach (var lootItem in corpseContainer)
                    {
                        ls = LootStats(lootItem, ls);
                    }
                }
                else
                {
                    //// var wieldedTreasure = DatabaseManager.World.GetCachedWieldedTreasure(Biota.WeenieClassId);
                    //// if (wieldedTreasure != null)
                    ////{
                    ////    // TODO: get randomly generated wielded treasure from LootGenerationFactory
                    ////    //log.Debug($"{_generator.Name}.TreasureGenerator(): found wielded treasure {Biota.WeenieClassId}");

                    ////    // roll into the wielded treasure table
                    ////    var table = new TreasureWieldedTable(wieldedTreasure);
                    ////    return Generator.GenerateWieldedTreasureSets(table);
                    ////}
                    ////else
                    ////{
                    ////    log.Debug($"{Generator.Name}.TreasureGenerator(): couldn't find death treasure or wielded treasure for ID {Biota.WeenieClassId}");
                    ////    return new List<WorldObject>();
                    ////}
                }
            }
            DisplayStats(ls);

            return test;
        }
        public static LootStats LootStats(WorldObject wo, LootStats ls)
        {
            string dataToPrint = "";
          
            // Weapon Properties 
            double missileDefMod = 0.00f;
            double magicDefMod = 0.00f;
            double wield = 0.00f;
            int value = 0;
            // Loop depending on how many items you are creating
            for (int i = 0; i < 1; i++)
            {
                var testItem = wo;
                if (testItem is null)
                {
                    ls.NullCount++;
                    continue;
                }
                string itemType = testItem.ItemType.ToString();
                if (itemType == null)
                {
                    ls.NullCount++;

                    continue;
                }

                switch (itemType)
                {
                    case "Armor":
                        ls.ArmorCount++;
                        break;
                    case "MeleeWeapon":
                        ls.MeleeWeaponCount++;
                        if (testItem.WeaponMagicDefense != null)
                            magicDefMod = testItem.WeaponMagicDefense.Value;
                        if (testItem.Value != null)
                            value = testItem.Value.Value;
                        if (testItem.WeaponMissileDefense != null)
                            missileDefMod = testItem.WeaponMissileDefense.Value;
                        if (testItem.WieldDifficulty != null)
                            wield = testItem.WieldDifficulty.Value;
                        if (testItem.WeaponSkill == Skill.TwoHandedCombat)
                            ls.MeleeWeapons = ls.MeleeWeapons + $" {testItem.WeaponSkill}\t {wield}\t {testItem.Damage.Value}\t\t {testItem.DamageVariance.Value}\t\t {testItem.WeaponDefense.Value}\t\t {magicDefMod}\t\t {missileDefMod}\t\t {value}\t {testItem.Name}\n";
                        else
                            ls.MeleeWeapons = ls.MeleeWeapons + $" {testItem.WeaponSkill}\t\t {wield}\t {testItem.Damage.Value}\t\t {testItem.DamageVariance.Value}\t\t {testItem.WeaponDefense.Value}\t\t {magicDefMod}\t\t {missileDefMod}\t\t {value}\t {testItem.Name}\n";

                        break;
                    case "Caster":
                        ls.CasterCount++;
                        double eleMod = 0.00f;
                        if (testItem.WeaponMagicDefense != null)
                            magicDefMod = testItem.WeaponMagicDefense.Value;
                        if (testItem.Value != null)
                            value = testItem.Value.Value;
                        if (testItem.WeaponMissileDefense != null)
                            missileDefMod = testItem.WeaponMissileDefense.Value;
                        if (testItem.WieldDifficulty != null)
                            wield = testItem.WieldDifficulty.Value;
                        if (testItem.ElementalDamageMod != null)
                            eleMod = testItem.ElementalDamageMod.Value;
                        if (testItem.ItemMaxMana != null)
                            ls.ItemMaxMana = testItem.ItemMaxMana.Value;

                        ls.CasterWeapons = ls.CasterWeapons + $" {wield}\t {eleMod}\t\t {testItem.WeaponDefense.Value}\t\t  {magicDefMod}\t\t {missileDefMod}\t\t {value}\t {ls.ItemMaxMana}\n";
                        break;
                    case "MissileWeapon":
                        
                        double eleBonus = 0.00f;
                        double damageMod = 0.00f;
                        string missileType = "Other";
                        if (testItem.AmmoType != null)
                        {
                            switch (testItem.AmmoType.Value)
                            {
                                case ACE.Entity.Enum.AmmoType.None:
                                    break;
                                case ACE.Entity.Enum.AmmoType.Arrow:
                                    missileType = " Bow";
                                    ls.MissileWeaponCount++;
                                    break;
                                case ACE.Entity.Enum.AmmoType.Bolt:
                                    missileType = " X Bow";
                                    ls.MissileWeaponCount++;
                                    break;
                                case ACE.Entity.Enum.AmmoType.Atlatl:
                                    missileType = " Thrown";
                                    ls.MissileWeaponCount++;
                                    break;
                                case ACE.Entity.Enum.AmmoType.ArrowCrystal:
                                    break;
                                case ACE.Entity.Enum.AmmoType.BoltCrystal:
                                    break;
                                case ACE.Entity.Enum.AmmoType.AtlatlCrystal:
                                    break;
                                case ACE.Entity.Enum.AmmoType.ArrowChorizite:
                                    break;
                                case ACE.Entity.Enum.AmmoType.BoltChorizite:
                                    break;
                                case ACE.Entity.Enum.AmmoType.AtlatlChorizite:
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (testItem.WeaponMagicDefense != null)
                            magicDefMod = testItem.WeaponMagicDefense.Value;
                        if (testItem.Value != null)
                            value = testItem.Value.Value;
                        if (testItem.WeaponMissileDefense != null)
                            missileDefMod = testItem.WeaponMissileDefense.Value;
                        if (testItem.WieldDifficulty != null)
                            wield = testItem.WieldDifficulty.Value;
                        if (testItem.ElementalDamageBonus != null)
                            eleBonus = testItem.ElementalDamageBonus.Value;
                        if (testItem.DamageMod != null)
                            damageMod = testItem.DamageMod.Value;

                        if (missileType == "Other")
                        {
                        }
                        else
                        ls.MissileWeapons = ls.MissileWeapons + $"{missileType}\t {wield}\t {Math.Round(damageMod, 2)}\t\t{eleBonus}\t\t {testItem.WeaponDefense.Value}\t\t {magicDefMod}\t\t {missileDefMod}\t\t {value}\n";
                        break;
                    case "Jewelry":
                        ls.JewelryCount++;
                        break;
                    case "Gem":
                        ls.GemCount++;
                        break;
                    case "Clothing":
                        ls.ClothingCount++;
                        break;
                    default:
                        ls.OtherCount++;

                        break;
                }
                if (testItem.ItemMaxMana != null)
                {
                    if (testItem.ItemMaxMana > ls.MaxMana)
                        ls.MaxMana = testItem.ItemMaxMana.Value;
                    if (testItem.ItemMaxMana < ls.MinMana)
                        ls.MinMana = testItem.ItemMaxMana.Value;
                    ls.HasManaCount++;
                    ls.TotalMaxMana = ls.TotalMaxMana + testItem.ItemMaxMana.Value;
                }
                if (testItem == null)
                {
                    Console.WriteLine("*Name is Null*");
                    continue;
                }
                else
                {
                }
            }
            return ls;
        }
        private static string DisplayStats(LootStats ls)
        {
            string displayStats = "";

            float totalItemsGenerated = ls.ArmorCount + ls.MeleeWeaponCount + ls.CasterCount + ls.MissileWeaponCount + ls.JewelryCount + ls.GemCount + ls.ClothingCount + ls.OtherCount;
            Console.WriteLine($" Armor={ls.ArmorCount} \n " +
                                $"MeleeWeapon={ls.MeleeWeaponCount} \n " +
                                $"Caster={ls.CasterCount} \n " +
                                $"MissileWeapon={ls.MissileWeaponCount} \n " +
                                $"Jewelry={ls.JewelryCount} \n " +
                                $"Gem={ls.GemCount} \n " +
                                $"Clothing={ls.ClothingCount} \n " +
                                $"Other={ls.OtherCount} \n " +
                                $"NullCount={ls.NullCount} \n " +
                                $"TotalGenerated={totalItemsGenerated}");
            Console.WriteLine();
            Console.WriteLine($" Drop Rates \n " +
                                $"Armor= {ls.ArmorCount / totalItemsGenerated * 100}% \n " +
                                $"MeleeWeapon= {ls.MeleeWeaponCount / totalItemsGenerated * 100}% \n " +
                                $"Caster= {ls.CasterCount / totalItemsGenerated * 100}% \n " +
                                $"MissileWeapon= {ls.MissileWeaponCount / totalItemsGenerated * 100}% \n " +
                                $"Jewelry= {ls.JewelryCount / totalItemsGenerated * 100}% \n " +
                                $"Gem= {ls.GemCount / totalItemsGenerated * 100}% \n " +
                                $"Clothing= {ls.ClothingCount / totalItemsGenerated * 100}% \n " +
                                $"Other={ls.OtherCount / totalItemsGenerated * 100}% \n  ");

            Console.WriteLine(ls.MeleeWeapons);
            Console.WriteLine(ls.MissileWeapons);
            Console.WriteLine(ls.CasterWeapons);
            if (ls.HasManaCount == 0)
            {
            }
            else
            {
                Console.WriteLine($" Mana capacity across all items Min={ls.MinMana}  Max={ls.MaxMana} Avg Mana={ls.TotalMaxMana / ls.HasManaCount}");
            }
            if (ls.MinItemsCreated == 100)
            { }
            else
            {
                Console.WriteLine($" Min Items on a corpse = {ls.MinItemsCreated}, Max Items on coprse = {ls.MaxItemsCreated}");
            }

            return displayStats;
        }
        public static LootStats SetLootStatsDefaults(LootStats ls)
        {
            // Counters
            ls.ArmorCount = 0;
            ls.MeleeWeaponCount = 0;
            ls.CasterCount = 0;
            ls.MissileWeaponCount = 0;
            ls.JewelryCount = 0;
            ls.GemCount = 0;
            ls.ClothingCount = 0;
            ls.OtherCount = 0;
            ls.NullCount = 0;
            ls.ItemMaxMana = 0;
            ls.MinMana = 50000;
            ls.MaxMana = 0;
            ls.HasManaCount = 0;
            ls.TotalMaxMana = 0;
            ls.MinItemsCreated = 100;
            ls.MaxItemsCreated = 0;

            // Tables
            ls.MeleeWeapons = $"-----Melee Weapons----\n Skill \t\t\t Wield \t Damage \t Variance \t DefenseMod \t MagicDBonus \t MissileDBonus\t Value\t Type \n";
            ls.MissileWeapons = $"-----Missile Weapons----\n Type \t Wield \t Modifier \tElementBonus \t DefenseMod \t MagicDBonus \t MissileDBonus\t Value\n";
            ls.CasterWeapons = $"-----Caster Weapons----\n Wield \t ElementBonus \t DefenseMod \t MagicDBonus \t MissileDBonus \t Value \t MaxMana\n";

            return ls;
        }
    }
}
