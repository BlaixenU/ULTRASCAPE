using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System.IO;
using UltraScape.API;
using UnityEngine.AddressableAssets;

namespace UltraScape;

static public class PluginInfo {
    public const string PLUGIN_GUID = "com.blaixenu.ultrascape";
    public const string PLUGIN_NAME = "ULTRASCAPE";
    public const string PLUGIN_VERSION = "1.0.0";
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger { get; private set; } = null!;

    public static string ModDir => Path.GetDirectoryName(typeof(Plugin).Assembly.Location);

    public static readonly AssetBundle Bundle = AssetBundle.LoadFromFile($"{ModDir}/ultrascape.bundle");

    public static GameObject Baby => Bundle.LoadAsset<GameObject>("Baby");
        
    public static SpawnableObject BabyBestiary => Bundle.LoadAsset<SpawnableObject>("Baby Bestiary");

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        // add to bestiary / spawner arm

        SpawnableObjectsDatabase bestiary = Addressables.LoadAssetAsync<SpawnableObjectsDatabase>("Assets/Data/Bestiary Database.asset").WaitForCompletion();
        SpawnableObjectsDatabase sandbox = Addressables.LoadAssetAsync<SpawnableObjectsDatabase>("Assets/Data/Sandbox/Spawnable Objects Database.asset").WaitForCompletion();

        bestiary.enemies = [.. bestiary.enemies, BabyBestiary];
        sandbox.enemies = [.. sandbox.enemies, BabyBestiary];
    }
}

