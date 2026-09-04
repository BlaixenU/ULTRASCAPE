using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

namespace UltraScape
{
    static public class PluginInfo
    {
        public const string PLUGIN_GUID = "com.blaixenu.ultrascape";
        public const string PLUGIN_NAME = "ULTRASCAPE";
        public const string PLUGIN_VERSION = "0.1.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger { get; private set; } = null!;

        public static string ModDir => Path.GetDirectoryName(typeof(Plugin).Assembly.Location);

        private static bool hasAddedEnemies = false;

        private static readonly bool DEBUG_MODE = false;

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
            foreach (var enemy in Assetloader.Bundle.LoadAllAssets())
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
            bestiary.enemies = bestiary.enemies.ToArray().Concat(Assetloader.SpawnableEnemies.ToArray()).ToArray();

            Log("Adding enemies to bestiary success");

            // return;

            Log("starting adding enemies to sandbox");

            Log("Loading sandbox");
            SpawnableObjectsDatabase sandbox = Addressables.LoadAssetAsync<SpawnableObjectsDatabase>("Assets/Data/Sandbox/Spawnable Objects Database.asset").WaitForCompletion();
            Log("Loading sandbox success");

            Log("Adding enemies to sandbox");
            sandbox.enemies = sandbox.enemies.ToArray().Concat(Assetloader.SpawnableEnemies.ToArray()).ToArray();

            Log("Adding enemies to sandbox success");
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (hasAddedEnemies == false)
            {
                AddEnemies();
            }

            Instantiate(Assetloader.enemyManager);
        }


        public static void Log<T>(T log)
        {
            if (DEBUG_MODE == false)
            {
                return;
            }
            Logger.LogInfo(log);
        }

        public static void Log<T>(T[] log)
        {
            if (DEBUG_MODE == false)
            {
                return;
            }


            foreach (T element in log)
            {
                Logger.LogInfo(element);
            }
        }
    }
}