using BepInEx;
using HarmonyLib;
using LethalCompanyModV2.Component;
using LethalCompanyScalingMaster;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;
using PluginInfo = LethalCompanyScalingMaster.PluginInfo;

namespace LethalCompanyModV2
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool _loaded;
        public static int DeadlineAmount;
        public static float DeathPenalty = 0.2f;

        public static bool AutoUpdateQuota = true;
        
        Harmony _harmony;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is starting up...");
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!!");
        }


        private void OnDestroy()
        {
            if (!_loaded)
            {
                GameObject gameObject = new GameObject("ControlManager");
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<ControlManager>();


                LC_API.ServerAPI.ModdedServer.SetServerModdedOnly();
                _loaded = true;
            }
        }

        public static void AutoUpdateOnPlayerJoin()
        {
            Debug.Log("A player joined, the new player count is: " + NetworkManager.Singleton.ConnectedClients.Count);
            if (AutoUpdateQuota)
            {
                Debug.Log("Auto Updating Quotas to match the amount of players:  " + NetworkManager.Singleton.ConnectedClients.Count);

                UpdateAndSyncValues();
            }
        }


        public static void UpdateAndSyncValues()
        {
            var instance = TimeOfDay.Instance;

            Debug.Log("Updating Profit Quota's " + NetworkManager.Singleton.ConnectedClients.Count);

            if (!instance.IsServer)
                return;


            instance.SyncNewProfitQuotaClientRpc(instance.profitQuota, 0,
                instance.timesFulfilledQuota);

            instance.SyncTimeClientRpc(instance.globalTime, (int)(DeadlineAmount * instance.totalTime));
        }


    
    }
}