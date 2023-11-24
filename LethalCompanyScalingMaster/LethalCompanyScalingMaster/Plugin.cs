using BepInEx;
using HarmonyLib;
using LethalCompanyModV2.Component;
using LethalCompanyScalingMaster.Component;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LethalCompanyScalingMaster
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool _loaded;
        public static int DeadlineAmount;
        public static int groupCredits;
        public static bool AutoUpdateQuota = true;
        public static bool Host { get; set; }
        public static int DeathPenalty { get; set; }

        Harmony _harmony;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is starting up...");
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!");
        }

        private void OnDestroy()
        {
            if (!_loaded)
            {
                GameObject gameObject = new GameObject("DontDestroy");
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<ControlManager>();
                gameObject.AddComponent<BroadcastingComponent>();


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
            if (float.TryParse(GUIManager._baseQuota, out float baseQuotaParsed) && float.TryParse(
                    GUIManager._playerCountQuotaModifier,
                    out float playerCountQuotaModifierParsed))
            {
                int startingQuota = (int)(baseQuotaParsed + (GetConnectedPlayers() * playerCountQuotaModifierParsed));

                GUIManager._tod.quotaVariables.startingQuota = startingQuota;
                GUIManager._tod.profitQuota = startingQuota;
            }

            if (float.TryParse(GUIManager._baseIncreaseInput, out float baseIncrease))
            {
                GUIManager._tod.quotaVariables.baseIncrease = baseIncrease;
            }

            if (int.TryParse(GUIManager._daysUntilDeadlineInput, out int daysUntilDeadline))
            {
                DeadlineAmount = daysUntilDeadline;
                GUIManager._tod.quotaVariables.deadlineDaysAmount = daysUntilDeadline;
            }

            GUIManager._tod.quotaVariables.increaseSteepness = GUIManager._quotaIncreaseSteepness;
            // Plugin.DeathPenalty = _tempDeathPenalty;
            groupCredits = GUIManager._totalStartingCredits;

            DeathPenalty = GUIManager._deathPenalty;

            BroadcastingComponent.BroadcastDeathPenalty(DeathPenalty);
            UpdateAndSyncValues();
        }

        public static void UpdateAndSyncValues()
        {
            var instance = TimeOfDay.Instance;

            Debug.Log("Updating Profit Quota's " + GetConnectedPlayers());

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