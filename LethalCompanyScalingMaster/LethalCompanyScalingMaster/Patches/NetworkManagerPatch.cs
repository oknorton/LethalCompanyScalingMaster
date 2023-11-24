using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;
using System.Reflection;
using UnityEngine;

public static class NetworkManagerPatch
{
    // [HarmonyPostfix]
    // [HarmonyPatch(typeof(NetworkManager), "ConnectedClients")]
    // public static void ConnectedClientsPostfix(NetworkManager __instance, ref IReadOnlyDictionary<ulong, NetworkClient> __result)
    // {
    //     if (!__instance.IsServer)
    //     {
    //         var connectionManagerField = typeof(NetworkManager).GetField("ConnectionManager", BindingFlags.NonPublic | BindingFlags.Instance);
    //         var connectionManager = connectionManagerField.GetValue(__instance);
    //         var connectedClientsField = connectionManager.GetType().GetField("ConnectedClients");
    //         __result = (IReadOnlyDictionary<ulong, NetworkClient>)connectedClientsField.GetValue(connectionManager);
    //         Debug.Log("Ignoring isServer check requirement and retrieving the value either way." + __result.Count);
    //     }
    // }

}