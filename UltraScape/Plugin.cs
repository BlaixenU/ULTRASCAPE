using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System.IO;
using UltraScape.API.Enemies;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

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

    public static AssetBundle Bundle = AssetBundle.LoadFromFile($"{ModDir}/ultrascape");
        
    public static SpawnableObject[] SpawnableEnemies = Bundle.LoadAllAssets<SpawnableObject>();

    public static GameObject Baby = Bundle.LoadAsset<GameObject>("Baby");

    private static bool hasAddedEnemies = false;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;

        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        // add to bestiary / spawner arm

        Log("Harmony patching completed.");

        Log($"ModDir: {ModDir}");

        Log("Assets found in bundle:");
        foreach (var enemy in Bundle.LoadAllAssets())
        {
            Log(enemy.name);
        }

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void AddEnemies()
    {
        hasAddedEnemies = true;

        Log("Starting adding enemies to bestiary");

        Log("Loading bestiary");
        SpawnableObjectsDatabase bestiary = Addressables.LoadAssetAsync<SpawnableObjectsDatabase>("Assets/Data/Bestiary Database.asset").WaitForCompletion();
        Log("Loading bestiary success");

        Log("Adding enemies to bestiary");
        bestiary.enemies = [.. bestiary.enemies, .. SpawnableEnemies];
        Log("Adding enemies to bestiary success");

        // return;

        Log("starting adding enemies to sandbox");

        Log("Loading sandbox");
        SpawnableObjectsDatabase sandbox = Addressables.LoadAssetAsync<SpawnableObjectsDatabase>("Assets/Data/Sandbox/Spawnable Objects Database.asset").WaitForCompletion();
        Log("Loading sandbox success");

        Log("Adding enemies to sandbox");
        sandbox.enemies = [.. sandbox.enemies, .. SpawnableEnemies];
        Log("Adding enemies to sandbox success");
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (hasAddedEnemies == false)
        {
            AddEnemies();
        }
    }


    private void Log(string log)
    {
        Logger.LogInfo(log); // I DONT LIKE TYPING ALLAT
    }
}

[HarmonyPatch]
public static class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Revolver), nameof(Revolver.Shoot))]
    public static void UNLEASHTHEINFANT()
    {
        var baby = UnityEngine.Object.Instantiate(Plugin.Baby, NewMovement.Instance.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
    }
}