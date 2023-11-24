using HarmonyLib;
using LethalCompanyModV2;
using Unity.Netcode;
using UnityEngine;

namespace LethalCompanyScalingMaster.Patches;

[HarmonyPatch(typeof(StartOfRound))]
[HarmonyPatch("OnPlayerConnectedClientRpc")]
public class OnPlayerConnectedClientRpcPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "OnPlayerConnectedClientRpc")]
    static void Postfix(ulong clientId, int connectedPlayers, int profitQuota)
    {
      
        Plugin.OnPlayerJoin();

    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    static void AwakePostFix()
    {
        Debug.Log("Starting the patch: Postfix for OnPlayerConnectedClientRpc()");
      
        Plugin.Host = RoundManager.Instance.NetworkManager.IsHost;
        Debug.Log("Plugin host has been set to: " + RoundManager.Instance.NetworkManager.IsHost);

    }
}

