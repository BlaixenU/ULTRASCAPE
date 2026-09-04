using UnityEngine;

namespace UltraScape
{
    internal static class Assetloader
    {
        internal static AssetBundle Bundle = AssetBundle.LoadFromFile($"{Plugin.ModDir}/ultrascape");

        // enemies

        internal static SpawnableObject[] SpawnableEnemies = Bundle.LoadAllAssets<SpawnableObject>();

        internal static GameObject enemyManager = Bundle.LoadAsset<GameObject>("EnemyManager Prefab");
    }
}