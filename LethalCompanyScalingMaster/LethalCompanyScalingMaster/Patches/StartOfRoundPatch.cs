using HarmonyLib;
using UnityEngine;

namespace LethalCompanyModV2.Patches;

[HarmonyPatch(typeof(StartOfRound))]
[HarmonyPatch("OnPlayerConnectedClientRpc")]
public class OnPlayerConnectedClientRpcPatch
{
    static void Postfix(ulong clientId, int connectedPlayers, int profitQuota)
    {
        Debug.Log("Starting the patch: Postfix for OnPlayerConnectedClientRpc()");
        
        Plugin.AutoUpdateOnPlayerJoin();

    }
}

