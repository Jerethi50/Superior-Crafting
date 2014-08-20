using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using VerseBase;
namespace SuperiorCrafting
{
    public static class SCUpgrades
    {
        public static void NutrientResynthesisII()
        {
            DefDatabase<ThingDef>.GetNamed("NutrientPasteDispenser").building.foodCostPerDispense = 6;
        }
        public static void AgricultureI()
        {
            DefDatabase<ThingDef>.GetNamed("PlantAgaveCultivated").plant.sowableGround = true;
            DefDatabase<ThingDef>.GetNamed("PlantStrawberry").plant.sowableGround = true;
            DefDatabase<ThingDef>.GetNamed("PlantAgaveCultivated").plant.sowableHydroponic = true;
            DefDatabase<ThingDef>.GetNamed("PlantStrawberry").plant.sowableHydroponic = true;
        }
        public static void CraftingI()
        {
            DefDatabase<ThingDef>.GetNamed("PlantCotton").plant.sowableGround = true;
        }
        public static void ProteinReplication()
        {
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").food.nutrition = 75;
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").food.quality = FoodQuality.MealSimple;
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").food.eatenDirectThought = ThoughtDef.Named("AteSimpleMeal");
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").description = ("A replicated meal prepared quickly from one ingredient.  It's almost as good as home cooking!");
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").label = ("Replicated Simple Meal");
            DefDatabase<ThingDef>.GetNamed("MealNutrientPaste").texturePath = ("Salad");
        }
        public static void ConstructionIV()
        {
            DefDatabase<ThingDef>.GetNamed("WallMetal").texturePath = ("Things/Building/SCWalls/WallMetalUpgrade_Atlas");
        }
    }
}