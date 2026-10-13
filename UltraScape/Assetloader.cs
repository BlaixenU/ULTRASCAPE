using UltraScape.Enemies;
using UnityEngine;

namespace UltraScape
{
    internal static class Assetloader
    {
        internal static AssetBundle Bundle = AssetBundle.LoadFromFile($"{Plugin.ModDir}/ultrascape");

        // Enemies

        internal static SpawnableObject[] SpawnableEnemies = Bundle.LoadAllAssets<SpawnableObject>();

        internal static EnemyScriptableObject[] EnemyScriptableObjects = Bundle.LoadAllAssets<EnemyScriptableObject>();

        // Managers

        internal static GameObject enemyManager = Bundle.LoadAsset<GameObject>("EnemyManager");
    }
}