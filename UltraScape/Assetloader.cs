using UnityEngine;

namespace UltraScape
{
    internal static class Assetloader
    {
        internal static AssetBundle Bundle = AssetBundle.LoadFromFile($"{Plugin.ModDir}/ultrascape");

        // enemies

        internal static SpawnableObject[] SpawnableEnemies = Bundle.LoadAllAssets<SpawnableObject>();

        internal static GameObject Baby = Bundle.LoadAsset<GameObject>("Baby");

        internal static GameObject Telefragger = Bundle.LoadAsset<GameObject>("Telefragger");
    }
}