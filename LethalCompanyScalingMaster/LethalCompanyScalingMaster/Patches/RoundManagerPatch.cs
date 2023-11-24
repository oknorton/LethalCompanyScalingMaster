using HarmonyLib;
using LethalCompanyModV2;
using UnityEngine;

namespace LethalCompanyScalingMaster.Patches;

// public static class RoundManagerPatch
// {
//     [HarmonyPatch(typeof(RoundManager), "Start")]
//     [HarmonyPrefix]
//     static void StartPatch()
//     {
//         Debug.Log("ROUND MANAGER HAS BEEN PATCHED");
//
//         Debug.Log("Host status:" + Plugin.Host);
//     }
// }