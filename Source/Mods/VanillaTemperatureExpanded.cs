using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat;

/// <summary>Vanilla Temperature Expanded by Oskar Potocki, xrushha, Arquebus, Taranchuk</summary>
/// <see href="https://github.com/Vanilla-Expanded/VanillaTemperatureExpanded"/>
/// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=3202046258"/>
[MpCompatFor("VanillaExpanded.Temperature")]
public class VanillaTemperatureExpanded
{
    public VanillaTemperatureExpanded(ModContentPack mod)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);

        // Unlink/relink
        MpCompat.RegisterLambdaDelegate("VanillaTemperatureExpanded.Comps.CompAcTempControl", nameof(ThingComp.CompGetGizmosExtra), 0);
    }

    private static void LatePatch()
    {
        var type = AccessTools.TypeByName("VanillaTemperatureExpanded.Buildings.Building_AcControlUnit");
        // Change temperature by +/- 1/10 
        MP.RegisterSyncMethod(type, "InterfaceChangeTargetNetworkTemperature");
        // Reset temperature
        MP.RegisterSyncMethodLambda(type, nameof(Thing.GetGizmos), 2);

        var compHeatPusherType = AccessTools.TypeByName("VanillaTemperatureExpanded.Comps.CompHeatPusherPoweredWithIdleDraw");
        var shouldPushProp = AccessTools.PropertyGetter(compHeatPusherType, "ShouldPushHeatNow");
        if (shouldPushProp != null)
        {
            MpCompat.harmony.Patch(shouldPushProp, prefix: new HarmonyMethod(typeof(VanillaTemperatureExpanded), nameof(ShouldPushHeatNowPrefix)));
        }
    }

    private static bool ShouldPushHeatNowPrefix(ThingComp __instance, ref bool __result)
    {
        if (__instance.parent is not { Spawned: true, Map: not null })
        {
            __result = false;
            return false;
        }

        return true;
    }
}