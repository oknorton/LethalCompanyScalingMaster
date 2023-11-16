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
        public static int groupCredits;
        public static bool updateQuota = false;
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

        public static int GetConnectedPlayers()
        {
            return StartOfRound.Instance.connectedPlayersAmount + 1;
        }
        public static void OnPlayerJoin()
        {
            if (AutoUpdateQuota)
            {
                SaveValues();
            }
        }

        public static void SaveValues()
        {
            Debug.Log("NON parsed values= " + GUIManager._baseQuota + " + "  + GUIManager._playerCountQuotaModifier + " X " + NetworkManager.Singleton.ConnectedClients.Count);

            if (float.TryParse(GUIManager._baseQuota, out float baseQuotaParsed) && float.TryParse(GUIManager._playerCountQuotaModifier,
                    out float playerCountQuotaModifierParsed))
            {
                Debug.Log("Parsed values= " + baseQuotaParsed + " + "  + playerCountQuotaModifierParsed + " X " + NetworkManager.Singleton.ConnectedClients.Count);
                int startingQuota = (int)(baseQuotaParsed + (NetworkManager.Singleton.ConnectedClients.Count * playerCountQuotaModifierParsed));

                GUIManager._tod.quotaVariables.startingQuota = startingQuota;
                GUIManager._tod.profitQuota = startingQuota;
            }

            if (float.TryParse(GUIManager._baseIncreaseInput, out float baseIncrease))
            {
                GUIManager. _tod.quotaVariables.baseIncrease = baseIncrease;
            }

            if (int.TryParse(GUIManager._daysUntilDeadlineInput, out int daysUntilDeadline))
            {
                DeadlineAmount = daysUntilDeadline;
                GUIManager._tod.quotaVariables.deadlineDaysAmount = daysUntilDeadline;
            }

            GUIManager._tod.quotaVariables.increaseSteepness = GUIManager._quotaIncreaseSteepness;
            // Plugin.DeathPenalty = _tempDeathPenalty;
            groupCredits= GUIManager._totalStartingCredits;
            UpdateAndSyncValues();
        }

        public static void UpdateAndSyncValues()
        {
            var instance = TimeOfDay.Instance;

            Debug.Log("Updating Profit Quota's " + NetworkManager.Singleton.ConnectedClients.Count);

            if (!instance.IsServer)
                return;

            Terminal terminal = FindObjectOfType<Terminal>();

            if (!terminal.IsServer)
            {
                Debug.Log("This terminal instance is not a server!!!");
            }
            else
            {
                Debug.Log("This terminal instance is a server");
                terminal.SyncGroupCreditsClientRpc(groupCredits, terminal.numberOfItemsInDropship);
            }

            instance.SyncNewProfitQuotaClientRpc(instance.profitQuota, 0,
                instance.timesFulfilledQuota);

            instance.SyncTimeClientRpc(instance.globalTime, (int)(DeadlineAmount * instance.totalTime));
        }
    }
}