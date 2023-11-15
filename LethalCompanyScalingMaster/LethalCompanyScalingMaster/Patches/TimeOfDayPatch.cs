using HarmonyLib;
using LethalCompanyModV2.Component;
using UnityEngine;

namespace LethalCompanyModV2.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
[HarmonyPatch("SetNewProfitQuota")]
class QuotaPatch
{
    static bool Prefix()
    {
        var instance = TimeOfDay.Instance;
        Debug.Log("SetNewProfitQuotaPatch");
        instance.timesFulfilledQuota++;
        int num = instance.quotaFulfilled - instance.profitQuota;

        float num2 = Mathf.Clamp(
            1f + (float)instance.timesFulfilledQuota * ((float)instance.timesFulfilledQuota / instance.quotaVariables.increaseSteepness),
            0f, 10000f);
        num2 = instance.quotaVariables.baseIncrease * num2;

        instance.profitQuota = (int)Mathf.Clamp((float)instance.profitQuota + num2, 0f, 1E+09f);

        instance.quotaFulfilled = 0;
        float timeUntilDeadline = instance.totalTime * instance.quotaVariables.deadlineDaysAmount;

        int overtimeBonus = num / 5 + 15 * instance.daysUntilDeadline;
        Debug.Log("Syncing new profit quota!");

        TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(instance.profitQuota, overtimeBonus, instance.timesFulfilledQuota);

        return false;
    }
}