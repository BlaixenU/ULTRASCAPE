using HarmonyLib;
using UltraScape.Enemies;

namespace UltraScape
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndlessGrid), nameof(EndlessGrid.NextWave))]
        public static void WaveChangeTracker(ref EndlessGrid __instance)
        {
            if ((bool)(EnemyManager.Instance?.endlessEnabled)!)
            {
                EnemyManager.Instance?.WaveChange(__instance.currentWave);
            }

            Plugin.Log($"Wave Change to {__instance.currentWave} detected.");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.CanSubmitScores), MethodType.Getter)]
        private static void ScoresSubmission(ref bool __result)
        {
            if (SceneHelper.CurrentScene == "Endless" && (bool)EnemyManager.Instance?.endlessEnabled!)
            {
                __result = false;
            }
        }
    }
}