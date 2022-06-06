using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace DontGoThere
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        public static Harmony harmonyInstance;


        static HarmonyPatches()
        {
            harmonyInstance = new Harmony("rimworld.rwmods.DontGoThere");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

    }


    [HarmonyPatch(typeof(ForbidUtility))]
    [HarmonyPatch("IsForbiddenToPass")]
    internal static class CheckIsForbiddenToPassAllies
    {

        [HarmonyPostfix]
        public static void Postfix(Building_Door t, Pawn pawn, ref bool __result)
        {
            // If not yet forbidden
            if (__result == false)
            {
                // dont override if no faction
                if (pawn.HomeFaction == null)
                {
                    //Log.Message("No Faction");
                    return;
                }

                // dont override if a prisoner
                if (pawn.IsPrisoner == true)
                {
                    //Log.Message("Prisoner");
                    return;
                }

                // dont override if not home
                if (pawn.Map.IsPlayerHome == false)
                {
                    //Log.Message("Not player home");
                    return;
                }

                // dont override if player faction
                if (pawn.HomeFaction == Faction.OfPlayer)
                {
                    //Log.Message("Is Player");
                    return;
                }

                // dont override if mental breakdown
                if (pawn.InMentalState == true)
                {
                    //Log.Message("Is mental ");
                    return;
                }

                // dont override if rebelling
                if (SlaveRebellionUtility.IsRebelling(pawn) == true)
                {
                    //Log.Message("Is rebell");
                    return;
                }

                // dont override if following someone
                if (ThinkNode_ConditionalShouldFollowMaster.ShouldFollowMaster(pawn) == true)
                {
                    //Log.Message("Is following");
                    return;
                }


                // Check if the target is even forbidden
                if (t is ThingWithComps thingWithComps)
                {
                    //Log.Message("Has things");
                    CompForbiddable comp = thingWithComps.GetComp<CompForbiddable>();

                    if (comp != null)
                    {
                       // Log.Message("Has forbid");
                        if (comp.Forbidden)
                        {
                            //Log.Message("Is forbid");
                            if (pawn.HomeFaction.GoodwillWith(Faction.OfPlayer) > -20)
                            {
                                //Log.Error("ListenToForbid");
                                __result = true;    // Listen to this forbid
                            }
                        }
                    }
                }
            }
        }
    }
}
