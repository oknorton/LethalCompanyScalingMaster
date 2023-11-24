using HarmonyLib;
using LethalCompanyModV2;
using UnityEngine;

namespace LethalCompanyScalingMaster.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyPatch("ApplyPenalty")]
    class DeathPenaltyPatch
    {
        static bool Prefix(ref int playersDead, ref int bodiesInsured)
        {
            float newDeathPenaltyPercentage = (float)Plugin.DeathPenalty/100;


            float num = newDeathPenaltyPercentage;

            Terminal objectOfType = UnityEngine.Object.FindObjectOfType<Terminal>();

            int groupCredits = objectOfType.groupCredits;
            bodiesInsured = Mathf.Max(bodiesInsured, 0);

            for (int index = 0; index < playersDead - bodiesInsured; ++index)
                objectOfType.groupCredits -= (int)((double)groupCredits * (double)num);

            for (int index = 0; index < bodiesInsured; ++index)
                objectOfType.groupCredits -= (int)((double)groupCredits * ((double)num / 2.5));

            if (objectOfType.groupCredits < 0)
                objectOfType.groupCredits = 0;
            Debug.Log("Death penalty is:" + num);
            HUDManager.Instance.statsUIElements.penaltyAddition.text = string.Format(
                "{0} casualties: -{1}%\n({2} bodies recovered)", (object)playersDead,
                (object)(float)((double)num * 100.0 * (double)(playersDead - bodiesInsured)), (object)bodiesInsured);
            HUDManager.Instance.statsUIElements.penaltyTotal.text =
                string.Format("DUE: ${0}", (object)(groupCredits - objectOfType.groupCredits));
            Debug.Log((object)string.Format("New group credits after penalty: {0}", (object)objectOfType.groupCredits));

            return false;
        }
    }

    [HarmonyPatch(typeof(HUDManager), "Start")]
    public static class HUDManagerStartPatch
    {
        [HarmonyPrefix]
        public static void ApplyStartPrefix()
        {
            Debug.Log("APPLYING HUDMANAGER PREFIX");
        }
    }
}