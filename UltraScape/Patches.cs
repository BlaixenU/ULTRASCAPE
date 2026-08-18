using HarmonyLib;
using UltraScape.Enemies;

namespace UltraScape
{
    [HarmonyPatch]
    public static class Patches
    {
        /* [HarmonyPostfix]
        [HarmonyPatch(typeof(Revolver), nameof(Revolver.Shoot))]
        public static void UNLEASHTHEINFANT()
        {
            var baby = UnityEngine.Object.Instantiate(Plugin.Baby, NewMovement.Instance.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
        } */

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndlessGrid), nameof(EndlessGrid.NextWave))]
        public static void WaveChangeTracker(ref EndlessGrid __instance)
        {
            EnemyManager.Instance!.WaveChange(__instance.currentWave);
        }
    }
}
